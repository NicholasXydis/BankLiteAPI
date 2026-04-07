using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankLite.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;   

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task <IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _accountService.CreateAccountAsync(dto, userId);
            return Ok(result);
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
