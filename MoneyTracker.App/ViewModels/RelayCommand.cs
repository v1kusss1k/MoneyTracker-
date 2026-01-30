using System;
using System.Windows.Input;

namespace MoneyTracker.App.ViewModels
{
    // команда для связывания действий с кнопками в wpf, реализует icommand для выполнения действий по команде
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        // событие изменения возможности выполнения
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // конструктор с действием и условием выполнения
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // проверка возможности выполнения команды
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // выполнение команды
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}