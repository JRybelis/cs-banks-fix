using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using BankAccount.Core.Domain;
using BankAccount.Core.Interfaces.Domain;
using BankAccount.Core.Interfaces.Events;
using ReactiveUI;

namespace BankAccountApp.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly IAccount _checkingAccount;
    private readonly IAccount _savingsAccount;
    private string _amount = string.Empty;
    private string _checkingBalance = string.Empty;
    private string _savingsBalance = string.Empty;

    public ObservableCollection<string> TransactionLog { get; } = new();

    public string Amount
    {
        get => _amount;
        set => this.RaiseAndSetIfChanged(ref _amount, value);
    }

    public string CheckingBalance
    {
        get => _checkingBalance;
        private set => this.RaiseAndSetIfChanged(ref _checkingBalance, value);
    }

    public string SavingsBalance
    {
        get => _savingsBalance;
        private set => this.RaiseAndSetIfChanged(ref _savingsBalance, value);
    }

    public ReactiveCommand<Unit, Unit> DepositToCheckingCommand { get; }
    public ReactiveCommand<Unit, Unit> WithdrawFromCheckingCommand { get; }
    public ReactiveCommand<Unit, Unit> DepositToSavingsCommand { get; }
    public ReactiveCommand<Unit, Unit> WithdrawFromSavingsCommand { get; }
    public ReactiveCommand<Unit, Unit> CalculateInterestCommand { get; }

    public MainWindowViewModel(IAccount checkingAccount, IAccount savingsAccount)
    {
        _checkingAccount = checkingAccount;
        _savingsAccount = savingsAccount;

        // Subscribe to balance change events
        if (_checkingAccount is IAccountEventPublisher checkingEvents)
            checkingEvents.BalanceChanged += AccountBalanceChanged;
        if (_savingsAccount is IAccountEventPublisher savingsEvents)
            savingsEvents.BalanceChanged += AccountBalanceChanged;

        // Initialise commands
        var canExecuteTransaction = this.WhenAnyValue(
            x => x.Amount, 
            amount => !string.IsNullOrWhiteSpace(amount) && decimal.TryParse(amount, out _));

        DepositToCheckingCommand = ReactiveCommand.CreateFromTask(DepositToChecking, canExecuteTransaction);
        WithdrawFromCheckingCommand = ReactiveCommand.CreateFromTask(WithdrawFromChecking, canExecuteTransaction);
        DepositToSavingsCommand = ReactiveCommand.CreateFromTask(DepositToSavings, canExecuteTransaction);
        WithdrawFromSavingsCommand = ReactiveCommand.CreateFromTask(WithdrawFromSavings, canExecuteTransaction);
        CalculateInterestCommand = ReactiveCommand.CreateFromTask(CalculateInterest);

        UpdateAccountInfo();
    }

    private async Task DepositToChecking()
    {
        await HandleTransaction(amount => _checkingAccount.DepositAsync(amount), "Deposited to Checking account.");
    }

    private async Task WithdrawFromChecking()
    {
        await HandleTransaction(amount => _checkingAccount.WithdrawAsync(amount), "Withdrawn from Checking account.");
    }

    private async Task DepositToSavings()
    {
        await HandleTransaction(amount => _savingsAccount.DepositAsync(amount), "Deposited to Savings account.");
    }

    private async Task WithdrawFromSavings()
    {
        await HandleTransaction(amount => _savingsAccount.WithdrawAsync(amount), "Withdrawn from Savings account.");
    }
    
    private async Task HandleTransaction(Func<decimal, Task<ITransaction>> operation, string description)
    {
        if (decimal.TryParse(Amount, out var amount))
        {
            try
            {
                await operation(amount);
                LogTransaction($"{description}: {amount:C}.");
                Amount = string.Empty;
            }
            catch (Exception e)
            {
                LogTransaction($"Error: {e.Message}.");
            }
        }
    }

    private async Task CalculateInterest()
    {
        if (_savingsAccount is SavingsAccount savings)
        {
            try
            {
                await savings.CalculateAndApplyInterestAsync();
                LogTransaction("Interest Calculated for Savings Account.");
            }
            catch (Exception e)
            {
                LogTransaction($"Error calculating interest for Savings Account: {e.Message}.");
            }
        }
    }

    private void AccountBalanceChanged(object? sender, EventArgs e)
    {
        UpdateAccountInfo();
    }

    private void UpdateAccountInfo()
    {
        CheckingBalance = $"Checking Account: {_checkingAccount.AccountHolder}, Balance: {_checkingAccount.Balance:C}.";
        SavingsBalance = $"Savings Account: {_savingsAccount.AccountHolder}, Balance: {_savingsAccount.Balance:C}.";
    }

    private void LogTransaction(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        TransactionLog.Insert(0, $"[{timestamp}] {message}");
    }
}