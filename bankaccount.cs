using System;
using System.Collections.Generic;

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

        // Virtual method for Deposit, allowing it to be overridden in derived classes
        public virtual void Deposit(decimal amount)
        {

            // Modify the account sum
            Balance += amount;
            Console.WriteLine($"Deposited {amount:C} into Base Bank Account.");
        }

        // Abstract method to withdraw money, needs to be implemented by derived classes
        public abstract void Withdraw(decimal amount);

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

        // Override Deposit method for CheckingAccount
        public override void Deposit(decimal amount)
        {
            // Check the amount first
            if (amount > 0)
            {
                Console.WriteLine($"Depositing {amount:C} into Checking Account.");
                Balance += amount;
            }
            else
            {
                Console.WriteLine("Deposit amount must be positive.");
            }
        }

        public override void Withdraw(decimal amount)
        {
            // Check the amount again
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

        // Override Deposit method
        public override void Deposit(decimal amount)
        {
            base.Deposit(amount);
            Console.WriteLine($"Deposited {amount:C} into Savings Account.");
        }

        public override void Withdraw(decimal amount)
        {
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

        // Method to calculate and add interest
        public void CalculateInterest()
        {
            decimal interest = Balance * InterestRate / 100;
            Balance += interest;
            Console.WriteLine($"Interest of {interest:C} added to Savings Account.");
        }
    }

    // Main class, shows polymorphism
    class Program
    {
        static void Main(string[] args)
        {
            // Create instances of CheckingAccount and SavingsAccount
            BankAccount checking = new CheckingAccount("Alice", 500m);
            BankAccount savings = new SavingsAccount("Bob", 5m);

            // Polymorphism: Both accounts use DisplayAccountInfo method
            checking.DisplayAccountInfo();
            savings.DisplayAccountInfo();

            // Perform deposit and withdrawal operations
            checking.Deposit(200m); // Deposits into checking account
            savings.Deposit(300m); // Deposits into savings account

            checking.Withdraw(50m); // Withdraw from checking account
            savings.Withdraw(50m); // Withdraw from savings account

            // Demonstrate interest calculation for savings account
            SavingsAccount savingsAccount = savings as SavingsAccount;
            if (savingsAccount != null)
            {
                savingsAccount.CalculateInterest();
            }

            // Display final account details
            checking.DisplayAccountInfo(); 
            savings.DisplayAccountInfo();
        }
    }
}
