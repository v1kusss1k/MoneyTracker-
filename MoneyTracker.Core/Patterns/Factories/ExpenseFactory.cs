using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    // конкретная фабрика для создания расходов
    public class ExpenseFactory : TransactionFactory
    {
        // создание транзакции расхода
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
                Description = description, // ← ПРОСТО description
                Date = DateTime.Now
            };
        }

        // расход
        public override TransactionType GetTransactionType()
            => TransactionType.Expense;

        // отображаемое имя
        public override string GetDisplayName()
            => "Расход";

        // красный цвет для расходов
        public override string GetColor()
            => "#F44336";

        // иконка 
        public override string GetIcon()
            => "💸";
    }
}