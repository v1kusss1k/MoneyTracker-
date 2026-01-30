using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using System;

namespace MoneyTracker.App.ViewModels
{
    // для отображения транзакции в интерфейсе, преобразует модель транзакции в удобный для отображения формат
    public class TransactionViewModel : ViewModelBase
    {
        private readonly Transaction _transaction;

        // свойства транзакции для привязки в xaml
        public Guid Id => _transaction.Id;
        public string Type => _transaction.Type.ToString();

        // отображаемое название типа (русский)
        public string TypeDisplay => _transaction.Type == TransactionType.Income ? "Доход" : "Расход";

        public decimal Amount => _transaction.Amount;

        // отформатированная сумма с валютой
        public string AmountFormatted => $"{Amount:N0} ₽";

        public string Category => _transaction.Category;
        public DateTime Date => _transaction.Date;

        // отформатированная дата
        public string DateFormatted => Date.ToString("dd.MM.yyyy");

        public string Description => _transaction.Description;

        // иконка в зависимости от типа
        public string Icon => _transaction.Type == TransactionType.Income ? "💰" : "💸";

        // цвет в зависимости от типа
        public string Color => _transaction.Type == TransactionType.Income ? "Green" : "Red";

        // конструктор с проверкой null
        public TransactionViewModel(Transaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}