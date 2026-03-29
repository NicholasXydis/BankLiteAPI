using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using BankLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLite.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankLiteDbContext _context;

        public AccountRepository(BankLiteDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(Guid Id)
        {
            return await _context.Accounts.FindAsync(Id);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            // TODO: Remove SaveChangesAsync from repo - will be handeled by service layer for atomic transfers
            await _context.SaveChangesAsync();
        }
    }
}
