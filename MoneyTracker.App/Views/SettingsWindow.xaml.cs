#nullable disable

using MoneyTracker.Core;
using MoneyTracker.Core.Services;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MoneyTracker.App.Views
{
    public partial class SettingsWindow : Window
    {
        private NotificationService.AppSettings _currentSettings;
        private readonly string _settingsPath;

        public SettingsWindow()
        {
            InitializeComponent();

            _settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MoneyTracker", "settings.json");


            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                // Используем метод из NotificationService
                _currentSettings = NotificationService.LoadAppSettingsStatic();

                // Заполняем форму
                chkBudgetAlerts.IsChecked = _currentSettings.EnableBudgetAlerts;
                chkGoalReminders.IsChecked = _currentSettings.EnableGoalReminders;
                chkRecurringReminders.IsChecked = _currentSettings.EnableRecurringReminders;
                chkLargeExpenseAlerts.IsChecked = _currentSettings.EnableLargeExpenseAlerts;
                txtExpenseThreshold.Text = _currentSettings.LargeExpenseThreshold.ToString();
                chkAutoSave.IsChecked = _currentSettings.AutoSave;
                chkStartMinimized.IsChecked = _currentSettings.StartMinimized;
                chkShowWelcome.IsChecked = _currentSettings.ShowWelcome;

                cmbTheme.SelectedIndex = _currentSettings.Theme == "dark" ? 1 : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка");
                _currentSettings = new NotificationService.AppSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                _currentSettings.EnableBudgetAlerts = chkBudgetAlerts.IsChecked == true;
                _currentSettings.EnableGoalReminders = chkGoalReminders.IsChecked == true;
                _currentSettings.EnableRecurringReminders = chkRecurringReminders.IsChecked == true;
                _currentSettings.EnableLargeExpenseAlerts = chkLargeExpenseAlerts.IsChecked == true;

                if (decimal.TryParse(txtExpenseThreshold.Text, out decimal threshold) && threshold > 0)
                {
                    _currentSettings.LargeExpenseThreshold = threshold;
                }

                _currentSettings.AutoSave = chkAutoSave.IsChecked == true;
                _currentSettings.StartMinimized = chkStartMinimized.IsChecked == true;
                _currentSettings.ShowWelcome = chkShowWelcome.IsChecked == true;
                _currentSettings.Theme = cmbTheme.SelectedIndex == 1 ? "dark" : "light";

                var dir = Path.GetDirectoryName(_settingsPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_currentSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);

                // Обновляем автосохранение в FileManager
                FileManager.SetAutoSaveEnabled(_currentSettings.AutoSave);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ApplyTheme();

            MessageBox.Show("Настройки сохранены", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void ApplyTheme()
        {
            if (_currentSettings.Theme == "dark")
            {
                MessageBox.Show("Темная тема будет доступна в следующем обновлении",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Сбросить все настройки к значениям по умолчанию?",
                "Сброс настроек",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _currentSettings = new NotificationService.AppSettings();

                chkBudgetAlerts.IsChecked = true;
                chkGoalReminders.IsChecked = true;
                chkRecurringReminders.IsChecked = true;
                chkLargeExpenseAlerts.IsChecked = true;
                txtExpenseThreshold.Text = "10000";
                chkAutoSave.IsChecked = true;
                chkStartMinimized.IsChecked = false;
                chkShowWelcome.IsChecked = true;
                cmbTheme.SelectedIndex = 0;

                MessageBox.Show("Настройки сброшены", "Информация");
            }
        }
        private void EnsureSettingsFileExists()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MoneyTracker", "settings.json");

                var dir = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(settingsPath))
                {
                    var defaultSettings = new NotificationService.AppSettings();
                    var json = JsonSerializer.Serialize(defaultSettings,
                        new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(settingsPath, json);
                }
            }
            catch { }
        }
    }
}