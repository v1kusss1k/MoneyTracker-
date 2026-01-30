// MoneyTracker.App/Views/GoalsWindow.xaml.cs
using MoneyTracker.Core.Models;
using MoneyTracker.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class GoalsWindow : Window
    {
        private readonly GoalService _goalService;
        private ObservableCollection<Goal> _goals;

        public GoalsWindow()
        {
            InitializeComponent();
            _goalService = new GoalService();
            _goals = new ObservableCollection<Goal>(); // Инициализируем
            LoadGoals();
        }

        private void LoadGoals()
        {
            try
            {
                _goals.Clear();
                var allGoals = _goalService.GetGoals();
                bool hasNewCompletedGoals = false;

                foreach (var goal in allGoals)
                {
                    _goals.Add(goal);

                    // Проверяем на новые завершенные цели
                    if (goal.IsCompleted && !goal.WasNotified)
                    {
                        hasNewCompletedGoals = true;
                        goal.WasNotified = true;
                        _goalService.UpdateGoal(goal); // Обновляем в базе
                    }
                }

                goalsListView.ItemsSource = _goals;

                // Показываем общее уведомление
                if (hasNewCompletedGoals)
                {
                    MessageBox.Show("🎉 Есть достигнутые цели!\nПосмотрите список целей.",
                                  "Поздравляем!",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки целей: {ex.Message}", "Ошибка");
            }
        }

        private void BtnAddGoal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AddGoalWindow();
                if (window.ShowDialog() == true && window.Goal != null)
                {
                    _goalService.AddGoal(window.Goal);
                    LoadGoals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddMoney_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag is Guid goalId)
                {
                    var window = new AddAmountWindow { Owner = this };
                    if (window.ShowDialog() == true)
                    {
                        _goalService.AddToGoal(goalId, window.Amount);
                        LoadGoals();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите цель для пополнения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteGoal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag is Guid goalId)
                {
                    var goal = _goalService.GetGoalById(goalId);
                    if (goal != null)
                    {
                        var result = MessageBox.Show(
                            $"Удалить цель '{goal.Name}'?\nВсе данные будут потеряны.",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            _goalService.DeleteGoal(goalId);
                            LoadGoals();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}