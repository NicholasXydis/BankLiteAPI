using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using System.Security.Principal;

namespace BankLite.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task DepositAsync(DepositWithdrawDto dto)
        {
            var account = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (account == null) throw new Exception("No Account Found");
            account.Balance += dto.Amount;

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
            };

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);

        }

        public async Task WithdrawAsync(DepositWithdrawDto dto)
        {
            var account = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (account == null) throw new Exception("No Account Found");
            if (dto.Amount > account.Balance) throw new Exception("Insufficient Funds");
            account.Balance -= dto.Amount;

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = TransactionType.Withdrawal,
            };

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
        }

        public async Task TransferAsync(TransferDto dto)
        {
            var fromAccount = await _accountRepository.GetByIdAsync(dto.FromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(dto.ToAccountId);
            if (fromAccount == null) throw new Exception("From Account Not Found");
            if (toAccount == null) throw new Exception("To Account Not Found");
            if (dto.Amount > fromAccount.Balance) throw new Exception("Insufficient Funds");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                fromAccount.Balance -= dto.Amount;
                toAccount.Balance += dto.Amount;
                await _accountRepository.UpdateAsync(fromAccount);
                await _accountRepository.UpdateAsync(toAccount);

                var debitTransaction = new Transaction
                {
                    AccountId = dto.FromAccountId,
                    Amount = dto.Amount,
                    Type = TransactionType.Withdrawal,
                };

                var creditTransaction = new Transaction
                {
                    AccountId = dto.ToAccountId,
                    Amount = dto.Amount,
                    Type = TransactionType.Deposit,
                };

                await _transactionRepository.AddAsync(debitTransaction);
                await _transactionRepository.AddAsync(creditTransaction);

                await _unitOfWork.CommitAsync();
            }

            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResultDto<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, int page, int pageSize)
        {
            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId, page, pageSize);
            var totalCount = await _transactionRepository.GetTotalCountAsync(accountId);
            return new PagedResultDto<Transaction>
            {
                Items = transactions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
        }
    }
}
