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
    public class MainViewModel : ViewModelBase
    {
        private readonly AppWallet _wallet;
        private decimal _balance;
        private decimal _totalIncome;
        private decimal _totalExpense;
        private int _transactionCount;

        public decimal Balance
        {
            get => _balance;
            set => SetField(ref _balance, value);
        }

        public decimal TotalIncome
        {
            get => _totalIncome;
            set => SetField(ref _totalIncome, value);
        }

        public decimal TotalExpense
        {
            get => _totalExpense;
            set => SetField(ref _totalExpense, value);
        }

        public int TransactionCount
        {
            get => _transactionCount;
            set => SetField(ref _transactionCount, value);
        }

        public ObservableCollection<TransactionViewModel> Transactions { get; }

        // Команды
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand OpenGoalsCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            _wallet = AppWallet.Instance;
            Transactions = new ObservableCollection<TransactionViewModel>();

            // Инициализация команд
            AddIncomeCommand = new RelayCommand(AddIncome);
            AddExpenseCommand = new RelayCommand(AddExpense);
            OpenGoalsCommand = new RelayCommand(OpenGoals);
            RefreshCommand = new RelayCommand(Refresh);

            LoadData();
        }

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

        private void AddIncome(object? parameter)
        {
            var factory = new IncomeFactory();
            var transaction = factory.CreateTransaction(1000, "Доход", "Тестовый доход");
            _wallet.AddTransaction(transaction);
            LoadData();
        }

        private void AddExpense(object? parameter)
        {
            var factory = new ExpenseFactory();
            var transaction = factory.CreateTransaction(500, "Расход", "Тестовый расход");
            _wallet.AddTransaction(transaction);
            LoadData();
        }

        private void OpenGoals(object? parameter)
        {
            System.Windows.MessageBox.Show("Цели - в разработке", "Информация");
        }

        private void Refresh(object? parameter)
        {
            LoadData();
            System.Windows.MessageBox.Show("Данные обновлены", "Обновление");
        }
    }
}