// MoneyTracker.Core/Models/Goal.cs
using System;

namespace MoneyTracker.Core.Models
{
    public class Goal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsArchived { get; set; }
        public bool WasNotified { get; set; }

        public Goal()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            TargetDate = DateTime.Now.AddMonths(3);
            Name = "Новая цель";
            Description = "";
            Icon = "🎯";
            Color = "#FF9800";
            IsArchived = false;
            CurrentAmount = 0;
            WasNotified = false;
        }

        public Goal(string name, string description, decimal targetAmount) : this()
        {
            Name = name;
            Description = description;
            TargetAmount = targetAmount;
        }

        public decimal ProgressPercentage
        {
            get
            {
                if (TargetAmount <= 0) return 0;
                return Math.Min(100, (CurrentAmount / TargetAmount) * 100);
            }
        }

        public decimal RemainingAmount => TargetAmount - CurrentAmount;

        public string ProgressText => $"{CurrentAmount:N0}₽ / {TargetAmount:N0}₽";

        public int DaysRemaining => Math.Max(0, (TargetDate - DateTime.Now).Days);

        public string Status
        {
            get
            {
                if (IsCompleted) return "✅ Завершена";
                if (CurrentAmount == 0) return "🆕 Новая";
                return "⏳ В процессе";
            }
        }

        public void AddAmount(decimal amount)
        {
            if (amount <= 0) return;

            // УПРОЩАЕМ: убираем wasCompletedBefore
            CurrentAmount += amount;
            if (CurrentAmount >= TargetAmount)
            {
                CurrentAmount = TargetAmount;
                IsCompleted = true;
                // При достижении цели сбрасываем флаг уведомления
                WasNotified = false;
            }
        }
    }
}