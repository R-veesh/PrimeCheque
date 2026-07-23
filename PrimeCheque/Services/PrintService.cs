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
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers;
        }

        public Task<bool> PrintPdfAsync(string pdfPath, string printerName, string? trayName = null)
        {
            // Windows native printing trigger for PDF file
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfPath,
                    Verb = "printto",
                    Arguments = $"\"{printerName}\"",
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };
                process.Start();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<PrinterCalibration?> GetCalibrationAsync(string printerName, Guid? templateId = null)
        {
            return await _dbContext.PrinterCalibrations
                .FirstOrDefaultAsync(pc => pc.PrinterName == printerName && pc.TemplateId == templateId);
        }

        public async Task SaveCalibrationAsync(PrinterCalibration calibration)
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
    }
}
