using System.Collections.Concurrent;
using BankAccount.Core.Dto.Operations;
using BankAccount.Core.Dto.Views;
using BankAccount.Core.Interfaces.Domain;
using BankAccount.Services.Interfaces;
using BankAccount.Services.Mappings;

namespace BankAccount.Services.Services;

public class AccountService : IAccountService
{
    private readonly ConcurrentDictionary<Guid, IAccount> _accounts = new();

    public async Task<AccountBalanceDto> GetBalanceAsync(Guid accountId)
    {
        var account = GetAccountOrThrow(accountId);
        
        return new AccountBalanceDto(
            accountId,
            account.Balance,
            account.Balance, // Could be different for checking account with overdraft
            DateTime.UtcNow);
    }

    public async Task<TransactionDto> DepositAsync(Guid accountId, decimal amount)
    {
        var account = GetAccountOrThrow(accountId);
        var transaction = await account.DepositAsync(amount);
        return transaction.ToDto(accountId);
    }

    public async Task<TransactionDto> WithdrawAsync(Guid accountId, decimal amount)
    {
        var account = GetAccountOrThrow(accountId);
        var transaction = await account.WithdrawAsync(amount);
        return transaction.ToDto(accountId);
    }

    public async Task<IEnumerable<TransactionHistoryItemDto>> GetTransactionHistoryAsync(Guid accountId)
    {
        return await Task.Run(() =>
        {
            var account = GetAccountOrThrow(accountId);
            decimal runningBalance = 0;

            return account.GetTransactionHistory()
                .OrderBy(t => t.Timestamp)
                .Select(t =>
                {
                    runningBalance += t.Amount;
                    return new TransactionHistoryItemDto(
                        t.Timestamp,
                        t.Description,
                        t.Amount,
                        runningBalance);
                })
                .ToList();
        });
    }

    public async Task<AccountSummaryDto> GetAccountSummaryAsync(Guid accountId)
    {
        return await Task.Run(() =>
        {
            var account = GetAccountOrThrow(accountId);
            return account.ToSummaryDto();
        });
    }

    private IAccount GetAccountOrThrow(Guid accountId)
    {
        if (!_accounts.TryGetValue(accountId, out var account))
            throw new KeyNotFoundException($"Account {accountId} not found");
        
        return account;
    }
}