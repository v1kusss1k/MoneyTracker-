using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    // конкретная фабрика для создания доходов, реализация абстрактной фабрики
    public class IncomeFactory : TransactionFactory
    {
        // создание транзакции дохода
        // IncomeFactory.cs
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
                Description = description, // ← ПРОСТО description, без автоматического текста
                Date = DateTime.Now
            };
        }

        // тип - доход
        public override TransactionType GetTransactionType()
            => TransactionType.Income;

        // отображаемое имя
        public override string GetDisplayName()
            => "Доход";

        // зеленый цвет для доходов
        public override string GetColor()
            => "#4CAF50";

        // иконка 
        public override string GetIcon()
            => "💰";
    }
}