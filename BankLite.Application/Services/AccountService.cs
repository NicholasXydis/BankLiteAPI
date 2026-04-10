using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BankLite.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
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
            _logger.LogInformation("Account created for user {UserId}: {AccountNumber}", userId, account.AccountNumber);
            return account;
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Fetching accounts for user {UserId}", userId);
            return await _accountRepository.GetByUserIdAsync(userId);
        }
    }
}
