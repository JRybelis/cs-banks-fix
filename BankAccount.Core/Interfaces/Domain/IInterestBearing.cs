namespace BankAccount.Core.Interfaces.Domain;

public interface IInterestBearing
{
    decimal InterestRate { get; }
    Task<ITransaction> CalculateAndApplyInterestAsync(CancellationToken cancellationToken = default);
}