namespace KeyboardSplitter.Commands
{
    using System;
    using System.Windows.Input;

    public delegate void ExecuteDelegate(object parameter);

    public delegate bool CanExecuteDelegate(object parameter);

    public class RelayCommand : ICommand
    {
        private ExecuteDelegate execute;
        private CanExecuteDelegate canExecute;

        public RelayCommand(ExecuteDelegate execute)
            : this(execute, null)
        {
            if (this.CanExecuteChanged != null)
            {
                // just to remove the warnings
            }
        }

        public RelayCommand(ExecuteDelegate execute, CanExecuteDelegate canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}