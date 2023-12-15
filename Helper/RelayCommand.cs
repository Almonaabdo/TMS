using System;
using System.Windows.Input;

namespace TMS_Project.Helper
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        
        private readonly Action _methodToExecute;
        private readonly Func<bool> _canExecuteEvaluator;

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this._methodToExecute = methodToExecute;
            this._canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }


        /*
        * METHOD NAME: CanExecute
        * DESCRIPTION: To whether to execute the command or not
        * PARAM: paramaeter
        * RETURN: result - true or false
        */
        public bool CanExecute(object parameter)
        {
            if (this._canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this._canExecuteEvaluator.Invoke();
                return result;
            }
        }


        /*
        * METHOD NAME: Execute
        * DESCRIPTION: Executes the command
        * PARAM: paramaeter
        * RETURN: result - treu or false
        */
        public void Execute(object parameter)
        {
            this._methodToExecute.Invoke();
        }
    }
}