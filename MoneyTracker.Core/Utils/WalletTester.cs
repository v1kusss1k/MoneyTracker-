using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using System;

namespace MoneyTracker.Core.Utils
{
    public static class WalletTester
    {
        public static void Test()
        {
            Console.WriteLine("🧪 Тестирование паттернов...");

            // Тест Singleton
            var wallet1 = AppWallet.Instance;
            var wallet2 = AppWallet.Instance;

            Console.WriteLine($"Один и тот же кошелёк? {ReferenceEquals(wallet1, wallet2)}");

            // Тест Abstract Factory
            var incomeFactory = new IncomeFactory();
            var expenseFactory = new ExpenseFactory();

            // Создаём транзакции через фабрики
            var income = incomeFactory.CreateTransaction(1000, "Зарплата", "Зарплата за январь");
            var expense = expenseFactory.CreateTransaction(500, "Продукты", "Покупка в магазине");

            // Добавляем в кошелёк
            wallet1.AddTransaction(income);
            wallet1.AddTransaction(expense);

            Console.WriteLine($"Баланс: {wallet1.Balance}");
            Console.WriteLine($"Всего транзакций: {wallet1.Transactions.Count}");

            // Тест Factory Creator
            var factory = TransactionFactoryCreator.CreateFactory("income");
            Console.WriteLine($"Создана фабрика: {factory.GetDisplayName()}");

            Console.WriteLine("✅ Тестирование завершено!");
        }
    }
}