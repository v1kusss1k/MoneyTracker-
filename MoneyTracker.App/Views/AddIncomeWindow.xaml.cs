using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class AddIncomeWindow : Window
    {
        public event EventHandler? TransactionAdded;
        private AppWallet _wallet;

        public AddIncomeWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            try
            {
                cmbCategory.Items.Clear();

                // ПРОСТЫЕ КАТЕГОРИИ ДОХОДОВ 
                cmbCategory.Items.Add("Зарплата");
                cmbCategory.Items.Add("Фриланс");
                cmbCategory.Items.Add("Инвестиции");
                cmbCategory.Items.Add("Подарок");
                cmbCategory.Items.Add("Премия");
                cmbCategory.Items.Add("Аренда");
                cmbCategory.Items.Add("Возврат долга");
                cmbCategory.Items.Add("Прочие доходы");

                if (cmbCategory.Items.Count > 0)
                    cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ПРОВЕРКА СУММЫ
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите положительную сумму!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ПОЛУЧАЕМ ВЫБРАННУЮ КАТЕГОРИЮ
                string categoryName = "Доход";

                if (cmbCategory.SelectedItem is string selectedCategory)
                {
                    categoryName = selectedCategory;
                }
                else if (cmbCategory.SelectedIndex >= 0)
                {
                    categoryName = cmbCategory.Items[cmbCategory.SelectedIndex].ToString();
                }

                // СОЗДАЁМ ТРАНЗАКЦИЮ
                var factory = new IncomeFactory();
                var transaction = factory.CreateTransaction(
                    amount,
                    categoryName,
                    txtDescription.Text.Trim()
                );

                // ДОБАВЛЯЕМ В КОШЕЛЁК
                _wallet.AddTransaction(transaction);

                // ВЫЗЫВАЕМ СОБЫТИЕ ОБНОВЛЕНИЯ
                TransactionAdded?.Invoke(this, EventArgs.Empty);

                // ЗАКРЫВАЕМ ОКНО
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении дохода: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}