namespace BankAccount.Core.Interfaces;

public interface IInterestBearing
{
    decimal InterestRate { get; }
    Task<ITransaction> CalculateAndApplyInterestAsync(CancellationToken cancellationToken = default);
}