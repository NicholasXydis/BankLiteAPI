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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IValidator<CreateAccountDto> _accountValidator;

        public AccountController(IAccountService accountService, IValidator<CreateAccountDto> accountValidator)
        {
            _accountService = accountService;
            _accountValidator = accountValidator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var validation = await _accountValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _accountService.CreateAccountAsync(dto, userId);
            return CreatedAtAction(nameof(GetAccounts), result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _accountService.GetAccountsByUserIdAsync(userId);
            return Ok(result);
        }
    }
}
