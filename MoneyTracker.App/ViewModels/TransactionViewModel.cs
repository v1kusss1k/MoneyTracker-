using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using System;

namespace MoneyTracker.App.ViewModels
{
    public class TransactionViewModel : ViewModelBase
    {
        private readonly Transaction _transaction;

        public Guid Id => _transaction.Id;
        public string Type => _transaction.Type.ToString();
        public string TypeDisplay => _transaction.Type == TransactionType.Income ? "Доход" : "Расход";
        public decimal Amount => _transaction.Amount;
        public string AmountFormatted => $"{Amount:N0} ₽";
        public string Category => _transaction.Category;
        public DateTime Date => _transaction.Date;
        public string DateFormatted => Date.ToString("dd.MM.yyyy");
        public string Description => _transaction.Description;
        public string Icon => _transaction.Type == TransactionType.Income ? "💰" : "💸";
        public string Color => _transaction.Type == TransactionType.Income ? "Green" : "Red";

        public TransactionViewModel(Transaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}