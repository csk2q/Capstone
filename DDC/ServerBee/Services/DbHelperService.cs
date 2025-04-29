using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerBee.Data;
using ServerBee.Data.Models;

// TODO remove this warning once all methods are implemented (also remove restore line at the end of the file)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously


namespace ServerBee.Services;

/// <summary>
/// Service for managing DDC money accounts and transactions.
/// </summary>
public class DbHelperService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApplicationDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbHelperService"/> class.
    /// </summary>
    public DbHelperService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        this.userManager = userManager;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Transfers funds internally between DDC accounts.
    /// Preconditions: amount > 0, source account is owned by current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task TransferInternalAsync(string sourceAccountNumber, string targetAccountNumber, decimal amount)
    {
        // Side-effects: Deduct from source, log withdraw, add to target, log deposit
        await Task.WhenAll([
            WithdrawAsync(sourceAccountNumber, amount),
            DepositAsync(targetAccountNumber, amount)
        ]);
    }

    /// <summary>
    /// Deposits funds into a DDC account.
    /// Preconditions: amount > 0, account is owned by current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task DepositAsync(string accountNumber, decimal amount)
    {
        // Side-effects: Add to account, log deposit transaction
        var currentUser = await GetCurrentUserAsync();
        var account = await GetMoneyAccountAsync(accountNumber, currentUser);
        
        // Add to account
        account.CurrentBalance += amount;
        dbContext.Update(account);
        
        // Log deposit
        var transaction =  await CreateTransactionAsync(account.AccountId, amount, "Deposit", currentUser.UserName, accountNumber);
        dbContext.Transactions.Add(transaction);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Withdraws funds from a DDC account.
    /// Preconditions: amount > 0, account is owned by current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task WithdrawAsync(string accountNumber, decimal amount)
    {
        // Side-effects: Deduct from account, log withdrawal transaction
        var currentUser = await GetCurrentUserAsync();
        var account = await GetMoneyAccountAsync(accountNumber, currentUser);

        // Deduct from account
        account.CurrentBalance -= amount;
        dbContext.Update(account);

        // Log withdraw
        var withdrawTransaction = await CreateTransactionAsync(account.AccountId, amount, "Withdraw", currentUser.UserName, accountNumber);
        dbContext.Transactions.Add(withdrawTransaction);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Makes an external payment from a DDC account.
    /// Preconditions: account belongs to current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task PayExternalAsync(string accountNumber, decimal amount, string externalName, string externalAccount)
    {
        // Side-effects: Deduct from account, log withdrawal with external details
        var currentUser = await GetCurrentUserAsync();
        var account = await GetMoneyAccountAsync(accountNumber, currentUser);

        // Add to account
        account.CurrentBalance += amount;
        dbContext.Update(account);
        
        // Log transaction
        var transaction = await CreateTransactionAsync(account.AccountId, amount, "Transfer from DDC", externalName, externalAccount);
        dbContext.Transactions.Add(transaction);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Receives a payment from an external source into a DDC account.
    /// Preconditions: account belongs to current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task ReceivePaymentExternalAsync(string accountNumber, decimal amount, string externalName, string externalAccount)
    {
        // Side-effects: Add to account, log deposit with external details
        var currentUser = await GetCurrentUserAsync();
        var account = await GetMoneyAccountAsync(accountNumber, currentUser);

        // Add to account
        account.CurrentBalance += amount;
        dbContext.Update(account);
        
        // Log transaction
        var transaction = await CreateTransactionAsync(account.AccountId, amount, "Transfer from DDC", externalName, externalAccount);
        dbContext.Transactions.Add(transaction);
        
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new DDC money account for the current user.
    /// Output: The new account number
    /// </summary>
    public async Task<string> CreateAccountAsync(string accountType, decimal openingBalance)
    {
        // Side-effects: Create a new account in the system for the user
        var currentUser = await GetCurrentUserAsync();
        
        // Get new account id
        int accountId = 0;
        if (!dbContext.Accounts.IsNullOrEmpty())
            accountId = await dbContext.Accounts.Select(t => t.AccountId).MaxAsync() + 1;
        string accountNumber = "DDC" + (1000000 + accountId);
        
        var account = new Account()
        {
            CustomerId = currentUser.CustomerId,
            AccountId = accountId,
            AccountType = accountType,
            AccountNumber = accountNumber,
            CurrentBalance = openingBalance,
            OpeningBalance = openingBalance
        };
        dbContext.Accounts.Add(account);

        await dbContext.SaveChangesAsync();
        return accountNumber;
    }

    /// <summary>
    /// Removes a DDC money account owned by the current user.
    /// Preconditions: account belongs to current user
    /// Output: true if account was successfully deleted, false otherwise
    /// </summary>
    public async Task<bool> RemoveAccountAsync(string accountNumber)
    {
        // Side-effects: Remove account from system
        var currentUser = await GetCurrentUserAsync();

        var matchingAccounts = await dbContext.Accounts.Where(
            a => a.CustomerId == currentUser.CustomerId && a.AccountNumber == accountNumber).ToListAsync();
        
        if (matchingAccounts.IsNullOrEmpty())
            return false;
        
        dbContext.Accounts.RemoveRange(matchingAccounts);
        await dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves all money accounts owned by the current user.
    /// Output: List of money accounts owned by the user
    /// </summary>
    public async Task<List<Account>> GetMoneyAccountsAsync()
    {
        var currentUser = await GetCurrentUserAsync();
        return await dbContext.Accounts.Where(t => t.CustomerId == currentUser.CustomerId).ToListAsync();
    }

    /// <summary>
/// Retrieves recent transactions for a specific account owned by the current user.
/// </summary>
public async Task<List<Transaction>> GetRecentTransactionsForAccountAsync(string accountNumber, int limit = 5)
{
    var currentUser = await GetCurrentUserAsync();
    var account = await GetMoneyAccountAsync(accountNumber, currentUser);

    return await dbContext.Transactions
        .Where(t => t.AccountId == account.AccountId)
        .OrderByDescending(t => t.Date)
        .ThenByDescending(t => t.Time)
        .Take(limit)
        .ToListAsync();
}


    /// <summary>
    /// Retrieves the transaction history for the current user.
    /// Output: Paginated, newest-to-oldest transactions for current user
    /// </summary>
    public async Task<List<Transaction>> GetTransactionHistoryAsync(int limit, int pageNumber)
    {
            var user = await GetCurrentUserAsync();
            var accounts = await GetMoneyAccountsAsync();
            if (accounts.Count < 1)
                return [];
            
            var accountIds = accounts.Select(t => t.AccountId).ToList();

            return await dbContext.Transactions
                .Where(t => accounts.Select(a => a.AccountId).Contains<int>((int)t.AccountId!))
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Time)
                .Skip(pageNumber * limit)
                .Take(limit)
                .ToListAsync();
    }
    
    // --- Helper Methods --- //

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        ApplicationUser? user = null;
        if (principal is not null)
             user = await userManager.GetUserAsync(principal);
        if (user is null || principal is null)
            throw new ArgumentNullException("Function called while user is not sigined in. Ensure the user is authenticated/signed-in before calling.");
        return user;
    }

    public async Task<Account> GetMoneyAccountAsync(string accountNumber, ApplicationUser? currentUser = null)
    {
        if (currentUser is null)
            currentUser = await GetCurrentUserAsync();
        var userId = currentUser.CustomerId;

        var accounts = dbContext.Accounts
            .Where(t => t.CustomerId == userId & t.AccountNumber == accountNumber).ToList();
        if (accounts.Count > 1)
            throw new Exception($"Multiple accounts exist with ID '{accountNumber}' !");
        else if (accounts.Count == 0)
            throw new Exception($"No accounts exist with ID '{accountNumber}' !");
        return accounts[0];
    }
    
    private async Task<Transaction> CreateTransactionAsync(int accountId, decimal amount, string TransactionType, string PayeePayerName, string PayeePayerAccountNumber)
    {   
        int transactionId = 0;
        if (!dbContext.Transactions.IsNullOrEmpty())
            transactionId = await dbContext.Transactions.Select(t => t.TransactionId).MaxAsync() + 1;

        return new Transaction
        {
            AccountId = accountId,
            Amount = amount,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
            PayeePayerAccountNumber = PayeePayerAccountNumber,
            PayeePayerName = PayeePayerName,
            TransactionType = TransactionType,
            TransactionId = transactionId
        };
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously