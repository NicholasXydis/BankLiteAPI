using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;

namespace BankLite.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;

        public TransactionService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IAuditLogRepository auditLogRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Transaction> DepositAsync(DepositWithdrawDto dto, Guid userId)
        {
            var account = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (account == null || account.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this account.");
            account.Balance += dto.Amount;

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = TransactionType.Deposit,
                Description = $"Deposit of {dto.Amount}"
            };

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.SaveAsync();

            await _auditLogRepository.LogAsync(new AuditLog
            {
                Action = "Deposit",
                Details = $"User {userId} deposited {dto.Amount} to account {dto.AccountId}",
                PerformedAt = DateTime.UtcNow,
            });

            return transaction;
        }

        public async Task<Transaction> WithdrawAsync(DepositWithdrawDto dto, Guid userId)
        {
            var account = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (account == null || account.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this account.");
            if (dto.Amount > account.Balance) throw new InvalidOperationException("Insufficient Funds");
            account.Balance -= dto.Amount;

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = TransactionType.Withdrawal,
                Description = $"Withdrawal of {dto.Amount}"
            };

            await _transactionRepository.AddAsync(transaction);
            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.SaveAsync();

            await _auditLogRepository.LogAsync(new AuditLog
            {
                Action = "Withdrawal",
                Details = $"User {userId} withdrew {dto.Amount} from account {dto.AccountId}",
                PerformedAt = DateTime.UtcNow,
            });

            return transaction;
        }

        public async Task TransferAsync(TransferDto dto, Guid userId)
        {
            var fromAccount = await _accountRepository.GetByIdAsync(dto.FromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(dto.ToAccountId);
            if (fromAccount == null || fromAccount.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this account.");
            if (toAccount == null) throw new InvalidOperationException("To Account Not Found");
            if (dto.Amount > fromAccount.Balance) throw new InvalidOperationException("Insufficient Funds");

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
                    Description = $"Transfer to account {dto.ToAccountId}"
                };

                var creditTransaction = new Transaction
                {
                    AccountId = dto.ToAccountId,
                    Amount = dto.Amount,
                    Type = TransactionType.Deposit,
                    Description = $"Transfer from account {dto.FromAccountId}"
                };

                await _transactionRepository.AddAsync(debitTransaction);
                await _transactionRepository.AddAsync(creditTransaction);

                await _unitOfWork.CommitAsync();

                await _auditLogRepository.LogAsync(new AuditLog
                {
                    Action = "Transfer",
                    Details = $"User {userId} transferred {dto.Amount} from account {dto.FromAccountId} to account {dto.ToAccountId}",
                    PerformedAt = DateTime.UtcNow,
                });
            }

            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResultDto<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId, Guid userId, int page, int pageSize)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this account.");

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