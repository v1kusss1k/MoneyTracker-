using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Enums;
using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddTransactionWindow : Window
    {
        public event EventHandler TransactionAdded;

        private AppWallet _wallet;

        public AddTransactionWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка");
                    return;
                }

                var type = cmbTransactionType.SelectedIndex == 0
                    ? TransactionType.Income
                    : TransactionType.Expense;

                string category = "Другое";
                if (cmbCategory.SelectedItem != null)
                {
                    category = (cmbCategory.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Другое";
                }

                var factory = type == TransactionType.Income
                    ? new IncomeFactory()
                    : new ExpenseFactory() as TransactionFactory;

                var transaction = factory.CreateTransaction(amount, category, txtDescription.Text);
                _wallet.AddTransaction(transaction);
                TransactionAdded?.Invoke(this, EventArgs.Empty);

                Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}