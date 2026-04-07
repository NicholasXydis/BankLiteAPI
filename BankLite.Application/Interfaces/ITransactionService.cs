using BankLite.Application.DTOs;
using BankLite.Domain.Entities;

namespace BankLite.Application.Interfaces
{
    public interface ITransactionService
    {
        Task DepositAsync(DepositWithdrawDto dto);
        Task WithdrawAsync(DepositWithdrawDto dto);
        Task TransferAsync(TransferDto dto);
        Task<PagedResultDto<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId,int page, int pageSize);
    }
}
