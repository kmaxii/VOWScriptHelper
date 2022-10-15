using System;
using System.Windows.Input;

namespace VowScriptHelper.Core
{
    internal class RelayCommand : ICommand
    {

        private Action<object> _excecute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> excecute, Func<object, bool> canExecute = null)
        {
            this._excecute = excecute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _excecute(parameter);
        }
    }
}
