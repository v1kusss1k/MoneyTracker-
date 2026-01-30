using MoneyTracker.Core.Models;
using MoneyTracker.Core.Services;
using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddEditBudgetWindow : Window
    {
        public Budget Budget { get; private set; }

        public AddEditBudgetWindow()
        {
            InitializeComponent();
            dpMonth.SelectedDate = DateTime.Now;
            // Всё остальное ПУСТОЕ!
        }

        // Для редактирования
        public AddEditBudgetWindow(Budget existingBudget) : this()
        {
            Title = "Редактировать бюджет";
            Budget = existingBudget;

            // Заполняем существующими данными
            txtLimit.Text = existingBudget.MonthlyLimit.ToString();
            dpMonth.SelectedDate = new DateTime(existingBudget.Year, existingBudget.Month, 1);
            cmbCategory.Text = existingBudget.Category;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка категории
                string category = cmbCategory.Text?.Trim();
                if (string.IsNullOrEmpty(category))
                {
                    MessageBox.Show("Введите категорию расходов", "Ошибка");
                    cmbCategory.Focus();
                    return;
                }

                // Проверка лимита
                if (string.IsNullOrEmpty(txtLimit.Text))
                {
                    MessageBox.Show("Введите лимит", "Ошибка");
                    txtLimit.Focus();
                    return;
                }

                if (!decimal.TryParse(txtLimit.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal limit) || limit <= 0)
                {
                    MessageBox.Show("Лимит должен быть положительным числом", "Ошибка");
                    txtLimit.SelectAll();
                    txtLimit.Focus();
                    return;
                }

                // Проверка даты
                if (dpMonth.SelectedDate == null)
                {
                    MessageBox.Show("Выберите месяц", "Ошибка");
                    return;
                }

                var date = dpMonth.SelectedDate.Value;

                // Создаём или обновляем бюджет
                if (Budget == null)
                {
                    Budget = new Budget(category, limit)
                    {
                        Year = date.Year,
                        Month = date.Month
                    };
                }
                else
                {
                    Budget.Category = category;
                    Budget.MonthlyLimit = limit;
                    Budget.Year = date.Year;
                    Budget.Month = date.Month;
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