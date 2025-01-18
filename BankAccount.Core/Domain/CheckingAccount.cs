namespace BankAccount.Core.Domain;

public class CheckingAccount : BankAccountBase
{
    private readonly decimal _overdraftLimit;
    
    public decimal OverdraftLimit => _overdraftLimit;
    
    public CheckingAccount(string accountHolder, decimal overdraftLimit) : base(accountHolder)
    {
        if (overdraftLimit < 0)
            throw new ArgumentException("Overdraft limit cannot be negative", nameof(overdraftLimit));
        
        _overdraftLimit = overdraftLimit;
    }

    protected override async Task<bool> CanWithdrawAsync(decimal amount)
    {
        return await Task.Run(() =>
        {
            lock (BalanceLock)
            {
                return balance - amount >= -_overdraftLimit;
            }
        });
    }
}