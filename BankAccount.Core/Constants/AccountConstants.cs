namespace BankAccount.Core.Constants;

public static class AccountConstants
{
    public static class TransactionTypes
    {
        public const string Deposit = "Deposit";
        public const string Withdrawal = "Withdrawal";
        public const string Interest  = "Interest";
    }

    public static class Limits
    {
        public const decimal MinDeposit = 0.01m;
        public const decimal MaxDeposit = 1000000m;
        public const decimal MinBalance = -10000m;
        public const decimal MaxBalance = 1000000000m;
        public const decimal DefaultOverdraftLimit = 500m;
        public const decimal DefaultInterestRate = 2.5m;
    }

    public static class ValidationMessages
    {
        public const string InvalidAmount = "Amount must be greater than zero.";
        public const string ExceedsDepositLimit = "Deposit limit exceeded.";
        public const string ExceedsWithdrawalLimit = "Withdrawal limit exceeded.";
        public const string InsufficientFunds = "Insufficient funds.";
    }

    public static class LockTimeouts
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(90);
        public static readonly TimeSpan ExtendedTimeout = TimeSpan.FromMinutes(5);
    }
}