using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyTracker.Core.Patterns.Singleton
{
    public sealed class AppWallet
    {
        private static AppWallet _instance;
        private static readonly object _lock = new object();

        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        private AppWallet()
        {
            Transactions = FileManager.LoadTransactions();
            CalculateBalance();
        }

        public static AppWallet Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppWallet();
                    }
                    return _instance;
                }
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);

            if (transaction.Type == TransactionType.Income)
                Balance += transaction.Amount;
            else
                Balance -= transaction.Amount;

            FileManager.SaveTransactions(Transactions); // Сохраняем в файл
        }

        private void CalculateBalance()
        {
            Balance = 0;
            foreach (var transaction in Transactions)
            {
                if (transaction.Type == TransactionType.Income)
                    Balance += transaction.Amount;
                else
                    Balance -= transaction.Amount;
            }
        }

        public List<Transaction> GetTransactions()
        {
            return Transactions
                .OrderByDescending(t => t.Date)
                .ToList();
        }

        public decimal GetTotalIncome()
        {
            return Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        public decimal GetTotalExpense()
        {
            return Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
        }
        public void Clear()
        {
            Transactions.Clear();
            Balance = 0;
        }
        public void Reset()
        {
            Transactions.Clear();
            Balance = 0; // Можно внутри класса
            FileManager.SaveTransactions(Transactions); // Если есть
        }
    }
}