using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class PrintService : IPrintService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public PrintService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string> GetInstalledPrinters()
        {
            var printers = new List<string>();
            try
            {
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(printer);
                }
            }
            catch
            {
                // Fallback to Microsoft Print to PDF if printer enumeration fails
                printers.Add("Microsoft Print to PDF");
            }
            return printers;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        public async Task<bool> PrintPdfAsync(string pdfPath, string printerName, PrinterCalibration? calibration = null)
        {
            try
            {
                // Pre-load pdfium.dll from known unpackaged / self-contained output directories
                string arch = Environment.Is64BitProcess ? "x64" : "x86";
                string[] possiblePaths = new[]
                {
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arch, "pdfium.dll"),
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdfium.dll"),
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", $"win-{arch}", "native", "pdfium.dll")
                };

                foreach (var path in possiblePaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        LoadLibrary(path);
                        break;
                    }
                }

                // Attempt direct silent print using PdfiumViewer manually rendering to bypass WinForms dependency
                using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
                {
                    using (var printDocument = new System.Drawing.Printing.PrintDocument())
                    {
                        var targetPrinter = !string.IsNullOrWhiteSpace(printerName) ? printerName : new PrinterSettings().PrinterName;
                        printDocument.PrinterSettings.PrinterName = targetPrinter;
                        if (!printDocument.PrinterSettings.IsValid)
                        {
                            printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;
                        }
                        printDocument.PrintController = new System.Drawing.Printing.StandardPrintController(); // Hide print dialog
                        
                        // We do NOT set printDocument.DefaultPageSettings.Landscape = true here,
                        // because many printer drivers (especially EPSON) ignore it for custom sizes.
                        // Instead, we will manually rotate the image during rendering.
                        int currentPage = 0;
                        printDocument.PrintPage += (sender, e) =>
                        {
                            if (e.Graphics != null)
                            {
                                var pageSize = document.PageSizes[currentPage];
                                // PdfiumViewer PageSizes are in Points (72 points = 1 inch)
                                double widthInches = pageSize.Width / 72.0;
                                double heightInches = pageSize.Height / 72.0;
                                
                                // Cap rendering DPI at 300 to prevent OutOfMemory on high-DPI printers
                                float dpiX = Math.Min(300f, e.Graphics.DpiX);
                                float dpiY = Math.Min(300f, e.Graphics.DpiY);

                                int renderWidth = (int)(widthInches * dpiX);
                                int renderHeight = (int)(heightInches * dpiY);

                                // Render the PDF page to a bitmap
                                using (var image = document.Render(currentPage, renderWidth, renderHeight, dpiX, dpiY, PdfiumViewer.PdfRenderFlags.ForPrinting))
                                {
                                    // Calculate print dimensions in hundredths of an inch
                                    float printWidth = (float)(widthInches * 100.0);
                                    float printHeight = (float)(heightInches * 100.0);
                                    
                                    if (calibration != null && calibration.PrintLandscape)
                                    {
                                        // Manually rotate the image 270 degrees (90 degrees counter-clockwise)
                                        // This forces it to print sideways regardless of the printer driver's capabilities
                                        image.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
                                        
                                        // Swap dimensions to match the rotated image
                                        float temp = printWidth;
                                        printWidth = printHeight;
                                        printHeight = temp;
                                    }
                                    
                                    // When OriginAtMargins is false (default), (0,0) is the printable area top-left.
                                    // We need to offset by -HardMargin to draw from the absolute physical edge of the paper.
                                    float offsetX = -e.PageSettings.HardMarginX;
                                    float offsetY = -e.PageSettings.HardMarginY;

                                    e.Graphics.DrawImage(image, new System.Drawing.RectangleF(offsetX, offsetY, printWidth, printHeight));
                                }
                            }
                            
                            currentPage++;
                            e.HasMorePages = currentPage < document.PageCount;
                        };

                        printDocument.Print();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pdfium Print Error: {ex.Message}");
                try { System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PdfiumError.txt"), ex.ToString()); } catch { }
                // Fallback: if native print fails, try launching the file so the user can manually print
                try
                {
                    // Use WinRT Launcher so that the external browser/PDF viewer can bypass MSIX file virtualization
                    var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(pdfPath);
                    await Windows.System.Launcher.LaunchFileAsync(file);
                    return true; 
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<PrinterCalibration?> GetCalibrationAsync(string printerName, Guid? templateId = null)
        {
            try
            {
                return await _dbContext.PrinterCalibrations
                    .FirstOrDefaultAsync(pc => pc.PrinterName == printerName && pc.TemplateId == templateId);
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveCalibrationAsync(PrinterCalibration calibration)
        {
            try
            {
                var existing = await _dbContext.PrinterCalibrations
                    .FirstOrDefaultAsync(pc => pc.PrinterName == calibration.PrinterName && pc.TemplateId == calibration.TemplateId);

                if (existing == null)
                {
                    calibration.CreatedAt = DateTime.UtcNow;
                    _dbContext.PrinterCalibrations.Add(calibration);
                }
                else
                {
                    existing.HorizontalOffsetMm = calibration.HorizontalOffsetMm;
                    existing.VerticalOffsetMm = calibration.VerticalOffsetMm;
                    existing.TrayName = calibration.TrayName;
                    existing.PrintLandscape = calibration.PrintLandscape;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _dbContext.PrinterCalibrations.Update(existing);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Gracefully handle calibration save exceptions
            }
        }
    }
}
