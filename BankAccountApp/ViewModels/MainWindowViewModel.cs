﻿using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using BankAccount.Core.Domain;
using BankAccount.Core.Interfaces.Domain;
using BankAccount.Core.Interfaces.Events;
using ReactiveUI;

namespace BankAccountApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAccount _checkingAccount;
    private readonly IAccount _savingsAccount;
    private string _amount = string.Empty;
    private string _checkingBalance = string.Empty;
    private string _savingsBalance = string.Empty;
    private decimal _savingsAccountCurrentBalance;
    
    public decimal SavingsAccountCurrentBalance
    {
        get
        {
            var balance = _savingsAccount.Balance;
            return balance;
        }
        private set
        {
            this.RaiseAndSetIfChanged(ref _savingsAccountCurrentBalance, value);
        }
    }

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
        _checkingAccount = checkingAccount ?? throw new ArgumentNullException(nameof(checkingAccount));
        _savingsAccount = savingsAccount ?? throw new ArgumentNullException(nameof(savingsAccount));

        // Subscribe to balance change events
        if (_checkingAccount is IAccountEventPublisher checkingEvents) checkingEvents.BalanceChanged += OnAccountBalanceChanged;
        if (_savingsAccount is IAccountEventPublisher savingsEvents) savingsEvents.BalanceChanged += OnAccountBalanceChanged;

        _ = UpdateAccountInfoAsync();
        
        this.WhenAnyValue(x => x.SavingsAccountCurrentBalance)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(SavingsAccountCurrentBalance)));

        // Validate amount input with explicit threading
        var canExecuteTransaction = this.WhenAnyValue(
            vm => vm.Amount,
            amount => !string.IsNullOrWhiteSpace(amount) && 
                      decimal.TryParse(amount, out var parsedAmount) &&
                      parsedAmount > 0)
            .ObserveOn(RxApp.MainThreadScheduler);

        DepositToCheckingCommand = ReactiveCommand.CreateFromTask(DepositToChecking, canExecuteTransaction);
        WithdrawFromCheckingCommand = ReactiveCommand.CreateFromTask(WithdrawFromChecking, canExecuteTransaction);
        DepositToSavingsCommand = ReactiveCommand.CreateFromTask(DepositToSavings, canExecuteTransaction);
        WithdrawFromSavingsCommand = ReactiveCommand.CreateFromTask(WithdrawFromSavings, canExecuteTransaction);
        CalculateInterestCommand = ReactiveCommand.CreateFromTask(CalculateInterest, 
            this.WhenAnyValue(
                vm => vm.Amount,             
                vm => vm.SavingsAccountCurrentBalance,     
                (amount, balance) => 
                {
                    // Validate amount input
                    var isAmountInvalid = string.IsNullOrWhiteSpace(amount) || 
                                          !decimal.TryParse(amount, out _);

                    // Return true if balance is positive and amount is empty or invalid
                    return balance > 0 && isAmountInvalid;
                }
            )
        );
    }

    private async Task DepositToChecking()
    {
        await HandleTransaction(amount => _checkingAccount.DepositAsync(amount), 
            "Deposited to Checking account.");
    }

    private async Task WithdrawFromChecking()
    {
        await HandleTransaction(amount => _checkingAccount.WithdrawAsync(amount), 
            "Withdrawn from Checking account.");
    }

    private async Task DepositToSavings()
    {
        await HandleTransaction(amount => _savingsAccount.DepositAsync(amount), 
            "Deposited to Savings account.");
    }

    private async Task WithdrawFromSavings()
    {
        await HandleTransaction(amount => _savingsAccount.WithdrawAsync(amount), 
            "Withdrawn from Savings account.");
    }
    
    private async Task CalculateInterest()
    {
        if (_savingsAccount is not SavingsAccount savings)
            return;
        
        await HandleTransaction(
            async () => await savings.CalculateAndApplyInterestAsync(),
            "Interest calculated and applied to savings account.");
    }

    /// <summary>
    /// Handles various types of account transactions with built-in error handling and UI thread synchronization.
    /// </summary>
    /// <param name="transactionAction">The async transaction to perform.
    /// Can be a deposit, withdrawal, or other account operation.</param>
    /// <param name="successMessage">An optional message to log when the transaction succeeds.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleTransaction(Func<Task> transactionAction, string? successMessage = null)
    {
        try
        {
            await transactionAction();

            // If a success message is provided, log it on the UI thread
            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LogTransaction(successMessage);

                    // Clear the amount if it's a transaction that uses the Amount property
                    if (decimal.TryParse(Amount, out _))
                    {
                        Amount = string.Empty;
                    }
                });
            }
        }
        catch (Exception e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LogTransaction($"Error: {e.Message}");
            });
        }
    }
    
    private async Task HandleTransaction(Func<decimal, Task<ITransaction>> operation, string description)
    {
        if (!decimal.TryParse(Amount, out var amount))
            return;
        
        // Use the overloaded HandleTransaction method to wrap the operation
        await HandleTransaction(async () => await operation(amount), $"{description}: {amount:C}");
    }

    private async void OnAccountBalanceChanged(object? sender, EventArgs e)
    {
        await UpdateAccountInfoAsync();
    }

    private async Task UpdateAccountInfoAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            CheckingBalance = $"Checking Account: {_checkingAccount.AccountHolder}, Balance: {_checkingAccount.Balance:C}.";
            SavingsBalance = $"Savings Account: {_savingsAccount.AccountHolder}, Balance: {_savingsAccount.Balance:C}.";
            SavingsAccountCurrentBalance = _savingsAccount.Balance;
        });
    }

    private void LogTransaction(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        TransactionLog.Insert(0, $"[{timestamp}] {message}");
    }
}