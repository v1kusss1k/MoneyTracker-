using MoneyTracker.App.ViewModels;
using MoneyTracker.Core.Patterns.Singleton;
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
            btnAddIncome.Click += (s, e) => ShowAddTransactionWindow("Income");
            btnAddExpense.Click += (s, e) => ShowAddTransactionWindow("Expense");
            btnGoals.Click += (s, e) => new GoalsWindow().ShowDialog();
            btnRefresh.Click += (s, e) => UpdateDisplay();

            _wallet = AppWallet.Instance;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {            
            try
            {
                // Обновляем баланс
                txtBalance.Text = $"{_wallet.Balance:N0} ₽";

                // Обновляем статистику
                txtIncomeTotal.Text = $"Доходы: {_wallet.GetTotalIncome():N0} ₽";
                txtExpenseTotal.Text = $"Расходы: {_wallet.GetTotalExpense():N0} ₽";
                txtTransactionCount.Text = $"Операций: {_wallet.Transactions.Count}"; // Эта строка важна!

                // Обновляем список транзакций
                var viewModels = new System.Collections.ObjectModel.ObservableCollection<TransactionViewModel>();
                foreach (var transaction in _wallet.GetTransactions())
                {
                    viewModels.Add(new TransactionViewModel(transaction));
                }

                lstTransactions.ItemsSource = viewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }

        private void ShowAddTransactionWindow(string type)
        {
            var window = new AddTransactionWindow();

            if (type == "Expense")
                window.cmbTransactionType.SelectedIndex = 1;

            window.TransactionAdded += (s, args) => UpdateDisplay();
            window.ShowDialog();
        }
    }
}