using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    public class ExpenseFactory : TransactionFactory
    {
        public override Transaction CreateTransaction(
            decimal amount,
            string category,
            string description = "")
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма расхода должна быть положительной", nameof(amount));

            return new Transaction
            {
                Type = TransactionType.Expense,
                Amount = amount,
                Category = category ?? "Расход",
                Description = string.IsNullOrEmpty(description)
                    ? $"Расход: {category}"
                    : description,
                Date = DateTime.Now
            };
        }

        // Добавьте эти методы:
        public override TransactionType GetTransactionType()
            => TransactionType.Expense;

        public override string GetDisplayName()
            => "Расход";

        public override string GetColor()
            => "#F44336"; // Красный

        public override string GetIcon()
            => "💸";
    }
}