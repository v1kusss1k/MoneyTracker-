// MoneyTracker.App/ViewModels/BudgetViewModel.cs
using MoneyTracker.Core.Models;
using System;
using System.Windows.Media;

namespace MoneyTracker.App.ViewModels
{
    public class BudgetViewModel : ViewModelBase
    {
        private readonly BudgetTracking _tracking;
        private Brush? _statusColor;
        private Brush? _progressColor;

        public Guid Id => _tracking.Budget.Id;
        public string Category => _tracking.Budget.Category;
        public decimal MonthlyLimit => _tracking.Budget.MonthlyLimit;
        public decimal Spent => _tracking.Spent;
        public decimal Remaining => _tracking.Remaining;
        public decimal Percentage => _tracking.Percentage;
        public bool IsExceeded => _tracking.IsExceeded;
        public bool IsWarning => _tracking.IsWarning;
        public string Status => _tracking.Status;
        public string ProgressText => _tracking.ProgressText;
        public string MonthName => _tracking.Budget.MonthName;

        // Цвета для UI (может быть null)
        public Brush? StatusColor
        {
            get => _statusColor;
            set => SetField(ref _statusColor, value);
        }

        public Brush? ProgressColor
        {
            get => _progressColor;
            set => SetField(ref _progressColor, value);
        }

        // Для прогресс-бара
        public int ProgressBarWidth => Math.Min(100, (int)Percentage);

        public BudgetViewModel(BudgetTracking tracking)
        {
            _tracking = tracking ?? throw new ArgumentNullException(nameof(tracking));
            // Инициализируем цвета по умолчанию
            UpdateColors();
        }

        private void UpdateColors()
        {
            StatusColor = GetStatusBrush();
            ProgressColor = GetProgressBrush();
        }

        private Brush GetStatusBrush()
        {
            if (IsExceeded)
                return new SolidColorBrush(Color.FromRgb(244, 67, 67)); // Красный
            if (IsWarning)
                return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Оранжевый
            return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
        }

        private Brush GetProgressBrush()
        {
            if (IsExceeded)
                return new SolidColorBrush(Color.FromRgb(244, 67, 67)); // Красный
            if (IsWarning)
                return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Желтый
            return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
        }

        // Для сортировки
        public decimal SpentToLimitRatio => MonthlyLimit > 0 ? Spent / MonthlyLimit : 0;
    }
}