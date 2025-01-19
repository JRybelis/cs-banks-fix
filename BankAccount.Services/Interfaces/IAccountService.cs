using BankAccount.Core.Dto.Operations;
using BankAccount.Core.Dto.Views;

namespace BankAccount.Services.Interfaces;

public interface IAccountService
{
    Task<AccountBalanceDto> GetBalanceAsync(Guid accountId);
    Task<TransactionDto> DepositAsync(Guid accountId, decimal amount);
    Task<TransactionDto> WithdrawAsync(Guid accountId, decimal amount);
    Task<IEnumerable<TransactionHistoryItemDto>> GetTransactionHistoryAsync(Guid accountId);
    Task<AccountSummaryDto> GetAccountSummaryAsync(Guid accountId);
}