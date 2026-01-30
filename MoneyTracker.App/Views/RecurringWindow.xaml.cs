#nullable disable

using MoneyTracker.Core.Services;
using MoneyTracker.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class RecurringWindow : Window
    {
        private RecurringService _service;
        private ObservableCollection<RecurringViewModel> _transactions;

        public RecurringWindow()
        {
            InitializeComponent();
            _service = new RecurringService();
            _transactions = new ObservableCollection<RecurringViewModel>();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            _transactions.Clear();
            foreach (var transaction in _service.GetAllRecurringTransactions())
            {
                _transactions.Add(new RecurringViewModel(transaction));
            }
            lstRecurring.ItemsSource = _transactions;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddRecurringWindow();
            if (window.ShowDialog() == true && window.RecurringTransaction != null)
            {
                _service.AddRecurringTransaction(window.RecurringTransaction);
                LoadTransactions();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button?.Tag is Guid id)
            {
                var transaction = _service.GetRecurringTransaction(id);
                if (transaction != null)
                {
                    var window = new AddRecurringWindow(transaction);
                    if (window.ShowDialog() == true && window.RecurringTransaction != null)
                    {
                        _service.UpdateRecurringTransaction(window.RecurringTransaction);
                        LoadTransactions();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button?.Tag is Guid id)
            {
                var result = MessageBox.Show("Удалить регулярный платеж?", "Подтверждение",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _service.DeleteRecurringTransaction(id);
                    LoadTransactions();
                }
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button?.Tag is Guid id)
            {
                // Находим транзакцию
                var transaction = _service.GetRecurringTransaction(id);
                if (transaction != null && transaction.ShouldProcessToday())
                {
                    // Обрабатываем конкретную транзакцию
                    var processed = _service.ProcessSingleTransaction(id);
                    if (processed)
                    {
                        LoadTransactions();
                        MessageBox.Show($"Платеж '{transaction.Name}' выполнен\nСледующий раз: {transaction.NextDate:dd.MM.yyyy}",
                                      "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Этот платеж не просрочен или уже обработан",
                                  "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnProcessAll_Click(object sender, RoutedEventArgs e)
        {
            var dueTransactions = _service.GetDueToday();
            if (dueTransactions.Count == 0)
            {
                MessageBox.Show("Нет просроченных платежей для выполнения",
                               "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Выполнить все просроченные платежи?\nКоличество: {dueTransactions.Count}",
                                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var processed = _service.ProcessDueTransactions();
                LoadTransactions();
                MessageBox.Show($"Выполнено {processed.Count} платежей",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}