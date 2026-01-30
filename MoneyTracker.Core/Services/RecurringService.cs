// MoneyTracker.Core/Services/RecurringService.cs
using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MoneyTracker.Core.Services
{
    public class RecurringService
    {
        private List<RecurringTransaction> _recurringTransactions;
        private readonly string _filePath;
        private readonly AppWallet _wallet;

        private List<RecurringTransaction> _cachedTransactions;
        private DateTime _transactionsCacheTime;

        public RecurringService()
        {
            _wallet = AppWallet.Instance;
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MoneyTracker", "recurring.json");
            LoadRecurringTransactions();
        }

        private void LoadRecurringTransactions()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _recurringTransactions = JsonSerializer.Deserialize<List<RecurringTransaction>>(json)
                        ?? new List<RecurringTransaction>();
                }
                else
                {
                    _recurringTransactions = new List<RecurringTransaction>();
                    InitializeExamples();
                }
            }
            catch
            {
                _recurringTransactions = new List<RecurringTransaction>();
            }
        }

        private void SaveRecurringTransactions()
        {
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_recurringTransactions,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch { }
        }

        private void InitializeExamples()
        {
            SaveRecurringTransactions();
        }

        // Основные методы

        public List<RecurringTransaction> GetAllRecurringTransactions()
        {
            // Кэшируем на 10 секунд
            if (_cachedTransactions != null && (DateTime.Now - _transactionsCacheTime).TotalSeconds < 10)
            {
                return _cachedTransactions
                    .Where(rt => rt.IsActive)
                    .OrderBy(rt => rt.NextDate)
                    .ThenBy(rt => rt.Name)
                    .ToList();
            }

            LoadRecurringTransactions();
            _cachedTransactions = _recurringTransactions;
            _transactionsCacheTime = DateTime.Now;

            return _recurringTransactions
                .Where(rt => rt.IsActive)
                .OrderBy(rt => rt.NextDate)
                .ThenBy(rt => rt.Name)
                .ToList();
        }

        public List<RecurringTransaction> GetActiveRecurringTransactions()
        {
            return _recurringTransactions
                .Where(rt => rt.IsActive && !rt.IsPaused)
                .OrderBy(rt => rt.NextDate)
                .ToList();
        }

        public List<RecurringTransaction> GetDueToday()
        {
            return _recurringTransactions
                .Where(rt => rt.ShouldProcessToday())
                .ToList();
        }

        public void AddRecurringTransaction(RecurringTransaction transaction)
        {
            _recurringTransactions.Add(transaction);
            SaveRecurringTransactions();
        }

        public void UpdateRecurringTransaction(RecurringTransaction transaction)
        {
            var existing = _recurringTransactions.FirstOrDefault(rt => rt.Id == transaction.Id);
            if (existing != null)
            {
                existing.Name = transaction.Name;
                existing.Description = transaction.Description;
                existing.Amount = transaction.Amount;
                existing.Type = transaction.Type;
                existing.Category = transaction.Category;
                existing.Recurrence = transaction.Recurrence;
                existing.DayOfMonth = transaction.DayOfMonth;
                existing.DayOfWeek = transaction.DayOfWeek;
                existing.StartDate = transaction.StartDate;
                existing.EndDate = transaction.EndDate;
                existing.NextDate = transaction.NextDate;
                existing.IsActive = transaction.IsActive;
                existing.IsPaused = transaction.IsPaused;
                SaveRecurringTransactions();
            }
        }

        public void DeleteRecurringTransaction(Guid id)
        {
            _recurringTransactions.RemoveAll(rt => rt.Id == id);
            SaveRecurringTransactions();
        }

        public RecurringTransaction? GetRecurringTransaction(Guid id)
        {
            return _recurringTransactions.FirstOrDefault(rt => rt.Id == id);
        }

        // Обработка регулярных транзакций

        public List<RecurringTransaction> ProcessDueTransactions()
        {
            var processedTransactions = new List<RecurringTransaction>();
            var dueTransactions = GetDueToday();

            foreach (var recurring in dueTransactions)
            {
                try
                {
                    // Создаем фактическую транзакцию
                    TransactionFactory factory = recurring.Type == TransactionType.Income
                        ? new IncomeFactory()
                        : new ExpenseFactory();

                    var transaction = factory.CreateTransaction(
                        recurring.Amount,
                        recurring.Category,
                        $"{recurring.Name} (автоматически)"
                    );

                    // Добавляем в кошелек
                    _wallet.AddTransaction(transaction);

                    // Обновляем дату следующего выполнения
                    recurring.LastProcessedDate = DateTime.Now;
                    recurring.NextDate = recurring.CalculateNextDate();

                    // Сохраняем изменения
                    UpdateRecurringTransaction(recurring);
                    processedTransactions.Add(recurring);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки регулярной транзакции {recurring.Name}: {ex.Message}");
                }
            }

            return processedTransactions;
        }

        public void TogglePause(Guid id)
        {
            var transaction = GetRecurringTransaction(id);
            if (transaction != null)
            {
                transaction.IsPaused = !transaction.IsPaused;
                UpdateRecurringTransaction(transaction);
            }
        }

        public void SkipNextOccurrence(Guid id)
        {
            var transaction = GetRecurringTransaction(id);
            if (transaction != null)
            {
                transaction.NextDate = transaction.CalculateNextDate();
                UpdateRecurringTransaction(transaction);
            }
        }

        // Статистика

        public int GetActiveCount()
        {
            return _recurringTransactions.Count(rt => rt.IsActive && !rt.IsPaused);
        }

        public decimal GetMonthlyRecurringTotal()
        {
            return _recurringTransactions
                .Where(rt => rt.IsActive && !rt.IsPaused && rt.Recurrence == RecurrenceType.Monthly)
                .Sum(rt => rt.Amount * (rt.Type == TransactionType.Income ? 1 : -1));
        }

        public List<RecurringTransaction> GetUpcomingTransactions(int days = 7)
        {
            var cutoffDate = DateTime.Now.AddDays(days);
            return _recurringTransactions
                .Where(rt => rt.IsActive && !rt.IsPaused && rt.NextDate <= cutoffDate)
                .OrderBy(rt => rt.NextDate)
                .ToList();
        }
        public bool ProcessSingleTransaction(Guid id)
        {
            var transaction = GetRecurringTransaction(id);
            if (transaction == null || !transaction.ShouldProcessToday())
                return false;

            // Создаем фактическую транзакцию
            TransactionFactory factory = transaction.Type == TransactionType.Income
                ? new IncomeFactory()
                : new ExpenseFactory();

            var newTransaction = factory.CreateTransaction(
                transaction.Amount,
                transaction.Category,
                $"{transaction.Name} (автоматически)"
            );

            // Добавляем в кошелек
            _wallet.AddTransaction(newTransaction);

            // Обновляем дату следующего выполнения
            transaction.LastProcessedDate = DateTime.Now;
            transaction.NextDate = transaction.CalculateNextDate();

            UpdateRecurringTransaction(transaction);
            return true;
        }
    }
}