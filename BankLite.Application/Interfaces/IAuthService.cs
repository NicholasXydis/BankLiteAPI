using BankLite.Application.DTOs;
using System.Threading.Tasks;

namespace BankLite.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<AuthResponseDto> LoginAsync(LoginUserDto dto);
    }
}
