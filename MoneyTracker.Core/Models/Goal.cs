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

        public Goal()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            TargetDate = DateTime.Now.AddMonths(6);
            Name = string.Empty;
            Description = string.Empty;
        }

        public Goal(string name, string description, decimal targetAmount) : this()
        {
            Name = name;
            Description = description;
            TargetAmount = targetAmount;
        }

        // Добавьте эти свойства:
        public decimal ProgressPercentage
        {
            get
            {
                if (TargetAmount <= 0) return 0;
                return Math.Min(100, (CurrentAmount / TargetAmount) * 100);
            }
        }

        public decimal RemainingAmount => TargetAmount - CurrentAmount;

        public string ProgressText => $"{ProgressPercentage:F1}% ({CurrentAmount:N0}₽ из {TargetAmount:N0}₽)";

        // Добавьте этот метод:
        public void AddAmount(decimal amount)
        {
            CurrentAmount += amount;
            if (CurrentAmount >= TargetAmount)
            {
                CurrentAmount = TargetAmount;
                IsCompleted = true;
            }
        }
    }
}