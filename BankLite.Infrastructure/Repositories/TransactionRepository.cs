using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using BankLite.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankLite.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankLiteDbContext _context;

        public TransactionRepository(BankLiteDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int page, int pageSize)
        {
            return await _context.Transactions 
            .Where(t => t.AccountId == accountId)
            .Skip((page -1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(Guid accountId)
        {
            return await _context.Transactions
            .Where (t => t.AccountId == accountId)
            .CountAsync();
        }
    }
}
