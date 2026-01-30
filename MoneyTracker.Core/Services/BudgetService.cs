using MoneyTracker.Core.Models;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MoneyTracker.Core.Services
{
    public class BudgetService
    {
        private List<Budget> _budgets;
        private readonly string _filePath;
        private readonly AppWallet _wallet;

        private List<Budget> _cachedBudgets;
        private DateTime _budgetsCacheTime;
        private List<BudgetTracking> _cachedBudgetTracking;
        private DateTime _trackingCacheTime;

        public BudgetService()
        {
            _wallet = AppWallet.Instance;
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MoneyTracker", "budgets.json");
            LoadBudgets();
        }

        private void LoadBudgets()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _budgets = JsonSerializer.Deserialize<List<Budget>>(json) ?? new List<Budget>();
                }
                else
                {
                    _budgets = new List<Budget>();
                    // Теперь НЕ создаем тестовые бюджеты!
                }
            }
            catch
            {
                _budgets = new List<Budget>();
            }
        }

        private void SaveBudgets()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_budgets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }

        // Основные методы (остаются без изменений)
        public List<Budget> GetAllBudgets()
        {
            // Кэшируем на 10 секунд
            if (_cachedBudgets != null && (DateTime.Now - _budgetsCacheTime).TotalSeconds < 10)
            {
                return _cachedBudgets.Where(b => b.IsActive).OrderBy(b => b.Category).ToList();
            }

            LoadBudgets();
            _cachedBudgets = _budgets;
            _budgetsCacheTime = DateTime.Now;

            return _budgets.Where(b => b.IsActive).OrderBy(b => b.Category).ToList();
        }

        public List<Budget> GetBudgetsForMonth(int year, int month)
        {
            return _budgets
                .Where(b => b.IsActive && b.Year == year && b.Month == month)
                .ToList();
        }

        public List<BudgetTracking> GetCurrentBudgetsWithTracking()
        {
            // Кэшируем на 5 секунд
            if (_cachedBudgetTracking != null && (DateTime.Now - _trackingCacheTime).TotalSeconds < 5)
            {
                return _cachedBudgetTracking;
            }

            var now = DateTime.Now;
            var budgets = GetBudgetsForMonth(now.Year, now.Month);
            var result = new List<BudgetTracking>();

            foreach (var budget in budgets)
            {
                var spent = _wallet.Transactions
                    .Where(t => t.Type == MoneyTracker.Core.Enums.TransactionType.Expense &&
                               t.Category == budget.Category &&
                               t.Date.Year == now.Year &&
                               t.Date.Month == now.Month)
                    .Sum(t => t.Amount);

                result.Add(new BudgetTracking
                {
                    Budget = budget,
                    Spent = spent
                });
            }

            var sortedResult = result
                .OrderByDescending(b => b.Spent)
                .ThenBy(b => b.Budget.Category)
                .ToList();

            // Сохраняем в кэш
            _cachedBudgetTracking = sortedResult;
            _trackingCacheTime = DateTime.Now;

            return sortedResult;
        }

        public void AddBudget(Budget budget)
        {
            var exists = _budgets.Any(b =>
                b.Category == budget.Category &&
                b.Year == budget.Year &&
                b.Month == budget.Month);

            if (!exists)
            {
                _budgets.Add(budget);
                SaveBudgets();
                // СБРАСЫВАЕМ КЭШ
                _cachedBudgets = null;
                _cachedBudgetTracking = null;
            }
        }

        public void UpdateBudget(Budget budget)
        {
            var existing = _budgets.FirstOrDefault(b => b.Id == budget.Id);
            if (existing != null)
            {
                existing.Category = budget.Category;
                existing.MonthlyLimit = budget.MonthlyLimit;
                existing.Year = budget.Year;
                existing.Month = budget.Month;
                existing.IsActive = budget.IsActive;
                SaveBudgets();
                // СБРАСЫВАЕМ КЭШ
                _cachedBudgets = null;
                _cachedBudgetTracking = null;
            }
        }

        public void DeleteBudget(Guid id)
        {
            _budgets.RemoveAll(b => b.Id == id);
            SaveBudgets();
            // СБРАСЫВАЕМ КЭШ
            _cachedBudgets = null;
            _cachedBudgetTracking = null;
        }

        public Budget GetBudget(Guid id)
        {
            return _budgets.FirstOrDefault(b => b.Id == id);
        }

        public decimal GetTotalMonthlyLimit()
        {
            var now = DateTime.Now;
            return _budgets
                .Where(b => b.IsActive && b.Year == now.Year && b.Month == now.Month)
                .Sum(b => b.MonthlyLimit);
        }

        public decimal GetTotalSpentThisMonth()
        {
            var now = DateTime.Now;
            return _wallet.Transactions
                .Where(t => t.Type == MoneyTracker.Core.Enums.TransactionType.Expense &&
                           t.Date.Year == now.Year &&
                           t.Date.Month == now.Month)
                .Sum(t => t.Amount);
        }

        public decimal GetTotalRemaining()
        {
            return GetTotalMonthlyLimit() - GetTotalSpentThisMonth();
        }
    }
}