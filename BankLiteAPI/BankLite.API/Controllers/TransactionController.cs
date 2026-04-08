using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankLite.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositWithdrawDto dto)
        {
            await _transactionService.DepositAsync(dto);
            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] DepositWithdrawDto dto)
        {
            await _transactionService.WithdrawAsync(dto);
            return Ok();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            await _transactionService.TransferAsync(dto);
            return Ok();
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetTransactions(Guid accountId, [FromQuery] int page = 1 , [FromQuery] int pageSize = 10)
        {
            var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId, page, pageSize);
            return Ok(result);
        }
    }
}
