using Moq;
using Xunit;
using BankLite.Application.Services;
using BankLite.Application.DTOs;
using BankLite.Domain.Interfaces;
using BankLite.Domain.Entities;
using Microsoft.Extensions.Configuration;

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
    }
}
