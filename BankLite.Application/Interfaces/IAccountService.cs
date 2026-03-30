using BankLite.Application.DTOs;
using BankLite.Domain.Entities;

namespace BankLite.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(CreateAccountDto dto, Guid userId);
        Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);
    }
}
