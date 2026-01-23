using MoneyTracker.Core.Patterns.Singleton;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class ReportsWindow : Window
    {
        private AppWallet _wallet;

        public ReportsWindow()
        {
            InitializeComponent();
            _wallet = AppWallet.Instance;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            txtTotalTransactions.Text = _wallet.Transactions.Count.ToString();
            txtTotalIncome.Text = $"{_wallet.GetTotalIncome():N0} ₽";
            txtTotalExpense.Text = $"{_wallet.GetTotalExpense():N0} ₽";
            txtCurrentBalance.Text = $"{_wallet.Balance:N0} ₽";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}