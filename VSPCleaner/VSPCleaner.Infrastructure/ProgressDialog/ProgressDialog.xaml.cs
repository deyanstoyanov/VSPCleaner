namespace VSPCleaner.Infrastructure.ProgressDialog
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    // taken from - http://www.parago.de/blog/2011/04/22/how-to-implement-a-modern-progress-dialog-for-wpf-applications.html
    public partial class ProgressDialog : Window
    {
        private volatile bool isBusy;

        private BackgroundWorker worker;

        public ProgressDialog(ProgressDialogSettings settings)
        {
            this.InitializeComponent();

            if (settings == null)
            {
                settings = ProgressDialogSettings.WithLabelOnly;
            }

            if (settings.ShowSubLabel)
            {
                this.Height = 140;
                this.MinHeight = 140;
                this.SubTextLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.Height = 110;
                this.MinHeight = 110;
                this.SubTextLabel.Visibility = Visibility.Collapsed;
            }

            this.CancelButton.Visibility = settings.ShowCancelButton ? Visibility.Visible : Visibility.Collapsed;

            this.ProgressBar.IsIndeterminate = settings.ShowProgressBarIndeterminate;
        }

        public static ProgressDialogContext Current { get; set; }

        public string Label
        {
            get
            {
                return this.TextLabel.Text;
            }

            set
            {
                this.TextLabel.Text = value;
            }
        }

        public ProgressDialogResult Result { get; private set; }

        public string SubLabel
        {
            get
            {
                return this.SubTextLabel.Text;
            }

            set
            {
                this.SubTextLabel.Text = value;
            }
        }

        public static ProgressDialogResult Execute(Window owner, string label, Action operation) => ExecuteInternal(
            owner,
            label,
            operation,
            null);

        public static ProgressDialogResult Execute(
            Window owner,
            string label,
            Action operation,
            ProgressDialogSettings settings) => ExecuteInternal(owner, label, operation, settings);

        public static ProgressDialogResult Execute(Window owner, string label, Func<object> operationWithResult) =>
            ExecuteInternal(owner, label, operationWithResult, null);

        public static ProgressDialogResult Execute(
            Window owner,
            string label,
            Func<object> operationWithResult,
            ProgressDialogSettings settings) => ExecuteInternal(owner, label, operationWithResult, settings);

        public static void Execute(
            Window owner,
            string label,
            Action operation,
            Action<ProgressDialogResult> successOperation,
            Action<ProgressDialogResult> failureOperation = null,
            Action<ProgressDialogResult> cancelledOperation = null)
        {
            var result = ExecuteInternal(owner, label, operation, null);
            if (result.Cancelled && cancelledOperation != null)
            {
                cancelledOperation(result);
            }
            else if (result.OperationFailed && failureOperation != null)
            {
                failureOperation(result);
            }
            else
            {
                successOperation?.Invoke(result);
            }
        }

        public static ProgressDialogResult ExecuteInternal(
            Window owner,
            string label,
            object operation,
            ProgressDialogSettings settings)
        {
            var dialog = new ProgressDialog(settings) { Owner = owner };

            if (!string.IsNullOrEmpty(label))
            {
                dialog.Label = label;
            }

            return dialog.Execute(operation);
        }

        public ProgressDialogResult Execute(object operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            ProgressDialogResult result = null;
            this.isBusy = true;
            this.worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            this.worker.DoWork += (s, e) =>
                {
                    try
                    {
                        Current = new ProgressDialogContext(s as BackgroundWorker, e);

                        if (operation is Action)
                        {
                            ((Action)operation)();
                        }
                        else if (operation is Func<object>)
                        {
                            e.Result = ((Func<object>)operation)();
                        }
                        else
                        {
                            throw new InvalidOperationException("Operation type is not supoorted");
                        }

                        // NOTE: Always do this check in order to avoid default processing after the Cancel button has been pressed.
                        // This call will set the Cancelled flag on the result structure.
                        Current.CheckCancellationPending();
                    }
                    catch (ProgressDialogCancellationExcpetion)
                    {
                    }
                    catch (Exception ex)
                    {
                        if (!Current.CheckCancellationPending())
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        Current = null;
                    }
                };

            this.worker.RunWorkerCompleted += (s, e) =>
                {
                    result = new ProgressDialogResult(e);

                    this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Send,
                        (SendOrPostCallback)delegate
                            {
                                this.isBusy = false;
                                this.Close();
                            },
                        null);
                };

            this.worker.ProgressChanged += (s, e) =>
                {
                    if (this.worker.CancellationPending)
                    {
                        return;
                    }

                    this.SubLabel = (e.UserState as string) ?? string.Empty;
                    this.ProgressBar.Value = e.ProgressPercentage;
                };

            this.worker.RunWorkerAsync();

            this.ShowDialog();

            return result;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.worker == null || !this.worker.WorkerSupportsCancellation)
            {
                return;
            }

            this.SubLabel = "Please wait while process will be cancelled...";
            this.CancelButton.IsEnabled = false;
            this.worker.CancelAsync();
        }

        private void OnClosing(object sender, CancelEventArgs e) => e.Cancel = this.isBusy;
    }
}