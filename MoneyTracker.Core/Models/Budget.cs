// MoneyTracker.Core/Models/Budget.cs
using System;

namespace MoneyTracker.Core.Models
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Category { get; set; } // Категория расхода
        public decimal MonthlyLimit { get; set; } // Лимит на месяц
        public int Year { get; set; } // Год бюджета
        public int Month { get; set; } // Месяц бюджета (1-12)
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Расчетные свойства
        public DateTime BudgetMonth => new DateTime(Year, Month, 1);
        public string MonthName => BudgetMonth.ToString("MMMM yyyy");

        public Budget()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            var now = DateTime.Now;
            Year = now.Year;
            Month = now.Month;
            IsActive = true;
            Category = string.Empty;
        }

        public Budget(string category, decimal monthlyLimit) : this()
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MonthlyLimit = monthlyLimit;
        }

        // Для отображения
        public override string ToString() => $"{Category}: {MonthlyLimit:N0}₽/мес";
    }
}