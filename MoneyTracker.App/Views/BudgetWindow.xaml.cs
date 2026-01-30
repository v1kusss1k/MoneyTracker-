using MoneyTracker.Core.Models;
using MoneyTracker.Core.Services;
using MoneyTracker.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace MoneyTracker.App.Views
{
    public partial class BudgetWindow : Window
    {
        private readonly BudgetService _budgetService;
        private ObservableCollection<BudgetViewModel> _budgets;

        public BudgetWindow()
        {
            InitializeComponent();
            _budgetService = new BudgetService();
            _budgets = new ObservableCollection<BudgetViewModel>();
            LoadBudgets();
            UpdateTotalStatistics();
        }

        private void LoadBudgets()
        {
            try
            {
                _budgets.Clear();
                var budgetTrackings = _budgetService.GetCurrentBudgetsWithTracking();

                foreach (var tracking in budgetTrackings)
                {
                    var viewModel = new BudgetViewModel(tracking);
                    // Цвета уже установлены в конструкторе BudgetViewModel
                    _budgets.Add(viewModel);
                }

                lstBudgets.ItemsSource = _budgets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бюджетов: {ex.Message}", "Ошибка");
            }
        }

        private Brush GetStatusColor(BudgetViewModel budget)
        {
            if (budget.IsExceeded)
                return new SolidColorBrush(Color.FromRgb(244, 67, 67)); // Красный
            if (budget.IsWarning)
                return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Оранжевый
            return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
        }

        private Brush GetProgressColor(BudgetViewModel budget)
        {
            if (budget.IsExceeded)
                return new SolidColorBrush(Color.FromRgb(244, 67, 67)); // Красный
            if (budget.IsWarning)
                return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Желтый
            return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
        }

        private void UpdateTotalStatistics()
        {
            try
            {
                txtTotalLimit.Text = $"{_budgetService.GetTotalMonthlyLimit():N0} ₽";
                txtTotalSpent.Text = $"{_budgetService.GetTotalSpentThisMonth():N0} ₽";
                txtTotalRemaining.Text = $"{_budgetService.GetTotalRemaining():N0} ₽";
                txtBudgetCount.Text = _budgets.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статистики: {ex.Message}", "Ошибка");
            }
        }

        private void BtnAddBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AddEditBudgetWindow();
                if (window.ShowDialog() == true && window.Budget != null)
                {
                    _budgetService.AddBudget(window.Budget);
                    LoadBudgets();
                    UpdateTotalStatistics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания бюджета: {ex.Message}", "Ошибка");
            }
        }

        private void BtnEditBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as FrameworkElement;
                if (button?.Tag is Guid budgetId)
                {
                    var budget = _budgetService.GetBudget(budgetId);
                    if (budget != null)
                    {
                        var window = new AddEditBudgetWindow(budget);
                        if (window.ShowDialog() == true && window.Budget != null)
                        {
                            _budgetService.UpdateBudget(window.Budget);
                            LoadBudgets();
                            UpdateTotalStatistics();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования бюджета: {ex.Message}", "Ошибка");
            }
        }

        private void BtnDeleteBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as FrameworkElement;
                if (button?.Tag is Guid budgetId)
                {
                    var budget = _budgetService.GetBudget(budgetId);
                    if (budget != null)
                    {
                        var result = MessageBox.Show(
                            $"Удалить бюджет для категории '{budget.Category}'?\nЛимит: {budget.MonthlyLimit:N0}₽",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            _budgetService.DeleteBudget(budgetId);
                            LoadBudgets();
                            UpdateTotalStatistics();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления бюджета: {ex.Message}", "Ошибка");
            }
        }

        private void BtnRecommendations_Click(object sender, RoutedEventArgs e)
        {
            ShowRecommendations();
        }

        private void ShowRecommendations()
        {
            var recommendations = "";
            var exceededBudgets = _budgets.Where(b => b.IsExceeded).ToList();
            var warningBudgets = _budgets.Where(b => b.IsWarning).ToList();

            if (exceededBudgets.Any())
            {
                recommendations += "⚠️ **Превышены лимиты:**\n";
                foreach (var budget in exceededBudgets)
                {
                    recommendations += $"• {budget.Category}: превышение на {budget.Spent - budget.MonthlyLimit:N0}₽\n";
                }
                recommendations += "\n";
            }

            if (warningBudgets.Any())
            {
                recommendations += "🔸 **Близко к лимиту:**\n";
                foreach (var budget in warningBudgets)
                {
                    recommendations += $"• {budget.Category}: осталось {budget.Remaining:N0}₽ ({100 - budget.Percentage:F0}%)\n";
                }
                recommendations += "\n";
            }

            if (string.IsNullOrEmpty(recommendations))
            {
                recommendations = "✅ Все бюджеты в норме! Отличная работа!";
            }
            else
            {
                recommendations += "💡 **Советы:**\n" +
                                 "1. Пересмотрите траты в категориях с превышением\n" +
                                 "2. Подумайте о снижении лимитов\n" +
                                 "3. Отложите крупные покупки на следующий месяц";
            }

            MessageBox.Show(recommendations, "Рекомендации по бюджетам",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}