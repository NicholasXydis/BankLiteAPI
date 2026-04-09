using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;

namespace BankLite.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> CreateAccountAsync(CreateAccountDto dto, Guid userId)
        {
            var account = new Account
            {
                UserId = userId,
                Type = dto.Type,
                AccountNumber = Guid.NewGuid().ToString("N")[..12].ToUpper()
            };

            await _accountRepository.AddAsync(account);
            return account;
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId)
        {
            return await _accountRepository.GetByUserIdAsync(userId);
        }
    }
}
