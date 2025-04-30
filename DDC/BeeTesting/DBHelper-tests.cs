using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServerBee.Data;
using ServerBee.Data.Models;
using ServerBee.Services;
using Xunit;

namespace BeeTesting
{
    public class DbHelperServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly DbHelperService _service;
        private readonly ApplicationUser _testUser;

        public DbHelperServiceTests()
        {
            // Set up the mock user manager
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Set up the test user
            _testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                CustomerId = 1
            };

            // Set up HTTP context with claims principal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _testUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, _testUser.Id)
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(claimsPrincipal);

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);

            // Set up mock user manager to return our test user
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);

            // Set up in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Create the service
            _service = new DbHelperService(_mockUserManager.Object, _mockHttpContextAccessor.Object, _dbContext);
        }

        [Fact]
        public async Task CreateAccountAsync_Test() //Creates New Account With Correct Details
        {
            // Arrange
            string accountType = "Checking";
            decimal openingBalance = 1000.00m;

            // Act
            string accountNumber = await _service.CreateAccountAsync(accountType, openingBalance);

            // Assert
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            Assert.NotNull(account);
            Assert.Equal(accountType, account.AccountType);
            Assert.Equal(openingBalance, account.OpeningBalance);
            Assert.Equal(openingBalance, account.CurrentBalance);
            Assert.Equal(_testUser.CustomerId, account.CustomerId);
        }

        [Fact]
        public async Task GetMoneyAccountsAsync_Test() // Returns Only User Accounts
        {
            // Arrange
            // Create accounts for the test user
            var userAccount1 = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            var userAccount2 = new Account
            {
                AccountId = 2,
                AccountNumber = "DDC1000002",
                CustomerId = _testUser.CustomerId,
                AccountType = "Savings",
                CurrentBalance = 1000.00m,
                OpeningBalance = 1000.00m
            };
            
            // Create an account for a different user
            var otherAccount = new Account
            {
                AccountId = 3,
                AccountNumber = "DDC1000003",
                CustomerId = 6, // Different user ID
                AccountType = "Checking",
                CurrentBalance = 750.00m,
                OpeningBalance = 750.00m
            };
            
            _dbContext.Accounts.AddRange(userAccount1, userAccount2, otherAccount);
            await _dbContext.SaveChangesAsync();

            // Act
            var accounts = await _service.GetMoneyAccountsAsync();

            // Assert
            Assert.Equal(2, accounts.Count);
            Assert.Contains(accounts, a => a.AccountId == userAccount1.AccountId);
            Assert.Contains(accounts, a => a.AccountId == userAccount2.AccountId);
            Assert.DoesNotContain(accounts, a => a.AccountId == otherAccount.AccountId);
        }

        [Fact]
        public async Task DepositAsync_Test() // Increases Account Balance And Creates Transaction
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            decimal depositAmount = 250.00m;

            // Act
            await _service.DepositAsync(account.AccountNumber, depositAmount);

            // Assert
            var updatedAccount = await _dbContext.Accounts.FindAsync(account.AccountId);
            Assert.Equal(750.00m, updatedAccount.CurrentBalance);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
            Assert.NotNull(transaction);
            Assert.Equal(depositAmount, transaction.Amount);
            Assert.Equal("Deposit", transaction.TransactionType);
            Assert.Equal(_testUser.UserName, transaction.PayeePayerName);
            Assert.Equal(account.AccountNumber, transaction.PayeePayerAccountNumber);
        }

        [Fact]
        public async Task WithdrawAsync_Test() // Decreases Account Balance And Creates Transaction
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            decimal withdrawAmount = 200.00m;

            // Act
            await _service.WithdrawAsync(account.AccountNumber, withdrawAmount);

            // Assert
            var updatedAccount = await _dbContext.Accounts.FindAsync(account.AccountId);
            Assert.Equal(300.00m, updatedAccount.CurrentBalance);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
            Assert.NotNull(transaction);
            Assert.Equal(withdrawAmount, transaction.Amount);
            Assert.Equal("Withdraw", transaction.TransactionType);
            Assert.Equal(_testUser.UserName, transaction.PayeePayerName);
            Assert.Equal(account.AccountNumber, transaction.PayeePayerAccountNumber);
        }

        [Fact]
        public async Task TransferInternalAsync_Test() // Moves Money Between Accounts And Creates Transactions
        {
            // Arrange
            var sourceAccount = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            
            var targetAccount = new Account
            {
                AccountId = 2,
                AccountNumber = "DDC1000002",
                CustomerId = _testUser.CustomerId,
                AccountType = "Savings",
                CurrentBalance = 1000.00m,
                OpeningBalance = 1000.00m
            };
            
            _dbContext.Accounts.AddRange(sourceAccount, targetAccount);
            await _dbContext.SaveChangesAsync();

            decimal transferAmount = 300.00m;

            // Act
            await _service.TransferInternalAsync(sourceAccount.AccountNumber, targetAccount.AccountNumber, transferAmount);

            // Assert
            var updatedSourceAccount = await _dbContext.Accounts.FindAsync(sourceAccount.AccountId);
            var updatedTargetAccount = await _dbContext.Accounts.FindAsync(targetAccount.AccountId);
            
            Assert.Equal(200.00m, updatedSourceAccount.CurrentBalance);
            Assert.Equal(1300.00m, updatedTargetAccount.CurrentBalance);

            var transactions = await _dbContext.Transactions.ToListAsync();
            Assert.Equal(2, transactions.Count);
            
            Assert.Contains(transactions, t => 
                t.AccountId == sourceAccount.AccountId && 
                t.Amount == transferAmount && 
                t.TransactionType == "Withdraw");
                
            Assert.Contains(transactions, t => 
                t.AccountId == targetAccount.AccountId && 
                t.Amount == transferAmount && 
                t.TransactionType == "Deposit");
        }

        [Fact]
        public async Task RemoveAccountAsync_SuccessTest() // Deletes Account When Owner Is Current User
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            // Act
            bool result = await _service.RemoveAccountAsync(account.AccountNumber);

            // Assert
            Assert.True(result);
            var accounts = await _dbContext.Accounts.ToListAsync();
            Assert.Empty(accounts);
        }

        [Fact]
        public async Task RemoveAccountAsync_FalseTest() // Returns False When Account Not Found
        {
            // Act
            bool result = await _service.RemoveAccountAsync("NONEXISTENT");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTransactionHistoryAsync_Test() // Returns Paginated Transactions For User Accounts
        {
            // Arrange
            var account1 = new Account
            {
                AccountId = 1,
                AccountNumber = "DDC1000001",
                CustomerId = _testUser.CustomerId,
                AccountType = "Checking",
                CurrentBalance = 500.00m,
                OpeningBalance = 500.00m
            };
            
            var account2 = new Account
            {
                AccountId = 2,
                AccountNumber = "DDC1000002",
                CustomerId = _testUser.CustomerId,
                AccountType = "Savings",
                CurrentBalance = 1000.00m,
                OpeningBalance = 1000.00m
            };
            
            _dbContext.Accounts.AddRange(account1, account2);
            
            // Create transactions for each account
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    TransactionId = 1,
                    AccountId = account1.AccountId,
                    Amount = 100.00m,
                    Date = new DateOnly(2025, 4, 20),
                    Time = new TimeOnly(9, 0),
                    TransactionType = "Deposit",
                    PayeePayerName = "Test User",
                    PayeePayerAccountNumber = "EXT123"
                },
                new Transaction
                {
                    TransactionId = 2,
                    AccountId = account1.AccountId,
                    Amount = 50.00m,
                    Date = new DateOnly(2025, 4, 21),
                    Time = new TimeOnly(10, 0),
                    TransactionType = "Withdraw",
                    PayeePayerName = "Test User",
                    PayeePayerAccountNumber = "EXT456"
                },
                new Transaction
                {
                    TransactionId = 3,
                    AccountId = account2.AccountId,
                    Amount = 200.00m,
                    Date = new DateOnly(2025, 4, 22),
                    Time = new TimeOnly(11, 0),
                    TransactionType = "Deposit",
                    PayeePayerName = "Test User",
                    PayeePayerAccountNumber = "EXT789"
                },
                new Transaction
                {
                    TransactionId = 4,
                    AccountId = 6, // Different account not owned by user
                    Amount = 300.00m,
                    Date = new DateOnly(2025, 4, 23),
                    Time = new TimeOnly(12, 0),
                    TransactionType = "Deposit",
                    PayeePayerName = "Other User",
                    PayeePayerAccountNumber = "EXT999"
                }
            };
            
            _dbContext.Transactions.AddRange(transactions);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetTransactionHistoryAsync(2, 0); // First page, 2 items per page

            // Assert
            Assert.Equal(2, result.Count);
            
            // Should be ordered by newest first
            Assert.Equal(3, result[0].TransactionId); // Newest transaction for user accounts
            Assert.Equal(2, result[1].TransactionId); // Second newest
            
            // Second page
            var page2 = await _service.GetTransactionHistoryAsync(2, 1);
            Assert.Single(page2);
            Assert.Equal(1, page2[0].TransactionId); // Oldest transaction
            
            // Transaction from account not owned by user should not be included
            Assert.DoesNotContain(result, t => t.TransactionId == 4);
            Assert.DoesNotContain(page2, t => t.TransactionId == 4);
        }
    }
}