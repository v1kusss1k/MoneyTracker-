using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Patterns.Factories;
using MoneyTracker.Core.Enums;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class MainWindow : Window
    {
        private AppWallet _wallet;

        public MainWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Обновляем баланс
            txtBalance.Text = $"{_wallet.Balance:N0} ₽";

            // Обновляем статистику
            txtIncomeTotal.Text = $"Доходы: {_wallet.GetTotalIncome():N0} ₽";
            txtExpenseTotal.Text = $"Расходы: {_wallet.GetTotalExpense():N0} ₽";
            txtTransactionCount.Text = $"Операций: {_wallet.Transactions.Count}";

            // Обновляем список транзакций
            lstTransactions.ItemsSource = _wallet.GetTransactions();
        }

        private void btnAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTransactionWindow();
            window.cmbType.SelectedIndex = 0; // Доход
            window.TransactionAdded += UpdateDisplay;
            window.Owner = this;
            window.ShowDialog();
        }

        private void btnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTransactionWindow();
            window.cmbType.SelectedIndex = 1; // Расход
            window.TransactionAdded += UpdateDisplay;
            window.Owner = this;
            window.ShowDialog();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void btnGoals_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("🎯 Функция 'Цели накопления' будет реализована в следующем обновлении!",
                          "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📊 Функция 'Отчёты и графики' будет реализована в следующем обновлении!",
                          "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("⚙ Функция 'Настройки' будет реализована в следующем обновлении!",
                          "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}