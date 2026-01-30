#nullable disable

using MoneyTracker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoneyTracker.App.Views
{
    public partial class NotificationsWindow : Window
    {
        private readonly NotificationService _notificationService;
        private List<NotificationService.Notification> _notifications;

        public NotificationsWindow(List<NotificationService.Notification> notifications)
        {
            InitializeComponent();
            _notificationService = new NotificationService();
            _notifications = notifications ?? new List<NotificationService.Notification>();

            LoadNotifications();
        }

        private void LoadNotifications()
        {
            lstNotifications.ItemsSource = _notifications;

            int newCount = _notifications.Count(n => !n.IsRead);
            int totalCount = _notifications.Count;

            if (totalCount == 0)
            {
                txtNotificationCount.Text = "Нет уведомлений";
            }
            else if (newCount == 0)
            {
                txtNotificationCount.Text = $"Все уведомления прочитаны ({totalCount})";
            }
            else
            {
                txtNotificationCount.Text = $"{newCount} новых из {totalCount} уведомлений";
            }
        }

        private void BtnCloseNotification_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Guid notificationId)
            {
                var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    // Помечаем как прочитанное
                    notification.IsRead = true;

                    // Обновляем отображение
                    LoadNotifications();

                    // Сохраняем изменения в файл
                    SaveNotifications();

                    MessageBox.Show("Уведомление помечено как прочитанное",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnMarkAllRead_Click(object sender, RoutedEventArgs e)
        {
            if (!_notifications.Any())
            {
                MessageBox.Show("Нет уведомлений для отметки",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Отметить все {_notifications.Count} уведомлений как прочитанные?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var notification in _notifications)
                {
                    notification.IsRead = true;
                }

                LoadNotifications();
                SaveNotifications();

                MessageBox.Show($"Все уведомления ({_notifications.Count}) отмечены как прочитанные",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveNotifications()
        {
            try
            {
                // Сохраняем текущие уведомления
                _notificationService.SaveNotificationsToFile(_notifications);

                // Также обновляем цели в GoalService если есть уведомления о завершенных целях
                UpdateCompletedGoals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения уведомлений: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCompletedGoals()
        {
            try
            {
                var goalService = new GoalService();
                var goals = goalService.GetGoals();

                foreach (var notification in _notifications)
                {
                    if (notification.Title.Contains("Цель достигнута") && notification.IsRead)
                    {
                        // Ищем цель по названию в сообщении
                        var message = notification.Message;
                        if (message.Contains("Цель '") && message.Contains("' достигнута"))
                        {
                            var startIndex = message.IndexOf("Цель '") + 6;
                            var endIndex = message.IndexOf("' достигнута");
                            if (startIndex < endIndex)
                            {
                                var goalName = message.Substring(startIndex, endIndex - startIndex);
                                var goal = goals.FirstOrDefault(g => g.Name == goalName);
                                if (goal != null && !goal.WasNotified)
                                {
                                    goal.WasNotified = true;
                                    goalService.UpdateGoal(goal);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки обновления целей
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Добавим возможность удалить уведомление
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstNotifications.SelectedItem is NotificationService.Notification selectedNotification)
            {
                var result = MessageBox.Show(
                    "Удалить это уведомление?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _notifications.Remove(selectedNotification);
                    LoadNotifications();
                    SaveNotifications();

                    MessageBox.Show("Уведомление удалено",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Добавим контекстное меню в XAML
        private void ListViewItem_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.Content is NotificationService.Notification notification)
            {
                lstNotifications.SelectedItem = notification;

                ContextMenu contextMenu = new ContextMenu();

                MenuItem markReadItem = new MenuItem();
                markReadItem.Header = notification.IsRead ? "Пометить как непрочитанное" : "Пометить как прочитанное";
                markReadItem.Click += (s, args) =>
                {
                    notification.IsRead = !notification.IsRead;
                    LoadNotifications();
                    SaveNotifications();
                };

                MenuItem deleteItem = new MenuItem();
                deleteItem.Header = "Удалить";
                deleteItem.Click += MenuItemDelete_Click;

                contextMenu.Items.Add(markReadItem);
                contextMenu.Items.Add(new Separator());
                contextMenu.Items.Add(deleteItem);

                contextMenu.IsOpen = true;
            }
        }
    }
}