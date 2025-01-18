
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OOPExample
{
    // Abstract base class to define common behavior for different types of accounts
    public abstract class BankAccount
    {
        public string AccountHolder { get; set; }
        public decimal Balance { get; protected set; }

        // Constructor to initialize account holder's name
        public BankAccount(string accountHolder)
        {
            AccountHolder = accountHolder;
            Balance = 0m; // Initially, balance is zero
        }

        // Abstract method to deposit money, needs to be implemented by derived classes
        public abstract Task DepositAsync(decimal amount);

        // Abstract method to withdraw money, needs to be implemented by derived classes
        public abstract Task WithdrawAsync(decimal amount);

        // Method to display account details (common functionality for all accounts)
        public void DisplayAccountInfo()
        {
            Console.WriteLine($"Account Holder: {AccountHolder}, Balance: {Balance:C}");
        }
    }

    // Derived class for a Checking account
    public class CheckingAccount : BankAccount
    {
        public decimal OverdraftLimit { get; private set; }

        // Constructor for CheckingAccount, initializes overdraft limit
        public CheckingAccount(string accountHolder, decimal overdraftLimit)
            : base(accountHolder)
        {
            OverdraftLimit = overdraftLimit;
        }

        // Implement DepositAsync for CheckingAccount
        public override async Task DepositAsync(decimal amount)
        {
            await Task.Delay(50); // Simulate delay for async operation
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C} into Checking Account.");
        }

        // Implement WithdrawAsync for CheckingAccount with overdraft handling
        public override async Task WithdrawAsync(decimal amount)
        {
            await Task.Delay(50); // Simulate delay for async operation
            if (Balance - amount >= -OverdraftLimit)
            {
                Balance -= amount;
                Console.WriteLine($"Withdrew {amount:C} from Checking Account.");
            }
            else
            {
                Console.WriteLine($"Withdrawal denied. Insufficient funds for this withdrawal.");
            }
        }
    }

    // Derived class for a Savings account
    public class SavingsAccount : BankAccount
    {
        public decimal InterestRate { get; private set; }

        // Constructor for SavingsAccount, initializes interest rate
        public SavingsAccount(string accountHolder, decimal interestRate)
            : base(accountHolder)
        {
            InterestRate = interestRate;
        }

        // Implement DepositAsync for SavingsAccount
        public override async Task DepositAsync(decimal amount)
        {
            await Task.Delay(50); // Simulate delay for async operation
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C} into Savings Account.");
        }

        // Implement WithdrawAsync for SavingsAccount
        public override async Task WithdrawAsync(decimal amount)
        {
            await Task.Delay(50); // Simulate delay for async operation
            if (Balance >= amount)
            {
                Balance -= amount;
                Console.WriteLine($"Withdrew {amount:C} from Savings Account.");
            }
            else
            {
                Console.WriteLine($"Withdrawal denied. Insufficient funds in Savings Account.");
            }
        }

        // Method to calculate interest for the Savings account
        public void CalculateInterest()
        {
            decimal interest = Balance * InterestRate / 100;
            Balance += interest;
            Console.WriteLine($"Interest of {interest:C} added to Savings Account.");
        }
    }

    // Main class to demonstrate OOP concepts and polymorphism
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create instances of CheckingAccount and SavingsAccount
            BankAccount checking = new CheckingAccount("Alice", 500m); // Alice's Checking Account with $500 overdraft
            BankAccount savings = new SavingsAccount("Bob", 5m); // Bob's Savings Account with 5% interest rate

            // Demonstrate polymorphism (both checking and savings accounts use DisplayAccountInfo method)
            checking.DisplayAccountInfo();
            savings.DisplayAccountInfo();

            // Perform asynchronous deposit and withdrawal operations
            var depositTasks = new List<Task>
            {
                checking.DepositAsync(200m), // Deposit into checking account
                savings.DepositAsync(300m)  // Deposit into savings account
            };

            var withdrawTasks = new List<Task>
            {
                checking.WithdrawAsync(50m), // Withdraw from checking account
                savings.WithdrawAsync(50m)   // Withdraw from savings account
            };

            await Task.WhenAll(depositTasks);
            await Task.WhenAll(withdrawTasks);

            // Demonstrate interest calculation for savings account
            SavingsAccount savingsAccount = savings as SavingsAccount; // Cast to access SavingsAccount specific features
            if (savingsAccount != null)
            {
                savingsAccount.CalculateInterest(); // Calculate interest for savings account
            }

            // Display final account information
            checking.DisplayAccountInfo();
            savings.DisplayAccountInfo();
        }
    }
}


