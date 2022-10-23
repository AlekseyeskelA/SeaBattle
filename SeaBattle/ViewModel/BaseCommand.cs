using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;                                             // Для  ": ICommand".

namespace SeaBattle.ViewModel
{
    // RelyCommand (Заготовка)
    // Будем использовать этот класс для введения своей собственной команды.
    internal class BaseCommand : ICommand
    {
        private Action<object> execute;                                 // Переменная обработчика события команды.
        private Func<object, bool> canExecute;                          // Переменная обработчика для определения возможности выполнения команды.

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        internal BaseCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
