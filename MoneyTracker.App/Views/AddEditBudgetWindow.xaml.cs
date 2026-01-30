// MoneyTracker.App/Views/AddEditBudgetWindow.xaml.cs
using MoneyTracker.Core.Models;
using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddEditBudgetWindow : Window
    {
        private readonly BudgetService _budgetService;
        public Budget? Budget { get; private set; }

        public AddEditBudgetWindow()
        {
            InitializeComponent();
            _budgetService = new BudgetService();
            InitializeControls();
        }

        // Конструктор для редактирования
        public AddEditBudgetWindow(Budget existingBudget) : this()
        {
            Budget = existingBudget;
            txtTitle.Text = "РЕДАКТИРОВАТЬ БЮДЖЕТ";
            LoadExistingBudget();
        }

        private void InitializeControls()
        {
            // Заполняем года (текущий и следующий)
            var currentYear = DateTime.Now.Year;
            for (int i = 0; i < 2; i++)
            {
                cmbYear.Items.Add(currentYear + i);
            }
            cmbYear.SelectedIndex = 0;

            // Месяц - текущий
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            // Категории расходов
            LoadCategories();
        }

        private void LoadExistingBudget()
        {
            if (Budget == null) return;

            txtLimit.Text = Budget.MonthlyLimit.ToString();
            cmbMonth.SelectedIndex = Budget.Month - 1;
            cmbYear.SelectedItem = Budget.Year;

            // Выбираем категорию
            foreach (var item in cmbCategory.Items)
            {
                if (item is string category && category == Budget.Category)
                {
                    cmbCategory.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                cmbCategory.Items.Clear();

                // Получаем все уникальные категории расходов из транзакций
                var wallet = AppWallet.Instance; // Получаем через Singleton
                var categories = wallet.Transactions
                    .Where(t => t.Type == MoneyTracker.Core.Enums.TransactionType.Expense)
                    .Select(t => t.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                // Добавляем популярные категории, если их еще нет
                var popularCategories = new[]
                {
                    "Продукты",
                    "Транспорт",
                    "Кафе/рестораны",
                    "Коммуналка",
                    "Одежда",
                    "Развлечения",
                    "Здоровье",
                    "Образование",
                    "Подарки",
                    "Другое"
                };

                foreach (var category in popularCategories)
                {
                    if (!categories.Contains(category))
                        categories.Add(category);
                }

                foreach (var category in categories.OrderBy(c => c))
                {
                    cmbCategory.Items.Add(category);
                }

                if (cmbCategory.Items.Count > 0)
                    cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка категории
                if (cmbCategory.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию", "Ошибка");
                    return;
                }

                // Проверка лимита
                if (!decimal.TryParse(txtLimit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal limit) || limit <= 0)
                {
                    MessageBox.Show("Введите корректный лимит (положительное число)", "Ошибка");
                    return;
                }

                // Получаем месяц и год
                var month = cmbMonth.SelectedIndex + 1;
                if (cmbYear.SelectedItem is not int year)
                {
                    year = DateTime.Now.Year;
                }

                var category = cmbCategory.SelectedItem.ToString() ?? "Другое";

                // Создаем или обновляем бюджет
                if (Budget == null)
                {
                    Budget = new Budget(category, limit)
                    {
                        Year = year,
                        Month = month
                    };
                }
                else
                {
                    Budget.Category = category;
                    Budget.MonthlyLimit = limit;
                    Budget.Year = year;
                    Budget.Month = month;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}