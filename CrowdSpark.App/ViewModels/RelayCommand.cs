using System;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged(object sender)
        {
            CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
