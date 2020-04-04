using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImhApp
{
    public class ClickCommandAsync : ICommand
    {
        Func<Task> executeMethod;
        Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {    
                CommandManager.RequerySuggested += value;
            }
            remove
            {    
                CommandManager.RequerySuggested -= value;    
            }    
        }    

        public ClickCommandAsync(Func<Task> executeMethod, Func<object, bool> canExecute)
        {
            this.executeMethod = executeMethod;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public Task ExecuteAsync()
        {
            return executeMethod();
        }
    }
}