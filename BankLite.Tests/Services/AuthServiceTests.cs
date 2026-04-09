using Moq;
using Xunit;
using BankLite.Application.Services;
using BankLite.Application.DTOs;
using BankLite.Domain.Interfaces;
using BankLite.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace BankLite.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IAuditLogRepository> _mockAuditRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockAuditRepo = new Mock<IAuditLogRepository>();
            _mockConfig = new Mock<IConfiguration>();

            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["Secret"]).Returns("supersecretkey12345678901234567890");
            jwtSection.Setup(x => x["Issuer"]).Returns("BankLiteAPI");
            jwtSection.Setup(x => x["Audience"]).Returns("BankLiteClient");
            _mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

            _authService = new AuthService(_mockUserRepo.Object, _mockConfig.Object, _mockAuditRepo.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            _mockUserRepo
                .Setup(r => r.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var dto = new RegisterUserDto
            {
                FullName = "New User",
                Email = "test@banklite.com",
                Password = "Password123"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            _mockUserRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

            var dto = new LoginUserDto
            {
                Email = "test@banklite.com",
                Password = "Password123"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordIsWrong()
        {
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@banklite.com",
                FullName = "Existing User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            };

            _mockUserRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

            var dto = new LoginUserDto
            {
                Email = "test@banklite.com",
                Password = "WrongPassword"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.LoginAsync(dto));
        }
    }
}
