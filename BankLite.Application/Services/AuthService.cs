using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;


namespace BankLite.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            if (await _userRepository.ExistsAsync(dto.Email))
                throw new Exception("Email already registered");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash
            };
            await _userRepository.AddAsync(user);

            var token = GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid Credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid Credentials");

            var token = GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id
            };
        }

        private string GenerateToken(User user)
        {
            var jwtsettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var token = new JwtSecurityToken(
                issuer: jwtsettings["Issuer"],
                audience: jwtsettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtsettings["ExpiryMinutes"]!)),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
