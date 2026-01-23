using MoneyTracker.Core.Models;
using System;
using System.ComponentModel;

namespace MoneyTracker.App.ViewModels
{
    public class GoalViewModel : ViewModelBase
    {
        private readonly Goal _goal;

        public Guid Id => _goal.Id;

        public string Name
        {
            get => _goal.Name;
            set
            {
                if (_goal.Name != value)
                {
                    _goal.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _goal.Description;
            set
            {
                if (_goal.Description != value)
                {
                    _goal.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public decimal TargetAmount
        {
            get => _goal.TargetAmount;
            set
            {
                if (_goal.TargetAmount != value)
                {
                    _goal.TargetAmount = value;
                    OnPropertyChanged(nameof(TargetAmount));
                    OnPropertyChanged(nameof(TargetAmountFormatted));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressPercentage));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressText));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(RemainingAmount));
                }
            }
        }

        public string TargetAmountFormatted => $"{TargetAmount:N0} ₽";

        public decimal CurrentAmount
        {
            get => _goal.CurrentAmount;
            set
            {
                if (_goal.CurrentAmount != value)
                {
                    _goal.CurrentAmount = value;
                    OnPropertyChanged(nameof(CurrentAmount));
                    OnPropertyChanged(nameof(CurrentAmountFormatted));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressPercentage));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressText));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(RemainingAmount));
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        public string CurrentAmountFormatted => $"{CurrentAmount:N0} ₽";

        public DateTime TargetDate
        {
            get => _goal.TargetDate;
            set
            {
                if (_goal.TargetDate != value)
                {
                    _goal.TargetDate = value;
                    OnPropertyChanged(nameof(TargetDate));
                    OnPropertyChanged(nameof(TargetDateFormatted));
                    // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(DaysRemaining));
                }
            }
        }

        public string TargetDateFormatted => TargetDate.ToString("dd.MM.yyyy");

        // ВРЕМЕННО ЗАКОММЕНТИРОВАТЬ эти свойства:
        /*
        public double ProgressPercentage => (double)_goal.ProgressPercentage;

        public string ProgressText => _goal.ProgressText;

        public decimal RemainingAmount => _goal.RemainingAmount;

        public string RemainingAmountFormatted => $"{RemainingAmount:N0} ₽";

        public int DaysRemaining => (TargetDate - DateTime.Now).Days;

        public string DaysRemainingText
        {
            get
            {
                int days = DaysRemaining;
                if (days < 0)
                    return $"Просрочено на {-days} дн.";
                else if (days == 0)
                    return "Сегодня";
                else
                    return $"Осталось {days} дн.";
            }
        }
        */

        public bool IsCompleted
        {
            get => _goal.IsCompleted;
            set
            {
                if (_goal.IsCompleted != value)
                {
                    _goal.IsCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                    OnPropertyChanged(nameof(StatusText));
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }

        public string StatusText => IsCompleted ? "Завершено" : "В процессе";

        public string StatusColor => IsCompleted ? "#4CAF50" : "#FF9800";

        public GoalViewModel(Goal goal)
        {
            _goal = goal ?? throw new ArgumentNullException(nameof(goal));
        }

        public Goal GetGoal() => _goal;

        public void AddAmount(decimal amount)
        {
            // ЗАКОММЕНТИРОВАТЬ: _goal.AddAmount(amount);
            _goal.CurrentAmount += amount; // Простая замена

            OnPropertyChanged(nameof(CurrentAmount));
            OnPropertyChanged(nameof(CurrentAmountFormatted));
            // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressPercentage));
            // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(ProgressText));
            // ЗАКОММЕНТИРОВАТЬ: OnPropertyChanged(nameof(RemainingAmount));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}