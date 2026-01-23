using MoneyTracker.App.ViewModels;
using MoneyTracker.Core.Patterns.Singleton;
using System;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class MainWindow : Window
    {
        private AppWallet _wallet;

        public MainWindow()
        {
            InitializeComponent();

            // Обработчики
            btnAddIncome.Click += btnAddIncome_Click;
            btnAddExpense.Click += btnAddExpense_Click;
            btnGoals.Click += (s, e) => new GoalsWindow().ShowDialog();
            btnRefresh.Click += (s, e) => UpdateDisplay();
            btnClearAll.Click += btnClearAll_Click;
            btnReports.Click += (s, e) => new ReportsWindow().ShowDialog();
            btnSettings.Click += (s, e) => new SettingsWindow().ShowDialog();

            _wallet = AppWallet.Instance;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            try
            {
                txtBalance.Text = $"{_wallet.Balance:N0} ₽";
                txtIncomeTotal.Text = $"Доходы: {_wallet.GetTotalIncome():N0} ₽";
                txtExpenseTotal.Text = $"Расходы: {_wallet.GetTotalExpense():N0} ₽";
                txtTransactionCount.Text = $"Операций: {_wallet.Transactions.Count}";

                // Обновляем статус
                txtStatus.Text = $"Баланс: {_wallet.Balance:N0} ₽ | Операций: {_wallet.Transactions.Count}";

                var viewModels = new System.Collections.ObjectModel.ObservableCollection<TransactionViewModel>();
                foreach (var transaction in _wallet.GetTransactions())
                {
                    viewModels.Add(new TransactionViewModel(transaction));
                }

                lstTransactions.ItemsSource = viewModels;
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }

        private void btnAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddIncomeWindow();
            window.TransactionAdded += (s, args) => UpdateDisplay();
            window.ShowDialog();
        }

        private void btnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddExpenseWindow();
            window.TransactionAdded += (s, args) => UpdateDisplay();
            window.ShowDialog();
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить ВСЕ операции?\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _wallet.Transactions.Clear();
                _wallet.Clear(); // Убедитесь, что этот метод есть в AppWallet
                UpdateDisplay();
                txtStatus.Text = "Все операции удалены";
            }
        }
    }
}