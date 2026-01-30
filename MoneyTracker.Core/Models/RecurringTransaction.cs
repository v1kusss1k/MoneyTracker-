// MoneyTracker.Core/Models/RecurringTransaction.cs
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Models
{
    public class RecurringTransaction
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; }

        // Периодичность
        public RecurrenceType Recurrence { get; set; }
        public int DayOfMonth { get; set; } // Для месячных: день месяца (1-31)
        public DayOfWeek DayOfWeek { get; set; } // Для недельных: день недели

        // Даты
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime NextDate { get; set; }

        // Статус
        public bool IsActive { get; set; }
        public bool IsPaused { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastProcessedDate { get; set; }

        public RecurringTransaction()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            StartDate = DateTime.Now;
            NextDate = CalculateNextDate();
            IsActive = true;
            Name = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
        }

        public RecurringTransaction(string name, decimal amount, TransactionType type, string category,
                                  RecurrenceType recurrence) : this()
        {
            Name = name;
            Amount = amount;
            Type = type;
            Category = category;
            Recurrence = recurrence;
        }

        public DateTime CalculateNextDate()
        {
            var now = DateTime.Now;
            var nextDate = NextDate < now ? now : NextDate;

            switch (Recurrence)
            {
                case RecurrenceType.Daily:
                    return nextDate.AddDays(1);

                case RecurrenceType.Weekly:
                    // Добавляем неделю
                    return nextDate.AddDays(7);

                case RecurrenceType.Monthly:
                    // Добавляем месяц, корректируем день
                    nextDate = nextDate.AddMonths(1);
                    if (DayOfMonth > 0)
                    {
                        // Проверяем, чтобы день был валидным для месяца
                        int daysInMonth = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
                        int actualDay = Math.Min(DayOfMonth, daysInMonth);
                        return new DateTime(nextDate.Year, nextDate.Month, actualDay);
                    }
                    return nextDate;

                case RecurrenceType.Yearly:
                    return nextDate.AddYears(1);

                default:
                    return nextDate.AddMonths(1);
            }
        }

        public string RecurrenceText
        {
            get
            {
                return Recurrence switch
                {
                    RecurrenceType.Daily => "Ежедневно",
                    RecurrenceType.Weekly => $"Еженедельно ({DayOfWeekToString()})",
                    RecurrenceType.Monthly => $"Ежемесячно ({DayOfMonth}-го числа)",
                    RecurrenceType.Yearly => $"Ежегодно ({StartDate:dd.MM})",
                    _ => "Разово"
                };
            }
        }

        private string DayOfWeekToString()
        {
            return DayOfWeek switch
            {
                DayOfWeek.Monday => "понедельник",
                DayOfWeek.Tuesday => "вторник",
                DayOfWeek.Wednesday => "среда",
                DayOfWeek.Thursday => "четверг",
                DayOfWeek.Friday => "пятница",
                DayOfWeek.Saturday => "суббота",
                DayOfWeek.Sunday => "воскресенье",
                _ => ""
            };
        }

        public string NextDateText => NextDate.ToString("dd.MM.yyyy");

        public bool ShouldProcessToday()
        {
            if (!IsActive || IsPaused) return false;
            if (NextDate.Date <= DateTime.Today &&
                (!EndDate.HasValue || DateTime.Today <= EndDate.Value.Date))
            {
                return true;
            }
            return false;
        }
    }

    public enum RecurrenceType
    {
        Monthly = 0,
        Weekly = 1,
        Daily = 2,
        Yearly = 3
    }
}