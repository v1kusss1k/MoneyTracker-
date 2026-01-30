using MoneyTracker.App.ViewModels;
using MoneyTracker.Core.Patterns.Singleton;
using MoneyTracker.Core.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            btnGoals.Click += btnGoals_Click;
            btnClearAll.Click += btnClearAll_Click;
            btnReports.Click += btnReports_Click;
            btnSettings.Click += btnSettings_Click;
            btnBudgets.Click += btnBudgets_Click;

            _wallet = AppWallet.Instance;
            UpdateDisplay();
        }

        // Обработчик кнопки "Цели"
        private void btnGoals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var goalsWindow = new GoalsWindow();
                goalsWindow.Owner = this;
                goalsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия целей: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportsWindow = new ReportsWindow();
                reportsWindow.Owner = this;
                reportsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия отчетов: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.Owner = this;
                settingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия настроек: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            txtStatus.Text = $"Баланс: {_wallet.Balance:N0}₽ | Операций: {_wallet.Transactions.Count} | ✓ Автосохранение";
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
                _wallet.Clear();
                UpdateDisplay();
                txtStatus.Text = "Все операции удалены";
            }
        }

        // НОВЫЕ МЕТОДЫ ДЛЯ УДАЛЕНИЯ ОТДЕЛЬНЫХ ТРАНЗАКЦИЙ

        // Обработчик для кнопки удаления в строке
        private void BtnDeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedTransaction(sender);
        }

        // Обработчик для контекстного меню
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedTransaction(null);
        }

        // Общий метод удаления
        private void DeleteSelectedTransaction(object buttonOrMenuItem)
        {
            try
            {
                Guid transactionId = Guid.Empty;

                // Если нажали кнопку в строке
                if (buttonOrMenuItem is Button button && button.Tag is Guid buttonTag)
                {
                    transactionId = buttonTag;
                }
                // Если выбрали из контекстного меню
                else if (lstTransactions.SelectedItem is TransactionViewModel selected)
                {
                    transactionId = selected.Id;
                }
                else
                {
                    MessageBox.Show("Выберите транзакцию для удаления",
                                  "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Находим транзакцию
                var transaction = _wallet.Transactions.FirstOrDefault(t => t.Id == transactionId);
                if (transaction == null) return;

                var transactionType = transaction.Type == MoneyTracker.Core.Enums.TransactionType.Income ? "доход" : "расход";
                var amount = transaction.Amount;
                var category = transaction.Category;
                var description = transaction.Description;

                var result = MessageBox.Show(
                    $"Удалить {transactionType}?\n\n" +
                    $"💰 Сумма: {amount:N0}₽\n" +
                    $"📁 Категория: {category}\n" +
                    $"📝 Описание: {(string.IsNullOrEmpty(description) ? "(нет)" : description)}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var success = _wallet.RemoveTransaction(transactionId);
                    if (success)
                    {
                        UpdateDisplay();
                        txtStatus.Text = $"Транзакция удалена. Баланс: {_wallet.Balance:N0}₽";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Копирование суммы
        private void MenuItemCopyAmount_Click(object sender, RoutedEventArgs e)
        {
            if (lstTransactions.SelectedItem is TransactionViewModel selected)
            {
                System.Windows.Clipboard.SetText(selected.Amount.ToString());
                txtStatus.Text = "Сумма скопирована в буфер";
            }
        }

        // Копирование описания
        private void MenuItemCopyDescription_Click(object sender, RoutedEventArgs e)
        {
            if (lstTransactions.SelectedItem is TransactionViewModel selected)
            {
                System.Windows.Clipboard.SetText(selected.Description);
                txtStatus.Text = "Описание скопировано в буфер";
            }
        }

        // Просмотр деталей
        private void MenuItemDetails_Click(object sender, RoutedEventArgs e)
        {
            if (lstTransactions.SelectedItem is TransactionViewModel selected)
            {
                MessageBox.Show(
                    $"📋 Детали транзакции:\n\n" +
                    $"📅 Дата: {selected.DateFormatted}\n" +
                    $"📁 Тип: {selected.TypeDisplay}\n" +
                    $"🏷️ Категория: {selected.Category}\n" +
                    $"💰 Сумма: {selected.AmountFormatted}\n" +
                    $"📝 Описание: {selected.Description}",
                    "Детали транзакции",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        private void btnBudgets_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var budgetWindow = new BudgetWindow();
                budgetWindow.Owner = this;
                budgetWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия бюджетов: {ex.Message}", "Ошибка");
            }
        }
        private void btnRecurring_Click(object sender, RoutedEventArgs e)
        {
            var window = new RecurringWindow();
            window.Owner = this;
            window.ShowDialog();
        }
        private void btnNotifications_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var notificationService = new NotificationService();

                // Получаем новые уведомления
                var newNotifications = notificationService.CheckAllNotifications();

                // Загружаем сохраненные уведомления
                var savedNotifications = notificationService.LoadNotificationsFromFile();

                // Объединяем, убирая дубликаты
                var allNotifications = new List<NotificationService.Notification>();
                var seenIds = new HashSet<Guid>();

                // Сначала добавляем новые
                foreach (var notification in newNotifications)
                {
                    if (!seenIds.Contains(notification.Id))
                    {
                        allNotifications.Add(notification);
                        seenIds.Add(notification.Id);
                    }
                }

                // Затем добавляем сохраненные
                foreach (var notification in savedNotifications)
                {
                    if (!seenIds.Contains(notification.Id))
                    {
                        allNotifications.Add(notification);
                        seenIds.Add(notification.Id);
                    }
                }

                // Сортируем
                allNotifications = allNotifications
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                if (allNotifications.Any())
                {
                    var notificationWindow = new NotificationsWindow(allNotifications);
                    notificationWindow.Owner = this;
                    notificationWindow.ShowDialog();

                    // Обновляем кнопку после закрытия
                    UpdateNotificationButton();
                }
                else
                {
                    MessageBox.Show("Нет уведомлений", "Уведомления",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Упрощенное сообщение об ошибке
                MessageBox.Show("Не удалось загрузить уведомления. Файл уведомлений будет создан заново.",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Попробуем создать новый файл
                try
                {
                    var notificationService = new NotificationService();
                    notificationService.SaveNotificationsToFile(new List<NotificationService.Notification>());
                }
                catch { }
            }
        }

        // Метод для обновления текста кнопки уведомлений
        private void UpdateNotificationButton()
        {
            try
            {
                var notificationService = new NotificationService();

                // Просто проверяем есть ли уведомления
                bool hasNotifications = notificationService.HasNotifications();

                if (hasNotifications)
                {
                    btnNotifications.Content = "🔔 Есть уведомления";
                    btnNotifications.Background = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    btnNotifications.ToolTip = "Есть новые уведомления";
                }
                else
                {
                    btnNotifications.Content = "🔔 Уведомления";
                    btnNotifications.Background = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                    btnNotifications.ToolTip = "Просмотреть уведомления";
                }
            }
            catch
            {
                // Если ошибка - просто показываем стандартную кнопку
                btnNotifications.Content = "🔔 Уведомления";
                btnNotifications.Background = new SolidColorBrush(Color.FromRgb(255, 152, 0));
            }
        }

        // Вызываем при загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateNotificationButton();
        }
    }
}