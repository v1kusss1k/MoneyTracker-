using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoneyTracker.App.ViewModels
{
    // базовая viewmodel для уведомления об изменениях свойств, реализует inotifypropertychanged для привязки данных wpf
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // событие изменения свойства
        public event PropertyChangedEventHandler? PropertyChanged;

        // вызов события при изменении свойства
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // установка значения с проверкой изменений
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
                return false;                                      // значение не изменилось

            field = value;
            OnPropertyChanged(propertyName);
            return true;                                           // значение изменилось
        }
    }
}