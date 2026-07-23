using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IPayeeService
    {
        Task<List<Payee>> GetPayeesByCompanyAsync(Guid companyId);
        Task<List<Payee>> SearchPayeesAsync(Guid companyId, string query);
        Task<Payee> AddOrUpdatePayeeAsync(Payee payee);
        Task DeletePayeeAsync(Guid id);
    }
}
