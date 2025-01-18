
namespace BankAccountApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblCheckingInfo;
        private Label lblSavingsInfo;
        private Button btnDepositChecking;
        private Button btnWithdrawChecking;
        private Button btnDepositSavings;
        private Button btnWithdrawSavings;
        private TextBox txtDepositAmount;
        private TextBox txtWithdrawAmount;
        private Button btnCalculateInterest;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCheckingInfo = new Label();
            this.lblSavingsInfo = new Label();
            this.btnDepositChecking = new Button();
            this.btnWithdrawChecking = new Button();
            this.btnDepositSavings = new Button();
            this.btnWithdrawSavings = new Button();
            this.txtDepositAmount = new TextBox();
            this.txtWithdrawAmount = new TextBox();
            this.btnCalculateInterest = new Button();
            this.SuspendLayout();
            // 
            // lblCheckingInfo
            // 
            this.lblCheckingInfo.AutoSize = true;
            this.lblCheckingInfo.Location = new System.Drawing.Point(12, 9);
            this.lblCheckingInfo.Name = "lblCheckingInfo";
            this.lblCheckingInfo.Size = new System.Drawing.Size(199, 13);
            this.lblCheckingInfo.TabIndex = 0;
            this.lblCheckingInfo.Text = "Checking Account: Alice, Balance: $0.00";
            // 
            // lblSavingsInfo
            // 
            this.lblSavingsInfo.AutoSize = true;
            this.lblSavingsInfo.Location = new System.Drawing.Point(12, 40);
            this.lblSavingsInfo.Name = "lblSavingsInfo";
            this.lblSavingsInfo.Size = new System.Drawing.Size(193, 13);
            this.lblSavingsInfo.TabIndex = 1;
            this.lblSavingsInfo.Text = "Savings Account: Bob, Balance: $0.00";
            // 
            // btnDepositChecking
            // 
            this.btnDepositChecking.Location = new System.Drawing.Point(15, 66);
            this.btnDepositChecking.Name = "btnDepositChecking";
            this.btnDepositChecking.Size = new System.Drawing.Size(75, 23);
            this.btnDepositChecking.TabIndex = 2;
            this.btnDepositChecking.Text = "Deposit Checking";
            this.btnDepositChecking.UseVisualStyleBackColor = true;
            this.btnDepositChecking.Click += new EventHandler(this.btnDepositChecking_Click);
            // 
            // btnWithdrawChecking
            // 
            this.btnWithdrawChecking.Location = new System.Drawing.Point(96, 66);
            this.btnWithdrawChecking.Name = "btnWithdrawChecking";
            this.btnWithdrawChecking.Size = new System.Drawing.Size(75, 23);
            this.btnWithdrawChecking.TabIndex = 3;
            this.btnWithdrawChecking.Text = "Withdraw Checking";
            this.btnWithdrawChecking.UseVisualStyleBackColor = true;
            this.btnWithdrawChecking.Click += new EventHandler(this.btnWithdrawChecking_Click);
            // 
            // btnDepositSavings
            // 
            this.btnDepositSavings.Location = new System.Drawing.Point(15, 95);
            this.btnDepositSavings.Name = "btnDepositSavings";
            this.btnDepositSavings.Size = new System.Drawing.Size(75, 23);
            this.btnDepositSavings.TabIndex = 4;
            this.btnDepositSavings.Text = "Deposit Savings";
            this.btnDepositSavings.UseVisualStyleBackColor = true;
            this.btnDepositSavings.Click += new EventHandler(this.btnDepositSavings_Click);
            // 
            // btnWithdrawSavings
            // 
            this.btnWithdrawSavings.Location = new System.Drawing.Point(96, 95);
            this.btnWithdrawSavings.Name = "btnWithdrawSavings";
            this.btnWithdrawSavings.Size = new System.Drawing.Size(75, 23);
            this.btnWithdrawSavings.TabIndex = 5;
            this.btnWithdrawSavings.Text = "Withdraw Savings";
            this.btnWithdrawSavings.UseVisualStyleBackColor = true;
            this.btnWithdrawSavings.Click += new EventHandler(this.btnWithdrawSavings_Click);
            // 
            // txtDepositAmount
            // 
            this.txtDepositAmount.Location = new System.Drawing.Point(15, 124);
            this.txtDepositAmount.Name = "txtDepositAmount";
            this.txtDepositAmount.Size = new System.Drawing.Size(100, 20);
            this.txtDepositAmount.TabIndex = 6;
            this.txtDepositAmount.PlaceholderText = "Amount to Deposit";
            // 
            // txtWithdrawAmount
            // 
            this.txtWithdrawAmount.Location = new System.Drawing.Point(15, 150);
            this.txtWithdrawAmount.Name = "txtWithdrawAmount";
            this.txtWithdrawAmount.Size = new System.Drawing.Size(100, 20);
            this.txtWithdrawAmount.TabIndex = 7;
            this.txtWithdrawAmount.PlaceholderText = "Amount to Withdraw";
            // 
            // btnCalculateInterest
            // 
            this.btnCalculateInterest.Location = new System.Drawing.Point(15, 176);
            this.btnCalculateInterest.Name = "btnCalculateInterest";
            this.btnCalculateInterest.Size = new System.Drawing.Size(120, 23);
            this.btnCalculateInterest.TabIndex = 8;
            this.btnCalculateInterest.Text = "Calculate Interest (Savings)";
            this.btnCalculateInterest.UseVisualStyleBackColor = true;
            this.btnCalculateInterest.Click += new EventHandler(this.btnCalculateInterest_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 211);
            this.Controls.Add(this.btnCalculateInterest);
            this.Controls.Add(this.txtWithdrawAmount);
            this.Controls.Add(this.txtDepositAmount);
            this.Controls.Add(this.btnWithdrawSavings);
            this.Controls.Add(this.btnDepositSavings);
            this.Controls.Add(this.btnWithdrawChecking);
            this.Controls.Add(this.btnDepositChecking);
            this.Controls.Add(this.lblSavingsInfo);
            this.Controls.Add(this.lblCheckingInfo);
            this.Name = "MainForm";
            this.Text = "Bank Account App";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

