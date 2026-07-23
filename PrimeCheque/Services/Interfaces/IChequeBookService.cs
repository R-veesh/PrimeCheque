using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrimeCheque.Models;

namespace PrimeCheque.Services.Interfaces
{
    public interface IChequeBookService
    {
        Task<List<ChequeBook>> GetChequeBooksByCompanyAsync(Guid companyId);
        Task<ChequeBook?> GetChequeBookByIdAsync(Guid id);
        Task<ChequeBook> AddChequeBookAsync(ChequeBook chequeBook);
        Task<ChequeBook> UpdateChequeBookAsync(ChequeBook chequeBook);
        Task<int> GetNextChequeNumberAsync(Guid chequeBookId);
    }
}
