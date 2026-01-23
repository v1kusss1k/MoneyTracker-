using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddExpenseWindow : Window
    {
        public event EventHandler TransactionAdded;
        private AppWallet _wallet;

        public AddExpenseWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем сумму
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите положительную сумму!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получаем категорию
                string category = "Расход";
                if (cmbCategory.SelectedItem != null)
                {
                    category = (cmbCategory.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Расход";
                }

                // Создаем транзакцию через фабрику расходов
                var factory = new ExpenseFactory();
                var transaction = factory.CreateTransaction(amount, category, txtDescription.Text);

                // Добавляем в кошелек
                _wallet.AddTransaction(transaction);

                // Вызываем событие обновления
                TransactionAdded?.Invoke(this, EventArgs.Empty);

                // Закрываем окно
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении расхода: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}