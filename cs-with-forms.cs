

using System;
using System.Windows.Forms;

namespace BankAccountApp
{
    public partial class MainForm : Form
    {
        private BankAccount checking;
        private BankAccount savings;

        public MainForm()
        {
            InitializeComponent();
            checking = new CheckingAccount("Alice", 500m);
            savings = new SavingsAccount("Bob", 5m);

            // Subscribe to BalanceChanged events
            checking.BalanceChanged += AccountBalanceChanged;
            savings.BalanceChanged += AccountBalanceChanged;

            // Initialize account info labels
            UpdateAccountInfo();
        }

        // Event handler for balance change
        private void AccountBalanceChanged(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }

        // Method to update account info labels
        private void UpdateAccountInfo()
        {
            lblCheckingInfo.Text = $"Checking Account: {checking.AccountHolder}, Balance: {checking.Balance:C}";
            lblSavingsInfo.Text = $"Savings Account: {savings.AccountHolder}, Balance: {savings.Balance:C}";
        }

        // Deposit for Checking Account
        private void btnDepositChecking_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtDepositAmount.Text, out decimal amount))
            {
                checking.Deposit(amount);
                txtDepositAmount.Clear();
            }
            else
            {
                MessageBox.Show("Invalid deposit amount!");
            }
        }

        // Withdraw from Checking Account
        private void btnWithdrawChecking_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtWithdrawAmount.Text, out decimal amount))
            {
                checking.Withdraw(amount);
                txtWithdrawAmount.Clear();
            }
            else
            {
                MessageBox.Show("Invalid withdrawal amount!");
            }
        }

        // Deposit for Savings Account
        private void btnDepositSavings_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtDepositAmount.Text, out decimal amount))
            {
                savings.Deposit(amount);
                txtDepositAmount.Clear();
            }
            else
            {
                MessageBox.Show("Invalid deposit amount!");
            }
        }

        // Withdraw from Savings Account
        private void btnWithdrawSavings_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtWithdrawAmount.Text, out decimal amount))
            {
                savings.Withdraw(amount);
                txtWithdrawAmount.Clear();
            }
            else
            {
                MessageBox.Show("Invalid withdrawal amount!");
            }
        }

        // Calculate interest for Savings Account
        private void btnCalculateInterest_Click(object sender, EventArgs e)
        {
            var savingsAccount = savings as SavingsAccount;
            savingsAccount?.CalculateInterest();
        }
    }
}

