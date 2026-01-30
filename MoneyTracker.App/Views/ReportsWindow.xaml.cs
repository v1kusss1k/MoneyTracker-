#nullable disable

using MoneyTracker.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class ReportsWindow : Window
    {
        private readonly ReportService _reportService;
        private bool _isLoading = false;

        public ReportsWindow()
        {
            InitializeComponent();
            _reportService = new ReportService();

            InitializeYearComboBox();

            // Загружаем данные при открытии окна
            Loaded += async (s, e) => await LoadAllReportsAsync();
        }

        private void InitializeYearComboBox()
        {
            cmbYear.Items.Clear();

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year.ToString());
            }

            cmbYear.SelectedIndex = 0;
        }

        private async Task LoadAllReportsAsync()
        {
            if (_isLoading) return;

            _isLoading = true;

            try
            {
                // Показываем индикатор загрузки
                ShowLoading(true);

                // Загружаем все отчеты параллельно
                await Task.WhenAll(
                    LoadCurrentMonthReportAsync(),
                    LoadBudgetReportsAsync(),
                    LoadGoalReportsAsync()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчетов: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowLoading(false);
                _isLoading = false;
            }
        }

        private async Task LoadCurrentMonthReportAsync()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        int year = GetSelectedYear();
                        int month = GetSelectedMonth();

                        var report = _reportService.GetMonthlyReport(year, month);

                        txtTotalIncome.Text = $"{report.TotalIncome:N0} ₽";
                        txtTotalExpense.Text = $"{report.TotalExpense:N0} ₽";
                        txtCurrentBalance.Text = $"{report.Balance:N0} ₽";
                        txtTotalTransactions.Text = report.TransactionCount.ToString();

                        var expenseAnalysis = _reportService.GetCategoryAnalysis(
                            MoneyTracker.Core.Enums.TransactionType.Expense,
                            new DateTime(year, month, 1),
                            new DateTime(year, month, DateTime.DaysInMonth(year, month))
                        );

                        lstExpenseCategories.ItemsSource = expenseAnalysis;

                        var incomeAnalysis = _reportService.GetCategoryAnalysis(
                            MoneyTracker.Core.Enums.TransactionType.Income,
                            new DateTime(year, month, 1),
                            new DateTime(year, month, DateTime.DaysInMonth(year, month))
                        );

                        lstIncomeCategories.ItemsSource = incomeAnalysis;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки отчета: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            });
        }

        private async Task LoadBudgetReportsAsync()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var budgetReports = _reportService.GetCurrentBudgetStatus();
                        lstBudgetReports.ItemsSource = budgetReports;

                        decimal totalLimit = 0;
                        decimal totalSpent = 0;
                        decimal totalRemaining = 0;

                        foreach (var budget in budgetReports)
                        {
                            totalLimit += budget.MonthlyLimit;
                            totalSpent += budget.Spent;
                            totalRemaining += budget.Remaining;
                        }

                        txtBudgetTotalLimit.Text = $"{totalLimit:N0} ₽";
                        txtBudgetTotalSpent.Text = $"{totalSpent:N0} ₽";
                        txtBudgetTotalRemaining.Text = $"{totalRemaining:N0} ₽";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки бюджетов: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            });
        }

        private async Task LoadGoalReportsAsync()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var goalReports = _reportService.GetGoalsProgressReport();
                        lstGoalReports.ItemsSource = goalReports;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки целей: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            });
        }

        private int GetSelectedYear()
        {
            if (cmbYear.SelectedItem != null && int.TryParse(cmbYear.SelectedItem.ToString(), out int year))
            {
                return year;
            }
            return DateTime.Now.Year;
        }

        private int GetSelectedMonth()
        {
            if (cmbMonth.SelectedIndex == 0)
            {
                return DateTime.Now.Month;
            }
            else if (cmbMonth.SelectedIndex > 0)
            {
                return cmbMonth.SelectedIndex;
            }
            return DateTime.Now.Month;
        }

        private async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoading)
            {
                await LoadCurrentMonthReportAsync();
            }
        }

        private void ShowLoading(bool isLoading)
        {
            // Можно добавить индикатор загрузки если нужно
            if (isLoading)
            {
                Cursor = System.Windows.Input.Cursors.Wait;
            }
            else
            {
                Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllReportsAsync();
        }
    }
}