#nullable disable

using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyTracker.Core.Services
{
    public class ReportService
    {
        private readonly AppWallet _wallet;
        private readonly BudgetService _budgetService;
        private readonly GoalService _goalService;

        // Кэш для отчетов
        private MonthlyReport _cachedMonthlyReport;
        private DateTime _cacheTimestamp;
        private int _cachedYear;
        private int _cachedMonth;

        public ReportService()
        {
            _wallet = AppWallet.Instance;
            _budgetService = new BudgetService();
            _goalService = new GoalService();
            _cacheTimestamp = DateTime.MinValue;
        }

        public class MonthlyReport
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalIncome { get; set; }
            public decimal TotalExpense { get; set; }
            public decimal Balance { get; set; }
            public int TransactionCount { get; set; }
            public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }

        public MonthlyReport GetMonthlyReport(int year, int month)
        {
            // Используем кэш, если запрашиваем тот же месяц и данные не устарели (5 секунд)
            if (_cachedMonthlyReport != null &&
                _cachedYear == year &&
                _cachedMonth == month &&
                (DateTime.Now - _cacheTimestamp).TotalSeconds < 5)
            {
                return _cachedMonthlyReport;
            }

            var report = new MonthlyReport
            {
                Year = year,
                Month = month
            };

            // Оптимизация: делаем один проход по транзакциям
            var monthTransactions = new List<Transaction>();
            decimal totalIncome = 0;
            decimal totalExpense = 0;
            int transactionCount = 0;

            foreach (var transaction in _wallet.Transactions)
            {
                if (transaction.Date.Year == year && transaction.Date.Month == month)
                {
                    transactionCount++;
                    if (transaction.Type == TransactionType.Income)
                    {
                        totalIncome += transaction.Amount;
                    }
                    else
                    {
                        totalExpense += transaction.Amount;
                    }
                    monthTransactions.Add(transaction);
                }
            }

            report.TransactionCount = transactionCount;
            report.TotalIncome = totalIncome;
            report.TotalExpense = totalExpense;
            report.Balance = totalIncome - totalExpense;

            // Сохраняем в кэш
            _cachedMonthlyReport = report;
            _cachedYear = year;
            _cachedMonth = month;
            _cacheTimestamp = DateTime.Now;

            return report;
        }

        public class CategoryAnalysis
        {
            public string Category { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public List<CategoryAnalysis> GetCategoryAnalysis(TransactionType type, DateTime startDate, DateTime endDate)
        {
            // Быстрый способ без LINQ GroupBy (он может быть медленным)
            var categoryTotals = new Dictionary<string, decimal>();

            foreach (var transaction in _wallet.Transactions)
            {
                if (transaction.Type == type &&
                    transaction.Date >= startDate &&
                    transaction.Date <= endDate)
                {
                    if (categoryTotals.ContainsKey(transaction.Category))
                    {
                        categoryTotals[transaction.Category] += transaction.Amount;
                    }
                    else
                    {
                        categoryTotals[transaction.Category] = transaction.Amount;
                    }
                }
            }

            // Преобразуем в список и сортируем
            return categoryTotals
                .Select(kv => new CategoryAnalysis
                {
                    Category = kv.Key,
                    TotalAmount = kv.Value
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(5) // Только топ-5
                .ToList();
        }

        public class GoalProgressReport
        {
            public string GoalName { get; set; }
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; }
            public decimal ProgressPercentage { get; set; }
            public int DaysRemaining { get; set; }
            public string Status { get; set; }
        }

        public List<GoalProgressReport> GetGoalsProgressReport()
        {
            var goals = _goalService.GetGoals();
            var reports = new List<GoalProgressReport>();

            foreach (var goal in goals)
            {
                reports.Add(new GoalProgressReport
                {
                    GoalName = goal.Name,
                    TargetAmount = goal.TargetAmount,
                    CurrentAmount = goal.CurrentAmount,
                    ProgressPercentage = goal.ProgressPercentage,
                    DaysRemaining = goal.DaysRemaining,
                    Status = goal.Status
                });
            }

            return reports.OrderByDescending(g => g.ProgressPercentage).ToList();
        }

        public class BudgetStatusReport
        {
            public string Category { get; set; }
            public decimal MonthlyLimit { get; set; }
            public decimal Spent { get; set; }
            public decimal Remaining { get; set; }
            public decimal Percentage { get; set; }
            public string Status { get; set; }
        }

        public List<BudgetStatusReport> GetCurrentBudgetStatus()
        {
            var budgets = _budgetService.GetCurrentBudgetsWithTracking();
            var reports = new List<BudgetStatusReport>();

            foreach (var budget in budgets)
            {
                var status = budget.IsExceeded ? "Превышен" :
                            budget.IsWarning ? "Близко к лимиту" : "В норме";

                reports.Add(new BudgetStatusReport
                {
                    Category = budget.Budget.Category,
                    MonthlyLimit = budget.Budget.MonthlyLimit,
                    Spent = budget.Spent,
                    Remaining = budget.Remaining,
                    Percentage = budget.Percentage,
                    Status = status
                });
            }

            return reports.OrderByDescending(b => b.Percentage).ToList();
        }
    }
}