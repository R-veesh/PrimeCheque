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

        public Task<bool> PrintPdfAsync(string pdfPath, string printerName, string? trayName = null)
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfPath,
                    Verb = "printto",
                    Arguments = $"\"{printerName}\"",
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };
                process.Start();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Print Error: {ex.Message}");
                // Fallback: if printto fails, try launching the file so the user can manually print
                try
                {
                    var pInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(pInfo);
                    return Task.FromResult(true); // Return true because we successfully launched it
                }
                catch
                {
                    return Task.FromResult(false);
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
