using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    /// <summary>
    /// Класс-создатель фабрик (Factory Method паттерн)
    /// Позволяет создавать нужную фабрику по типу транзакции
    /// </summary>
    public static class TransactionFactoryCreator
    {
        /// <summary>
        /// Создаёт фабрику для указанного типа транзакции
        /// </summary>
        public static TransactionFactory CreateFactory(TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => new IncomeFactory(),
                TransactionType.Expense => new ExpenseFactory(),
                _ => throw new ArgumentException($"Неизвестный тип транзакции: {type}", nameof(type))
            };
        }

        /// <summary>
        /// Создаёт фабрику по строковому названию типа
        /// </summary>
        public static TransactionFactory CreateFactory(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentException("Название типа не может быть пустым", nameof(typeName));

            return typeName.ToLower() switch
            {
                "income" or "доход" => new IncomeFactory(),
                "expense" or "расход" => new ExpenseFactory(),
                "transfer" or "перевод" => new TransferFactory(),
                _ => throw new ArgumentException($"Неизвестный тип транзакции: {typeName}", nameof(typeName))
            };
        }

        /// <summary>
        /// Получает все доступные типы фабрик
        /// </summary>
        public static string[] GetAvailableFactoryTypes()
        {
            return new string[]
            {
                "Income",
                "Expense",
                "Transfer"
            };
        }
    }
}