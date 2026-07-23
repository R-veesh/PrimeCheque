using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrimeCheque.Data;
using PrimeCheque.Models;
using PrimeCheque.Services.Interfaces;

namespace PrimeCheque.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly PrimeChequeDbContext _dbContext;

        public CompanyService(PrimeChequeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _dbContext.Companies.AsNoTracking().ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid id)
        {
            return await _dbContext.Companies.FindAsync(id);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            company.CreatedAt = DateTime.UtcNow;
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }

        public async Task<Company> UpdateCompanyAsync(Company company)
        {
            company.UpdatedAt = DateTime.UtcNow;
            _dbContext.Companies.Update(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            var company = await _dbContext.Companies.FindAsync(id);
            if (company != null)
            {
                _dbContext.Companies.Remove(company);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
