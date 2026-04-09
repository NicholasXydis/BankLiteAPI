using BankLite.Application.DTOs;
using BankLite.Application.Interfaces;
using BankLite.Application.Services;
using BankLite.Domain.Entities;
using BankLite.Domain.Interfaces;
using Moq;
using System.Security.Principal;
using Xunit;

namespace BankLite.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<ITransactionRepository> _mockTransactionRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuditLogRepository> _mockAudiLogRepo;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAudiLogRepo = new Mock<IAuditLogRepository>();

            _transactionService = new TransactionService(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUnitOfWork.Object, _mockAudiLogRepo.Object);
        }

        [Fact]
        public async Task DepositAsync_ShouldIncreaseBalance()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 1000,
                AccountNumber = "ACC001",
                Type = AccountType.Chequing
            };

            _mockAccountRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(account);

            var dto = new DepositWithdrawDto
            {
                AccountId = account.Id,
                Amount = 250
            };

            await _transactionService.DepositAsync(dto);

            Assert.Equal(1250, account.Balance);
        }

        [Fact]
        public async Task WithdrawAsync_ShouldThrow_WhenInsufficientFunds()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 1000,
                AccountNumber = "ACC001",
                Type = AccountType.Chequing
            };

            _mockAccountRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(account);

            var dto = new DepositWithdrawDto
            {
                AccountId = account.Id,
                Amount = 2000
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _transactionService.WithdrawAsync(dto));
        }

        [Fact]
        public async Task TransferAsync_ShouldMoveMoney_BetweenAccounts()
        {
            var fromAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 1000,
                AccountNumber = "ACC001",
                Type = AccountType.Chequing
            };

            var toAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 1000,
                AccountNumber = "ACC002",
                Type = AccountType.Chequing
            };

            _mockAccountRepo
                .Setup(r => r.GetByIdAsync(fromAccount.Id))
                .ReturnsAsync(fromAccount);

            _mockAccountRepo
                .Setup(r => r.GetByIdAsync(toAccount.Id))
                .ReturnsAsync(toAccount);

            var dto = new TransferDto
            {
               FromAccountId = fromAccount.Id,
               ToAccountId = toAccount.Id,
               Amount = 500
            };

            await _transactionService.TransferAsync(dto);

            Assert.Equal(500, fromAccount.Balance);
            Assert.Equal(1500, toAccount.Balance);
        }
    }
}
