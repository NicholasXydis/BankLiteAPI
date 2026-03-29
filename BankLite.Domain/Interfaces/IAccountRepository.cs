using BankLite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLite.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid Id);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
    }
}
