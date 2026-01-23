using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddIncomeWindow : Window
    {
        public event EventHandler TransactionAdded;
        private AppWallet _wallet;

        public AddIncomeWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите положительную сумму!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string category = "Доход";
                if (cmbCategory.SelectedItem != null)
                {
                    category = (cmbCategory.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Доход";
                }

                var factory = new IncomeFactory();
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