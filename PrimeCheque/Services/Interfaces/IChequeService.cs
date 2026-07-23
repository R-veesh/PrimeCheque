using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IChequeService
    {
        Task<Cheque?> GetChequeByIdAsync(Guid id);
        Task<List<Cheque>> GetChequesAsync(Guid? companyId = null, ChequeStatus? status = null, string? searchQuery = null);
        Task<Cheque> CreateChequeAsync(Cheque cheque, string user);
        Task<Cheque> UpdateChequeAsync(Cheque cheque, string user);
        Task<bool> ApproveChequeAsync(Guid chequeId, string approver);
        Task<bool> VoidChequeAsync(Guid chequeId, string user, string reason);
        Task<bool> MarkAsPrintedAsync(Guid chequeId, string user, string pdfPath);
        Task<bool> IsDuplicateChequeAsync(Guid companyId, string payeeName, decimal amount);
        Task<List<int>> GetMissingSequenceNumbersAsync(Guid chequeBookId);
    }
}
