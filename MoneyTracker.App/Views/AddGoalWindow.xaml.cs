#nullable disable
using MoneyTracker.Core.Models;
using System;
using System.Globalization;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class AddGoalWindow : Window
    {
        public Goal? Goal { get; private set; }

        public AddGoalWindow()
        {
            InitializeComponent();
            dpTargetDate.SelectedDate = DateTime.Now.AddMonths(1);
        }

        // Конструктор для редактирования существующей цели
        public AddGoalWindow(Goal existingGoal) : this()
        {
            Title = "Редактировать цель";
            txtName.Text = existingGoal.Name;
            txtDescription.Text = existingGoal.Description;
            txtTargetAmount.Text = existingGoal.TargetAmount.ToString();
            dpTargetDate.SelectedDate = existingGoal.TargetDate;
            Goal = existingGoal;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка названия
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название цели!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка суммы
                if (!decimal.TryParse(txtTargetAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal targetAmount) || targetAmount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму (положительное число)!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка даты
                if (dpTargetDate.SelectedDate == null || dpTargetDate.SelectedDate < DateTime.Now)
                {
                    MessageBox.Show("Выберите корректную дату в будущем!", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создаём или обновляем цель
                if (Goal == null)
                {
                    Goal = new Goal(
                        txtName.Text.Trim(),
                        txtDescription.Text.Trim(),
                        targetAmount);
                }
                else
                {
                    Goal.Name = txtName.Text.Trim();
                    Goal.Description = txtDescription.Text.Trim();
                    Goal.TargetAmount = targetAmount;
                }

                Goal.TargetDate = dpTargetDate.SelectedDate.Value;

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