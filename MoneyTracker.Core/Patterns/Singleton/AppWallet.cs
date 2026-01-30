using MoneyTracker.Core.Models;
using MoneyTracker.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyTracker.Core.Patterns.Singleton
{
    // один экземпляр кошелька на всё приложение, гарантирует единую точку доступа к данным о финансах
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

        // свойство для доступа к единственному экземпляру
        public static AppWallet Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppWallet();                             //создается единственный экземпляр
                    }
                    return _instance;
                }
            }
        }

        public void AddTransaction(Transaction transaction)       // добавление новой транзакции
        {
            Transactions.Add(transaction);

            if (transaction.Type == TransactionType.Income)
                Balance += transaction.Amount;
            else
                Balance -= transaction.Amount;

            FileManager.SaveTransactions(Transactions);           // автосохранение
        }

        public bool RemoveTransaction(Guid transactionId)
        {
            var transaction = Transactions.FirstOrDefault(t => t.Id == transactionId);
            if (transaction == null)
                return false;

            // Пересчитываем баланс
            if (transaction.Type == TransactionType.Income)
                Balance -= transaction.Amount;
            else
                Balance += transaction.Amount;

            Transactions.Remove(transaction);
            FileManager.SaveTransactions(Transactions);
            return true;
        }
        private void CalculateBalance()                          // пересчет баланса
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

        public decimal GetTotalIncome()                                 // общий доход
        {
            return Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
        }

        public decimal GetTotalExpense()                               // общий расход
        {
            return Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
        }
        public void Clear()                                           // очистка всех данных
        {
            Transactions.Clear();
            Balance = 0;
        }
    }
}