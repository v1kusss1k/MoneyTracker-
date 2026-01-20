using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class AddTransactionWindow : Window
    {
        public event Action TransactionAdded;

        public AddTransactionWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем сумму
                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму!", "Ошибка");
                    return;
                }

                // Получаем тип
                var selectedItem = cmbType.SelectedItem as ComboBoxItem;
                var type = selectedItem?.Tag?.ToString() == "Income"
                    ? TransactionType.Income
                    : TransactionType.Expense;

                // Получаем категорию
                string category = cmbCategory.Text;
                if (string.IsNullOrWhiteSpace(category))
                    category = type == TransactionType.Income ? "Доход" : "Расход";

                // Получаем описание
                string description = txtDescription.Text;
                if (string.IsNullOrWhiteSpace(description))
                    description = $"{type}: {category}";

                // Создаём транзакцию через фабрику
                var factory = TransactionFactoryCreator.CreateFactory(type);
                var transaction = factory.CreateTransaction(amount, category, description);

                // Добавляем в кошелёк
                AppWallet.Instance.AddTransaction(transaction);

                // Вызываем событие
                TransactionAdded?.Invoke();

                MessageBox.Show("Операция успешно добавлена!", "Успех");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}