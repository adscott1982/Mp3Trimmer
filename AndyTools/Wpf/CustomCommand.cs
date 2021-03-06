﻿// <auto-generated />

namespace AndyTools.Wpf
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The custom command class. Implements ICommand interface and may be used as command for WPF controls.
    /// </summary>
    public class CustomCommand : ICommand
    {
        /// <summary>
        /// The execute Action.
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// The canExecute predicate.
        /// </summary>
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCommand"/> class. 
        /// </summary>
        /// <param name="execute">
        /// The method to execute, must be void.
        /// </param>
        /// <param name="canExecute">
        /// The method to check whether execute may be performed.
        /// </param>
        public CustomCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// The can execute changed event handler.
        /// </summary>
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

        /// <summary>
        /// The canExecute method to determine whether the command can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            // If CanExecute is null, then no method has been assigned for "canExecute". Therefore return true. 
            // Otherwise run the method and return the result.
            var b = this.canExecute?.Invoke(parameter) ?? true;
            return b;
        }

        /// <summary>
        /// The execute method. 
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}

