#nullable disable

using MoneyTracker.Core.Models;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoneyTracker.Core.Services
{
    public class NotificationService
    {
        private readonly AppWallet _wallet;
        private readonly BudgetService _budgetService;
        private readonly GoalService _goalService;
        private readonly RecurringService _recurringService;

        private AppSettings _settings;

        public class AppSettings
        {
            public bool EnableBudgetAlerts { get; set; } = true;
            public bool EnableGoalReminders { get; set; } = true;
            public bool EnableRecurringReminders { get; set; } = true;
            public bool EnableLargeExpenseAlerts { get; set; } = true;
            public decimal LargeExpenseThreshold { get; set; } = 10000;
            public bool AutoSave { get; set; } = true;
            public bool StartMinimized { get; set; } = false;
            public bool ShowWelcome { get; set; } = true;
            public string Theme { get; set; } = "light";
        }

        public NotificationService()
        {
            _wallet = AppWallet.Instance;
            _budgetService = new BudgetService();
            _goalService = new GoalService();
            _recurringService = new RecurringService();
            _settings = LoadAppSettings();
        }

        private AppSettings LoadAppSettings()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MoneyTracker", "settings.json");

                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }

            return new AppSettings();
        }

        public class Notification
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public NotificationType Type { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRead { get; set; }
            public string Icon { get; set; } = "🔔";
            public string Color { get; set; } = "#FF9800";

            public Notification()
            {
                Id = Guid.NewGuid();
                CreatedAt = DateTime.Now;
                IsRead = false;
            }

            // Конструктор для десериализации
            [JsonConstructor]
            public Notification(Guid id, string title, string message, NotificationType type,
                               DateTime createdAt, bool isRead, string icon, string color)
            {
                Id = id;
                Title = title ?? string.Empty;
                Message = message ?? string.Empty;
                Type = type;
                CreatedAt = createdAt;
                IsRead = isRead;
                Icon = icon ?? "🔔";
                Color = color ?? "#FF9800";
            }
        }

        public enum NotificationType
        {
            Info,
            Warning,
            Alert,
            Success
        }

        public List<Notification> CheckAllNotifications()
        {
            var notifications = new List<Notification>();

            if (_settings.EnableBudgetAlerts)
                notifications.AddRange(CheckBudgetNotifications());

            if (_settings.EnableGoalReminders)
                notifications.AddRange(CheckGoalNotifications());

            if (_settings.EnableRecurringReminders)
                notifications.AddRange(CheckRecurringNotifications());

            if (_settings.EnableLargeExpenseAlerts)
                notifications.AddRange(CheckLargeExpenses());

            return notifications.OrderByDescending(n => n.CreatedAt).ToList();
        }

        private List<Notification> CheckBudgetNotifications()
        {
            var notifications = new List<Notification>();
            var budgets = _budgetService.GetCurrentBudgetsWithTracking();

            foreach (var budget in budgets)
            {
                if (budget.IsExceeded)
                {
                    notifications.Add(new Notification
                    {
                        Title = "💰 Превышен бюджет!",
                        Message = $"Бюджет '{budget.Budget.Category}' превышен на {budget.Spent - budget.Budget.MonthlyLimit:N0}₽",
                        Type = NotificationType.Alert,
                        Icon = "❌",
                        Color = "#F44336"
                    });
                }
                else if (budget.IsWarning)
                {
                    notifications.Add(new Notification
                    {
                        Title = "⚠️ Близко к лимиту",
                        Message = $"Бюджет '{budget.Budget.Category}' использован на {budget.Percentage:F0}%",
                        Type = NotificationType.Warning,
                        Icon = "⚠️",
                        Color = "#FF9800"
                    });
                }
            }

            return notifications;
        }

        private List<Notification> CheckGoalNotifications()
        {
            var notifications = new List<Notification>();
            var goals = _goalService.GetGoals().Where(g => !g.IsArchived);

            foreach (var goal in goals)
            {
                if (goal.IsCompleted && !goal.WasNotified)
                {
                    notifications.Add(new Notification
                    {
                        Title = "🎉 Цель достигнута!",
                        Message = $"Поздравляем! Цель '{goal.Name}' достигнута!",
                        Type = NotificationType.Success,
                        Icon = "✅",
                        Color = "#4CAF50"
                    });

                    goal.WasNotified = true;
                    _goalService.UpdateGoal(goal);
                }
                else if (goal.DaysRemaining <= 7 && goal.DaysRemaining > 0)
                {
                    notifications.Add(new Notification
                    {
                        Title = "⏰ Срок цели истекает",
                        Message = $"До цели '{goal.Name}' осталось {goal.DaysRemaining} дней",
                        Type = NotificationType.Warning,
                        Icon = "⏰",
                        Color = "#FF9800"
                    });
                }
                else if (goal.DaysRemaining < 0)
                {
                    notifications.Add(new Notification
                    {
                        Title = "📅 Цель просрочена",
                        Message = $"Цель '{goal.Name}' просрочена на {-goal.DaysRemaining} дней",
                        Type = NotificationType.Alert,
                        Icon = "📅",
                        Color = "#F44336"
                    });
                }
            }

            return notifications;
        }

        private List<Notification> CheckRecurringNotifications()
        {
            var notifications = new List<Notification>();
            var recurring = _recurringService.GetAllRecurringTransactions();

            foreach (var transaction in recurring)
            {
                if (transaction.NextDate.Date == DateTime.Today)
                {
                    notifications.Add(new Notification
                    {
                        Title = "🔄 Регулярный платеж сегодня",
                        Message = $"Сегодня '{transaction.Name}' на сумму {transaction.Amount:N0}₽",
                        Type = NotificationType.Info,
                        Icon = "🔄",
                        Color = "#2196F3"
                    });
                }
                else if (transaction.NextDate.Date == DateTime.Today.AddDays(1))
                {
                    notifications.Add(new Notification
                    {
                        Title = "⏰ Регулярный платеж завтра",
                        Message = $"Завтра '{transaction.Name}' на сумму {transaction.Amount:N0}₽",
                        Type = NotificationType.Info,
                        Icon = "⏰",
                        Color = "#2196F3"
                    });
                }
            }

            return notifications;
        }

        private List<Notification> CheckLargeExpenses()
        {
            var notifications = new List<Notification>();
            var today = DateTime.Today;

            decimal threshold = _settings.LargeExpenseThreshold;

            var todayExpenses = _wallet.Transactions
                .Where(t => t.Type == MoneyTracker.Core.Enums.TransactionType.Expense &&
                           t.Date.Date == today &&
                           t.Amount >= threshold)
                .ToList();

            foreach (var expense in todayExpenses)
            {
                notifications.Add(new Notification
                {
                    Title = "💸 Крупный расход",
                    Message = $"Сегодня крупный расход: {expense.Category} - {expense.Amount:N0}₽",
                    Type = NotificationType.Warning,
                    Icon = "💸",
                    Color = "#F44336"
                });
            }

            return notifications;
        }

        // Убрали CheckOnStartup - слишком сложно

        public void SaveNotificationsToFile(List<Notification> notifications)
        {
            try
            {
                var filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MoneyTracker", "notifications.json");

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Фильтруем старые уведомления (старше 30 дней)
                var cutoffDate = DateTime.Now.AddDays(-30);
                var notificationsToSave = notifications
                    .Where(n => n.CreatedAt >= cutoffDate)
                    .ToList();

                // Сохраняем максимум 100 уведомлений
                if (notificationsToSave.Count > 100)
                {
                    notificationsToSave = notificationsToSave
                        .OrderByDescending(n => n.CreatedAt)
                        .Take(100)
                        .ToList();
                }

                var json = JsonSerializer.Serialize(notificationsToSave,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                Console.WriteLine($"Сохранено {notificationsToSave.Count} уведомлений");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения уведомлений: {ex.Message}");
            }
        }

        public List<Notification> LoadNotificationsFromFile()
        {
            try
            {
                var filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MoneyTracker", "notifications.json");

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Если файла нет - создаем пустой
                if (!File.Exists(filePath))
                {
                    var emptyList = new List<Notification>();
                    var json = JsonSerializer.Serialize(emptyList,
                        new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    return emptyList;
                }

                var fileContent = File.ReadAllText(filePath);

                // Проверяем, что файл не пустой
                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    var emptyList = new List<Notification>();
                    var json = JsonSerializer.Serialize(emptyList,
                        new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    return emptyList;
                }

                return JsonSerializer.Deserialize<List<Notification>>(fileContent) ?? new List<Notification>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки уведомлений: {ex.Message}");

                // Создаем новый пустой файл при ошибке
                try
                {
                    var filePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "MoneyTracker", "notifications.json");

                    var dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var emptyList = new List<Notification>();
                    var json = JsonSerializer.Serialize(emptyList,
                        new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                }
                catch { }

                return new List<Notification>();
            }
        }

        public static AppSettings LoadAppSettingsStatic()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MoneyTracker", "settings.json");

                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }

            return new AppSettings();
        }

        // Простой метод для проверки есть ли уведомления
        public bool HasNotifications()
        {
            var notifications = CheckAllNotifications();
            return notifications.Any();
        }

        // Получить количество уведомлений
        public int GetNotificationCount()
        {
            var notifications = CheckAllNotifications();
            return notifications.Count;
        }
    }
}