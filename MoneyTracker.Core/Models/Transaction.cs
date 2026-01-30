using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Models
{
    // модель транзакции 
    public class Transaction
    {
        // уникальный идентификатор
        public Guid Id { get; set; }

        // тип транзакции 
        public TransactionType Type { get; set; }

        // сумма
        public decimal Amount { get; set; }

        // категория
        public string Category { get; set; }

        // дата операции
        public DateTime Date { get; set; }

        // описание
        public string Description { get; set; }

        // конструктор по умолчанию
        public Transaction()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            Category = "";
            Description = "";
        }

        // конструктор с параметрами
        public Transaction(TransactionType type, decimal amount, string category, string description = "")
        {
            Id = Guid.NewGuid();
            Type = type;
            Amount = amount;
            Category = category;
            Description = description;
            Date = DateTime.Now;
        }
    }
}