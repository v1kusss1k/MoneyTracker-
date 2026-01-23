using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    /// <summary>
    /// Абстрактная фабрика для создания транзакций
    /// </алис>
    public abstract class TransactionFactory
    {
        /// <summary>
        /// Создаёт транзакцию указанного типа
        /// </summary>
        public abstract Transaction CreateTransaction(
            decimal amount,
            string category,
            string description = "");

        /// <summary>
        /// Возвращает тип транзакции
        /// </summary>
        public abstract TransactionType GetTransactionType();

        /// <summary>
        /// Возвращает название типа (для отображения)
        /// </summary>
        public abstract string GetDisplayName();

        /// <summary>
        /// Возвращает цвет для UI
        /// </summary>
        public abstract string GetColor();

        /// <summary>
        /// Возвращает иконку
        /// </summary>
        public abstract string GetIcon();
    }
}