using System;
using System.Windows.Input;

namespace Azyobuzi.Azyotter.Updater
{
    public class AlwaysExecutableCommand : ICommand
    {
        public AlwaysExecutableCommand(Action execute)
        {
            this.execute = execute;
        }

        private Action execute;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
