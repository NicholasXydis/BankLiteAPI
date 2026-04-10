using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BankLite.API.Controllers
{
    [EnableRateLimiting("fixed")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IValidator<DepositWithdrawDto> _depositWithdrawValidator;
        private readonly IValidator<TransferDto> _transferValidator;

        public TransactionController(ITransactionService transactionService, IValidator<DepositWithdrawDto> depositwithdrawValidator, IValidator<TransferDto> transferValidator)
        {
            _transactionService = transactionService;
            _depositWithdrawValidator = depositwithdrawValidator;
            _transferValidator = transferValidator;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositWithdrawDto dto)
        {
            var validation = await _depositWithdrawValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _transactionService.DepositAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] DepositWithdrawDto dto)
        {
            var validation = await _depositWithdrawValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _transactionService.WithdrawAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var validation = await _transferValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _transactionService.TransferAsync(dto, userId);
            return Ok(new { message = "Transfer successful", amount = dto.Amount });
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetTransactions(Guid accountId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId, userId, page, pageSize);
            return Ok(result);
        }
    }
}
