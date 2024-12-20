﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows.Input;

namespace ViewModel
{
    public class Commands : ICommand
    {
        private readonly Action<object> execute;

        private readonly Func<object, bool> canExecute;

        public Commands(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;   
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute == null ? true : canExecute(parameter);

        public void Execute(object parameter) => execute?.Invoke(parameter);
    }
}

