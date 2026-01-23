using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;

namespace MoneyTracker.App.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly AppWallet _wallet;

        public SeriesCollection ExpenseSeries { get; set; }
        public SeriesCollection IncomeExpenseSeries { get; set; }
        public ObservableCollection<string> Months { get; set; }

        public ReportsViewModel()
        {
            _wallet = AppWallet.Instance;

            ExpenseSeries = new SeriesCollection();
            IncomeExpenseSeries = new SeriesCollection();
            Months = new ObservableCollection<string>();

            LoadData();
        }

        private void LoadData()
        {
            // Загрузка данных для круговой диаграммы расходов
            var expenseByCategory = _wallet.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .Take(6) // Топ-6 категорий
                .ToList();

            ExpenseSeries.Clear();
            foreach (var item in expenseByCategory)
            {
                ExpenseSeries.Add(new PieSeries
                {
                    Title = item.Category,
                    Values = new ChartValues<decimal> { item.Amount },
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0}₽ ({point.Participation:P0})"
                });
            }

            // Загрузка данных для столбчатой диаграммы
            var monthlyData = _wallet.Transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .TakeLast(6) // Последние 6 месяцев
                .ToList();

            IncomeExpenseSeries.Clear();
            Months.Clear();

            if (monthlyData.Any())
            {
                IncomeExpenseSeries.Add(new ColumnSeries
                {
                    Title = "Доходы",
                    Values = new ChartValues<decimal>(monthlyData.Select(d => d.Income))
                });

                IncomeExpenseSeries.Add(new ColumnSeries
                {
                    Title = "Расходы",
                    Values = new ChartValues<decimal>(monthlyData.Select(d => d.Expense))
                });

                foreach (var data in monthlyData)
                {
                    Months.Add(data.Month.ToString("MMM yyyy"));
                }
            }
        }

        public Func<double, string> CurrencyFormatter => value => $"{value:N0} ₽";

        public void Refresh()
        {
            LoadData();
            OnPropertyChanged(nameof(ExpenseSeries));
            OnPropertyChanged(nameof(IncomeExpenseSeries));
            OnPropertyChanged(nameof(Months));
        }
    }
}