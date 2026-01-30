#nullable disable

using MoneyTracker.Core.Services;
using System;
using System.Windows;

namespace MoneyTracker.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ТОЛЬКО регулярные платежи
            var recurringService = new RecurringService();
            recurringService.ProcessDueTransactions();

            // НЕ проверяем уведомления при запуске
            // Это уберет ошибки
        }
    }
}