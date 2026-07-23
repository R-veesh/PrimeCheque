using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<Cheque>> GetChequeRegisterAsync(Guid companyId, DateTime startDate, DateTime endDate);
        Task<List<Cheque>> GetPostDatedChequesAsync(Guid companyId, DateTime minDate);
        Task<List<Cheque>> GetVoidChequesAsync(Guid companyId, DateTime startDate, DateTime endDate);
    }
}
