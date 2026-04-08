using BankLite.Domain.Interfaces;
using BankLite.Infrastructure.Data;

namespace BankLite.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankLiteDbContext _context;

        public UnitOfWork(BankLiteDbContext context)
        {
            _context = context;
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
