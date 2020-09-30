using System;
using System.Windows.Input;

namespace MockUpdatesProducer
{
    internal class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;

        public DelegateCommand(
            Action<object> execute)
        {
            _execute = execute;
        }
        
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;
    }
}