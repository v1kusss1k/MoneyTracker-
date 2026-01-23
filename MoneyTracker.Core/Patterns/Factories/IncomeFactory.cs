using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    public class IncomeFactory : TransactionFactory
    {
        public override Transaction CreateTransaction(
            decimal amount,
            string category,
            string description = "")
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма дохода должна быть положительной", nameof(amount));

            return new Transaction
            {
                Type = TransactionType.Income,
                Amount = amount,
                Category = category ?? "Доход",
                Description = string.IsNullOrEmpty(description)
                    ? $"Доход: {category}"
                    : description,
                Date = DateTime.Now
            };
        }

        // Добавьте эти методы:
        public override TransactionType GetTransactionType()
            => TransactionType.Income;

        public override string GetDisplayName()
            => "Доход";

        public override string GetColor()
            => "#4CAF50"; // Зелёный

        public override string GetIcon()
            => "💰";
    }
}