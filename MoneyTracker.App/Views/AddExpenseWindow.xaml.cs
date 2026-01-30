using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
// УБРАТЬ using MoneyTracker.Core.Services;
using MoneyTracker.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class AddExpenseWindow : Window
    {
        public event EventHandler TransactionAdded;
        private AppWallet _wallet;
        // private CategoryService _categoryService; // УДАЛИТЬ

        public AddExpenseWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
            // _categoryService = new CategoryService(); // УДАЛИТЬ
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            try
            {
                cmbCategory.Items.Clear();

                // ПРОСТЫЕ КАТЕГОРИИ РАСХОДОВ (без CategoryService)
                cmbCategory.Items.Add("Продукты");
                cmbCategory.Items.Add("Транспорт");
                cmbCategory.Items.Add("Коммуналка");
                cmbCategory.Items.Add("Кафе/рестораны");
                cmbCategory.Items.Add("Одежда");
                cmbCategory.Items.Add("Здоровье");
                cmbCategory.Items.Add("Развлечения");
                cmbCategory.Items.Add("Образование");
                cmbCategory.Items.Add("Путешествия");
                cmbCategory.Items.Add("Техника");
                cmbCategory.Items.Add("Кредиты");
                cmbCategory.Items.Add("Домашние животные");
                cmbCategory.Items.Add("Прочие расходы");

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
                string categoryName = "Расход";

                if (cmbCategory.SelectedItem is string selectedCategory)
                {
                    categoryName = selectedCategory;
                }
                else if (cmbCategory.SelectedIndex >= 0)
                {
                    categoryName = cmbCategory.Items[cmbCategory.SelectedIndex].ToString();
                }

                // СОЗДАЁМ ТРАНЗАКЦИЮ
                var factory = new ExpenseFactory();
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