using MoneyTracker.Core.Enums;
using System;

namespace MoneyTracker.Core.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public Transaction()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            Category = "";
            Description = "";
        }
    }
}