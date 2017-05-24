namespace VSPCleaner.Views.Shell
{
    using System.Windows.Controls.Ribbon;

    using VSPCleaner.ViewModels.Shell;

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : RibbonWindow
    {
        private readonly ShellViewModel viewModel = new ShellViewModel();

        public Shell()
        {
            this.InitializeComponent();
            this.DataContext = this.viewModel;
        }
    }
}