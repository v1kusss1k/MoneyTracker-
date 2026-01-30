#nullable disable

using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using System;
using System.Windows.Media;

namespace MoneyTracker.App.ViewModels
{
    public class RecurringViewModel : ViewModelBase
    {
        private readonly RecurringTransaction _transaction;

        public Guid Id => _transaction.Id;
        public string Name => _transaction.Name;
        public string Description => _transaction.Description;
        public decimal Amount => _transaction.Amount;
        public string AmountFormatted => $"{(_transaction.Type == TransactionType.Expense ? "-" : "+")}{Amount:N0} ₽";
        public string Type => _transaction.Type.ToString();
        public string TypeDisplay => _transaction.Type == TransactionType.Income ? "Доход" : "Расход";
        public string Category => _transaction.Category;
        public string RecurrenceText => _transaction.RecurrenceText;
        public string NextDateText => _transaction.NextDateText;
        public bool IsActive => _transaction.IsActive;
        public bool IsPaused => _transaction.IsPaused;
        public DateTime NextDate => _transaction.NextDate;

        // Статус выполнения
        public bool IsDue => _transaction.NextDate.Date <= DateTime.Today;
        public string Status => IsDue ? "⏰ ВЫПОЛНИТЬ" : "📅 ОЖИДАЕТ";

        // Цвета
        public string Color => _transaction.Type == TransactionType.Income ? "#4CAF50" : "#F44336";
        public Brush StatusColor => IsDue ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.Teal);
        public string Icon => _transaction.Type == TransactionType.Income ? "💰" : "💸";

        public RecurringViewModel(RecurringTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}