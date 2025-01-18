using BankAccount.Core.Domain;
using BankAccount.Core.Dto.Views;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Services.Mappings;

public static class AccountMappings
{
    public static AccountSummaryDto ToSummaryDto(this IAccount account)
    {
        return new AccountSummaryDto(
            account.AccountId,
            account.AccountHolder,
            account.GetType().Name,
            account.Balance,
            account is CheckingAccount checking ? checking.OverdraftLimit : null,
            account is SavingsAccount savings ? savings.InterestRate : null);
    }
}