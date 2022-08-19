using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeithleyControl.Commands
{
    //public class RelayCommand : ICommand
    //{
    //    private Action<object> execute;
    //    private Func<object, bool> canExecute;

    //    public RelayCommand(Action<object> execute)
    //    {
    //        this.execute = execute;
    //        canExecute = null;
    //    }

    //    public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
    //    {
    //        this.execute = execute;
    //        this.canExecute = canExecute;
    //    }

    //    /// <summary>
    //    /// CanExecuteChanged delegates the event subscription to the CommandManager.RequerySuggested event.
    //    /// This ensures that the WPF commanding infrastructure asks all RelayCommand objects if they can execute whenever
    //    /// it asks the built-in commands.
    //    /// </summary>

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add { CommandManager.RequerySuggested += value; }
    //        remove { CommandManager.RequerySuggested -= value; }
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return canExecute == null || CanExecute(parameter);
    //    }

    //    public void Execute(object parameter)
    //    {
    //        execute(parameter);
    //    }
    //}
    public class RelayCommand : ICommand

    {
        private Action commandTasks;

        public RelayCommand(Action workTodo)
        {
            commandTasks = workTodo;

        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;

        }

        public void Execute(object parameter)
        {
            commandTasks();
        }
    }
}
