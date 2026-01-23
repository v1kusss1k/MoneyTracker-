using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class GoalsWindow : Window
    {
        public GoalsWindow()
        {
            InitializeComponent();
        }

        private void BtnAddGoal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления цели будет реализована в следующей версии", "В разработке");
        }

        private void BtnEditGoal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование целей будет доступно в следующем обновлении", "В разработке");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}