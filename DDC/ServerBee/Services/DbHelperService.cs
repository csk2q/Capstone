using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public async Task<bool> TransferInternalAsync(string sourceAccountNumber, string targetAccountNumber, decimal amount)
    {
        // Side-effects: Deduct from source, log transaction, add to target, log transaction
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Deposits funds into a DDC account.
    /// Preconditions: amount > 0, account is owned by current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task<bool> DepositAsync(string accountNumber, decimal amount)
    {
        // Side-effects: Add to account, log deposit transaction
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Withdraws funds from a DDC account.
    /// Preconditions: amount > 0, account is owned by current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task<bool> WithdrawAsync(string accountNumber, decimal amount)
    {
        // Side-effects: Deduct from account, log withdrawal transaction
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Makes an external payment from a DDC account.
    /// Preconditions: account belongs to current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task<bool> PayExternalAsync(string accountNumber, decimal amount, string externalName, string externalAccount)
    {
        // Side-effects: Deduct from account, log withdrawal with external details
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Receives a payment from an external source into a DDC account.
    /// Preconditions: account belongs to current user
    /// Output: true if successful, false otherwise
    /// </summary>
    public async Task<bool> ReceivePaymentExternalAsync(string accountNumber, decimal amount, string externalName, string externalAccount)
    {
        // Side-effects: Add to account, log deposit with external details
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Creates a new DDC money account for the current user.
    /// Output: The new account number
    /// </summary>
    public async Task<string> CreateAccountAsync(string accountType, decimal openingBalance)
    {
        // Side-effects: Create a new account in the system for the user
        // TODO implement STUB
        return "stub function";
    }

    /// <summary>
    /// Removes a DDC money account owned by the current user.
    /// Preconditions: account belongs to current user
    /// Output: true if account was successfully deleted, false otherwise
    /// </summary>
    public async Task<bool> RemoveAccountAsync(string accountNumber)
    {
        // Side-effects: Remove account from system
        // TODO implement STUB
        return true;
    }

    /// <summary>
    /// Retrieves all money accounts owned by the current user.
    /// Output: List of money accounts owned by the user
    /// </summary>
    public async Task<List<Account>> GetMoneyAccountsAsync()
    {
        // TODO implement STUB
        return [];
    }

    /// <summary>
    /// Retrieves the transaction history for the current user.
    /// Output: Paginated, newest-to-oldest transactions for current user
    /// </summary>
    public async Task<List<Transaction>> GetTransactionHistoryAsync(int limit, int pageNumber)
    {
            var user = await GetCurrentUserAsync();

            return await dbContext.Transactions
                .Where(t => t.AccountId == user.CustomerId)
                .OrderByDescending(t => t.Date)
                .ThenBy(t => t.Time)
                .Skip(pageNumber * limit)
                .Take(limit)
                .ToListAsync();
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        ApplicationUser? user = null;
        if (principal is not null)
             user = await userManager.GetUserAsync(principal);
        if (user is null || principal is null)
            throw new ArgumentNullException("Function called while user is not sigined in. Ensure the user is authenticated/signed-in before calling.");
        return user;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously