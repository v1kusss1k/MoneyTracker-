using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddAmountWindow : Window
    {
        public decimal Amount { get; private set; }

        public AddAmountWindow()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму (положительное число)!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Amount = amount;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}