using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    /// <summary>
    /// Фабрика для создания переводов
    /// </summary>
    public class TransferFactory : TransactionFactory
    {
        public override Transaction CreateTransaction(
            decimal amount,
            string category,
            string description = "")
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма перевода должна быть положительной", nameof(amount));

            return new Transaction
            {
                Type = TransactionType.Expense, // Перевод считаем расходом
                Amount = amount,
                Category = "Перевод",
                Description = string.IsNullOrEmpty(description)
                    ? $"Перевод: {category}"
                    : description,
                Date = DateTime.Now
            };
        }

        // Добавьте эти методы:
        public override TransactionType GetTransactionType()
            => TransactionType.Expense;

        public override string GetDisplayName()
            => "Перевод";

        public override string GetColor()
            => "#2196F3"; // Синий

        public override string GetIcon()
            => "🔄";
    }
}