using System;
using System.Windows.Input;

namespace WpfImhApp
{
    /** BASED ON c-sharpcorner.com/article/explaing-icommand-in-mvvm-wpf by Nirav Daraniya**/

    public class ClickCommand : ICommand
    {
        Action<object> executeMethod;
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

        public ClickCommand(Action<object> executeMethod, Func<object, bool> canExecute)
        {
            this.executeMethod = executeMethod;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }
    }
}