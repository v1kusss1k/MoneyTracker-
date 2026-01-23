using MoneyTracker.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MoneyTracker.App.ViewModels
{
    public class GoalsViewModel : ViewModelBase
    {
        private GoalViewModel? _selectedGoal; // Добавили nullable

        public ObservableCollection<GoalViewModel> Goals { get; }

        public GoalViewModel? SelectedGoal // Добавили nullable
        {
            get => _selectedGoal;
            set => SetField(ref _selectedGoal, value);
        }

        public decimal TotalGoalsAmount => Goals.Sum(g => g.TargetAmount);
        public decimal TotalSavedAmount => Goals.Sum(g => g.CurrentAmount);
        public decimal TotalProgressPercentage => TotalGoalsAmount > 0 ? (TotalSavedAmount / TotalGoalsAmount) * 100 : 0;

        public int CompletedGoalsCount => Goals.Count(g => g.IsCompleted);
        public int ActiveGoalsCount => Goals.Count(g => !g.IsCompleted);

        // Команды
        public ICommand AddGoalCommand { get; }
        public ICommand EditGoalCommand { get; }
        public ICommand DeleteGoalCommand { get; }
        public ICommand AddToGoalCommand { get; }
        public ICommand MarkCompletedCommand { get; }
        public ICommand RefreshCommand { get; }

        public GoalsViewModel()
        {
            Goals = new ObservableCollection<GoalViewModel>();

            // Инициализация команд
            AddGoalCommand = new RelayCommand(AddGoal);
            EditGoalCommand = new RelayCommand(EditGoal, CanEditDeleteGoal);
            DeleteGoalCommand = new RelayCommand(DeleteGoal, CanEditDeleteGoal);
            AddToGoalCommand = new RelayCommand(AddToGoal!, CanAddToGoal); // Добавили !
            MarkCompletedCommand = new RelayCommand(MarkCompleted!, CanMarkCompleted); // Добавили !
            RefreshCommand = new RelayCommand(Refresh!);

            LoadGoals();
        }

        private void LoadGoals()
        {
            Goals.Clear();
            // Временные тестовые данные
            Goals.Add(new GoalViewModel(new Goal("Машина", "Накопить на машину", 500000)));
            Goals.Add(new GoalViewModel(new Goal("Отпуск", "Поездка на море", 150000)));
            Goals.Add(new GoalViewModel(new Goal("Ноутбук", "Новый ноутбук для работы", 80000)));

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            OnPropertyChanged(nameof(TotalGoalsAmount));
            OnPropertyChanged(nameof(TotalSavedAmount));
            OnPropertyChanged(nameof(TotalProgressPercentage));
            OnPropertyChanged(nameof(CompletedGoalsCount));
            OnPropertyChanged(nameof(ActiveGoalsCount));
        }

        private void AddGoal(object? parameter) // Добавили nullable
        {
            // Это будет вызываться из кода окна
            System.Windows.MessageBox.Show("Функция добавления цели будет реализована в диалоговом окне", "Добавить цель");
        }

        private void EditGoal(object? parameter) // Добавили nullable
        {
            if (SelectedGoal != null)
            {
                System.Windows.MessageBox.Show($"Редактирование цели: {SelectedGoal.Name}", "Редактировать");
            }
        }

        private void DeleteGoal(object? parameter) // Добавили nullable
        {
            if (SelectedGoal != null)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Вы уверены, что хотите удалить цель '{SelectedGoal.Name}'?",
                    "Подтверждение удаления",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Goals.Remove(SelectedGoal);
                    UpdateStatistics();
                }
            }
        }

        private void AddToGoal(object? parameter) // Добавили nullable
        {
            if (SelectedGoal != null && !SelectedGoal.IsCompleted)
            {
                // Временная заглушка
                SelectedGoal.AddAmount(10000);
                UpdateStatistics();
            }
        }

        private void MarkCompleted(object? parameter) // Добавили nullable
        {
            if (SelectedGoal != null && !SelectedGoal.IsCompleted)
            {
                SelectedGoal.IsCompleted = true;
                UpdateStatistics();
            }
        }

        private void Refresh(object? parameter) // Добавили nullable
        {
            LoadGoals();
        }

        private bool CanEditDeleteGoal(object? parameter) => SelectedGoal != null; // Добавили nullable
        private bool CanAddToGoal(object? parameter) => SelectedGoal != null && !SelectedGoal.IsCompleted; // Добавили nullable
        private bool CanMarkCompleted(object? parameter) => SelectedGoal != null && !SelectedGoal.IsCompleted; // Добавили nullable
    }
}