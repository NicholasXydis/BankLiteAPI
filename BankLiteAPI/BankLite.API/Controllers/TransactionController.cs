using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BankLite.API.Controllers
{
    [EnableRateLimiting("fixed")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IValidator<DepositWithdrawDto> _depositwithdrawValidator;
        private readonly IValidator<TransferDto> _transferValidator;


        public TransactionController(ITransactionService transactionService, IValidator<DepositWithdrawDto> depositwithdrawValidator, IValidator<TransferDto> transferValidator)
        {
            _transactionService = transactionService;
            _depositwithdrawValidator = depositwithdrawValidator;
            _transferValidator = transferValidator;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositWithdrawDto dto)
        {
            var validation = await _depositwithdrawValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            await _transactionService.DepositAsync(dto);
            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] DepositWithdrawDto dto)
        {
            var validation = await _depositwithdrawValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            await _transactionService.WithdrawAsync(dto);
            return Ok();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var validation = await _transferValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

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
