namespace VSPCleaner
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;
            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += this.TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(
            object sender, 
            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            HandleException(unobservedTaskExceptionEventArgs.Exception);
        }

        private void OnDispatcherUnhandledException(
            object sender, 
            DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            HandleException(dispatcherUnhandledExceptionEventArgs.Exception);
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }

        private void CurrentDomainOnUnhandledException(
            object sender, 
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Exception exception = unhandledExceptionEventArgs.ExceptionObject as Exception;
            HandleException(exception);
        }

        private static void HandleException(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}