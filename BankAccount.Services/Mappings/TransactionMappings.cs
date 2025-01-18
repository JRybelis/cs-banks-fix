using BankAccount.Core.Dto.Operations;
using BankAccount.Core.Interfaces.Domain;

namespace BankAccount.Services.Mappings;

public static class TransactionMappings
{
    public static TransactionDto ToDto(this ITransaction transaction, Guid accountId)
    {
        return new TransactionDto(
            transaction.TransactionId,
            accountId,
            transaction.Amount,
            transaction.Description,
            transaction.Timestamp);
    }
}