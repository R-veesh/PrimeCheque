using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IPrintService
    {
        List<string> GetInstalledPrinters();
        Task<bool> PrintPdfAsync(string pdfPath, string printerName, PrinterCalibration? calibration = null);
        Task<PrinterCalibration?> GetCalibrationAsync(string printerName, Guid? templateId = null);
        Task SaveCalibrationAsync(PrinterCalibration calibration);
    }
}
