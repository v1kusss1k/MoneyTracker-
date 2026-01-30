using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Enums;
using MoneyTracker.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MoneyTracker.App.ViewModels
{
    // основная viewmodel приложения, связывает модель с представлением 
    public class MainViewModel : ViewModelBase
    {
        private readonly AppWallet _wallet;                // singleton экземпляр
        private decimal _balance;
        private decimal _totalIncome;
        private decimal _totalExpense;
        private int _transactionCount;

        // текущий баланс
        public decimal Balance
        {
            get => _balance;
            set => SetField(ref _balance, value);
        }

        // общий доход
        public decimal TotalIncome
        {
            get => _totalIncome;
            set => SetField(ref _totalIncome, value);
        }

        // общий расход
        public decimal TotalExpense
        {
            get => _totalExpense;
            set => SetField(ref _totalExpense, value);
        }

        // количество транзакций
        public int TransactionCount
        {
            get => _transactionCount;
            set => SetField(ref _transactionCount, value);
        }

        // коллекция транзакций для отображения
        public ObservableCollection<TransactionViewModel> Transactions { get; }

        // команды для кнопок
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand OpenGoalsCommand { get; }

        public MainViewModel()
        {
            _wallet = AppWallet.Instance;                                            // получаем singleton
            Transactions = new ObservableCollection<TransactionViewModel>();

            // инициализация команд
            AddIncomeCommand = new RelayCommand(AddIncome);
            AddExpenseCommand = new RelayCommand(AddExpense);
            OpenGoalsCommand = new RelayCommand(OpenGoals);
            LoadData();
        }

        // загрузка данных из кошелька
        public void LoadData()
        {
            Transactions.Clear();

            foreach (var transaction in _wallet.GetTransactions())
            {
                Transactions.Add(new TransactionViewModel(transaction));
            }

            Balance = _wallet.Balance;
            TotalIncome = _wallet.GetTotalIncome();
            TotalExpense = _wallet.GetTotalExpense();
            TransactionCount = _wallet.Transactions.Count;
        }

        // добавление дохода через фабрику
        private void AddIncome(object? parameter)
        {
            var factory = new IncomeFactory(); // создаем фабрику доходов
            var transaction = factory.CreateTransaction(1000, "Доход", "тестовый доход");
            _wallet.AddTransaction(transaction);
            LoadData();
        }

        // добавление расхода через фабрику
        private void AddExpense(object? parameter)
        {
            var factory = new ExpenseFactory(); // создаем фабрику расходов
            var transaction = factory.CreateTransaction(500, "Расход", "тестовый расход");
            _wallet.AddTransaction(transaction);
            LoadData();
        }

        // открытие окна целей
        private void OpenGoals(object? parameter)
        {
            System.Windows.MessageBox.Show(" ");
        }
    }
}