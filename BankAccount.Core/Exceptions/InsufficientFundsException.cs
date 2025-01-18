namespace BankAccount.Core.Exceptions;

public class InsufficientFundsException(decimal attemptedAmount, decimal currentBalance)
    : Exception($"Insufficient funds for withdrawal of {attemptedAmount}. Current balance is {currentBalance:C}")
{
    public decimal AttemptedAmount { get; } = attemptedAmount;
    public decimal CurrentBalance { get; } = currentBalance;
}