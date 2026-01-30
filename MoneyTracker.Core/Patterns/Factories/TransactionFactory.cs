using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Patterns.Factories
{
    // абстрактная фабрика для создания транзакций, определяет общий интерфейс для создания объектов
    public abstract class TransactionFactory
    {
        // абстрактный метод создания транзакции
        public abstract Transaction CreateTransaction(
            decimal amount,
            string category,
            string description = "");

        // тип транзакции
        public abstract TransactionType GetTransactionType();

        // название для отображения
        public abstract string GetDisplayName();

        // цвет для ui
        public abstract string GetColor();

        // иконка
        public abstract string GetIcon();
    }
}