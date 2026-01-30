// MoneyTracker.Core/Services/BudgetService.cs
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
                    // Создаем несколько тестовых бюджетов
                    InitializeDefaultBudgets();
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

        private void InitializeDefaultBudgets()
        {
            // Примеры популярных категорий с лимитами
            var defaultBudgets = new List<Budget>
            {
                new Budget("Продукты", 15000),
                new Budget("Транспорт", 5000),
                new Budget("Кафе/рестораны", 8000),
                new Budget("Развлечения", 5000),
                new Budget("Одежда", 7000),
                new Budget("Коммуналка", 10000),
                new Budget("Здоровье", 3000),
                new Budget("Образование", 4000),
                new Budget("Подарки", 3000)
            };

            foreach (var budget in defaultBudgets)
            {
                _budgets.Add(budget);
            }

            SaveBudgets();
        }

        // Основные методы

        public List<Budget> GetAllBudgets()
        {
            return _budgets
                .Where(b => b.IsActive)
                .OrderBy(b => b.Category)
                .ToList();
        }

        public List<Budget> GetBudgetsForMonth(int year, int month)
        {
            return _budgets
                .Where(b => b.IsActive && b.Year == year && b.Month == month)
                .ToList();
        }

        public List<BudgetTracking> GetCurrentBudgetsWithTracking()
        {
            var now = DateTime.Now;
            var budgets = GetBudgetsForMonth(now.Year, now.Month);
            var result = new List<BudgetTracking>();

            foreach (var budget in budgets)
            {
                // Считаем расходы по этой категории в текущем месяце
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

            return result
                .OrderByDescending(b => b.Spent)
                .ThenBy(b => b.Budget.Category)
                .ToList();
        }

        public void AddBudget(Budget budget)
        {
            // Проверяем, нет ли уже бюджета на эту категорию в этом месяце
            var exists = _budgets.Any(b =>
                b.Category == budget.Category &&
                b.Year == budget.Year &&
                b.Month == budget.Month);

            if (!exists)
            {
                _budgets.Add(budget);
                SaveBudgets();
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
            }
        }

        public void DeleteBudget(Guid id)
        {
            _budgets.RemoveAll(b => b.Id == id);
            SaveBudgets();
        }

        public Budget? GetBudget(Guid id)
        {
            return _budgets.FirstOrDefault(b => b.Id == id);
        }

        // Утилиты

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

        public List<string> GetCategoriesWithoutBudget(int year, int month)
        {
            var budgetCategories = _budgets
                .Where(b => b.Year == year && b.Month == month)
                .Select(b => b.Category)
                .Distinct()
                .ToList();

            var allCategories = _wallet.Transactions
                .Where(t => t.Type == MoneyTracker.Core.Enums.TransactionType.Expense)
                .Select(t => t.Category)
                .Distinct()
                .ToList();

            return allCategories
                .Where(c => !budgetCategories.Contains(c))
                .OrderBy(c => c)
                .ToList();
        }
    }
}