namespace VSPCleaner.ViewModels.Shell
{
    using System.Windows;
    using System.Windows.Input;

    using VSPCleaner.Infrastructure.Commands;

    public class ShellViewModel
    {
        public ShellViewModel()
        {
            this.ExitCommand = new DelegateCommand(this.OnExitCommand);
        }

        public ICommand ExitCommand { get; set; }

        private void OnExitCommand() => Application.Current.Shutdown();
    }
}