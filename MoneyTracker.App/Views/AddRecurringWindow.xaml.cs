#nullable disable

using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class AddRecurringWindow : Window
    {
        public RecurringTransaction RecurringTransaction { get; private set; }

        public AddRecurringWindow()
        {
            InitializeComponent();
            cmbRecurrence.SelectionChanged += CmbRecurrence_SelectionChanged;

            // Установить значение по умолчанию для дня
            txtDay.Text = DateTime.Now.Day.ToString();
        }

        public AddRecurringWindow(RecurringTransaction existing) : this()
        {
            Title = "Редактировать регулярный платеж";

            txtName.Text = existing.Name;
            txtAmount.Text = existing.Amount.ToString();
            txtCategory.Text = existing.Category;
            cmbType.SelectedIndex = existing.Type == TransactionType.Income ? 1 : 0;
            cmbRecurrence.SelectedIndex = (int)existing.Recurrence;

            // Заполняем день ДЛЯ РЕДАКТИРОВАНИЯ
            if (existing.Recurrence == RecurrenceType.Monthly && existing.DayOfMonth > 0)
            {
                txtDay.Text = existing.DayOfMonth.ToString();
                pnlDaySettings.Visibility = Visibility.Visible;
            }
            else if (existing.Recurrence == RecurrenceType.Weekly)
            {
                // Для еженедельных преобразуем DayOfWeek в число (1-7)
                txtDay.Text = ((int)existing.DayOfWeek + 1).ToString();
                pnlDaySettings.Visibility = Visibility.Visible;
                txtDayLabel.Text = "День недели (1-ПН, 2-ВТ...7-ВС):";
            }

            RecurringTransaction = existing;
        }

        private void CmbRecurrence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRecurrence.SelectedIndex >= 0)
            {
                var recurrence = (RecurrenceType)cmbRecurrence.SelectedIndex;

                switch (recurrence)
                {
                    case RecurrenceType.Monthly:
                        pnlDaySettings.Visibility = Visibility.Visible;
                        txtDayLabel.Text = "День месяца (1-31):";
                        // Установить текущий день если поле пустое
                        if (string.IsNullOrEmpty(txtDay.Text) || txtDay.Text == "0")
                            txtDay.Text = DateTime.Now.Day.ToString();
                        break;

                    case RecurrenceType.Weekly:
                        pnlDaySettings.Visibility = Visibility.Visible;
                        txtDayLabel.Text = "День недели (1-ПН, 2-ВТ...7-ВС):";
                        // Установить понедельник (1) по умолчанию
                        if (string.IsNullOrEmpty(txtDay.Text) || txtDay.Text == "0")
                            txtDay.Text = "1";
                        break;

                    default:
                        pnlDaySettings.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Проверяем название
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название платежа", "Ошибка");
                    txtName.Focus();
                    return;
                }

                // 2. Проверяем сумму
                if (string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    MessageBox.Show("Введите сумму", "Ошибка");
                    txtAmount.Focus();
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму (положительное число)", "Ошибка");
                    txtAmount.SelectAll();
                    txtAmount.Focus();
                    return;
                }

                // 3. Проверяем тип
                if (cmbType.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите тип (доход/расход)", "Ошибка");
                    cmbType.Focus();
                    return;
                }

                // 4. Проверяем категорию
                if (string.IsNullOrWhiteSpace(txtCategory.Text))
                {
                    MessageBox.Show("Введите категорию", "Ошибка");
                    txtCategory.Focus();
                    return;
                }

                // 5. Проверяем периодичность
                if (cmbRecurrence.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите периодичность", "Ошибка");
                    cmbRecurrence.Focus();
                    return;
                }

                var type = cmbType.SelectedIndex == 1 ? TransactionType.Income : TransactionType.Expense;
                var recurrence = (RecurrenceType)cmbRecurrence.SelectedIndex;
                var category = txtCategory.Text.Trim();

                // 6. Вычисляем следующую дату
                DateTime nextDate = CalculateNextDate(recurrence);

                // 7. Создаем или обновляем
                if (RecurringTransaction == null)
                {
                    RecurringTransaction = new RecurringTransaction(
                        txtName.Text.Trim(),
                        amount,
                        type,
                        category,
                        recurrence
                    );
                }
                else
                {
                    // Обновляем основные поля
                    RecurringTransaction.Name = txtName.Text.Trim();
                    RecurringTransaction.Amount = amount;
                    RecurringTransaction.Type = type;
                    RecurringTransaction.Category = category;
                    RecurringTransaction.Recurrence = recurrence;
                }

                // Устанавливаем день для месячных (и для редактирования тоже!)
                if (recurrence == RecurrenceType.Monthly)
                {
                    if (int.TryParse(txtDay.Text, out int day) && day >= 1 && day <= 31)
                    {
                        RecurringTransaction.DayOfMonth = day;
                    }
                    else
                    {
                        // Если число невалидное, используем текущий день или 1
                        RecurringTransaction.DayOfMonth = DateTime.Now.Day;
                    }
                }
                else if (recurrence == RecurrenceType.Weekly)
                {
                    // Для еженедельных - устанавливаем день недели
                    if (int.TryParse(txtDay.Text, out int dayOfWeek) && dayOfWeek >= 1 && dayOfWeek <= 7)
                    {
                        RecurringTransaction.DayOfWeek = (DayOfWeek)(dayOfWeek - 1); // 1=Понедельник, 7=Воскресенье
                    }
                }

                // Устанавливаем следующую дату
                RecurringTransaction.NextDate = nextDate;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private DateTime CalculateNextDate(RecurrenceType recurrence)
        {
            var today = DateTime.Today;

            switch (recurrence)
            {
                case RecurrenceType.Daily:
                    return today.AddDays(1);

                case RecurrenceType.Weekly:
                    // Следующий выбранный день недели
                    if (int.TryParse(txtDay.Text, out int dayOfWeek) && dayOfWeek >= 1 && dayOfWeek <= 7)
                    {
                        var targetDayOfWeek = (DayOfWeek)(dayOfWeek - 1);
                        int daysToAdd = ((int)targetDayOfWeek - (int)today.DayOfWeek + 7) % 7;
                        if (daysToAdd == 0) daysToAdd = 7; // Если сегодня тот же день - на следующую неделю
                        return today.AddDays(daysToAdd);
                    }
                    return today.AddDays(7); // По умолчанию через неделю

                case RecurrenceType.Monthly:
                    int dayOfMonth;
                    if (int.TryParse(txtDay.Text, out dayOfMonth) && dayOfMonth >= 1 && dayOfMonth <= 31)
                    {
                        // Берем текущий месяц
                        var nextDate = new DateTime(today.Year, today.Month,
                            Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year, today.Month)));

                        // Если эта дата уже прошла в этом месяце - берем следующий месяц
                        if (nextDate < today)
                        {
                            nextDate = nextDate.AddMonths(1);
                            // Корректируем день если в следующем месяце меньше дней
                            if (dayOfMonth > DateTime.DaysInMonth(nextDate.Year, nextDate.Month))
                            {
                                dayOfMonth = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
                            }
                            nextDate = new DateTime(nextDate.Year, nextDate.Month, dayOfMonth);
                        }
                        return nextDate;
                    }
                    else
                    {
                        // Если день не указан, то на этот же день следующего месяца
                        return today.AddMonths(1);
                    }

                case RecurrenceType.Yearly:
                    return today.AddYears(1);

                default:
                    return today.AddMonths(1);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}