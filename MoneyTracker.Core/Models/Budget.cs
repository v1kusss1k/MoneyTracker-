// MoneyTracker.Core/Models/Budget.cs
using System;

namespace MoneyTracker.Core.Models
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty; // Инициализируем
        public decimal MonthlyLimit { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
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
            Category = string.Empty; // Явная инициализация
        }

        public Budget(string category, decimal monthlyLimit) : this()
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MonthlyLimit = monthlyLimit;
        }

        public override string ToString()
        {
            var monthName = new DateTime(Year, Month, 1).ToString("MMMM yyyy");
            return $"{Category}: {MonthlyLimit:N0}₽ ({monthName})";
        }
    }
}