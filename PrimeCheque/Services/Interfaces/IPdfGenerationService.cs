using System;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IPdfGenerationService
    {
        Task<string> GenerateChequePdfAsync(Cheque cheque, BankTemplate template, PrinterCalibration? calibration = null, string? watermarkText = null);
    }
}
