namespace VSPCleaner.Infrastructure.ProgressDialog
{
    using System;
    using System.ComponentModel;

    public class ProgressDialogContext
    {
        public ProgressDialogContext(BackgroundWorker worker, DoWorkEventArgs arguments)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            this.Worker = worker;
            this.Arguments = arguments;
        }

        public BackgroundWorker Worker { get; }

        public DoWorkEventArgs Arguments { get; }

        public bool CheckCancellationPending()
        {
            if (this.Worker.WorkerSupportsCancellation && this.Worker.CancellationPending)
            {
                this.Arguments.Cancel = true;
            }

            return this.Arguments.Cancel;
        }

        public void ThrowIfCancellationPending()
        {
            if (this.CheckCancellationPending())
            {
                throw new ProgressDialogCancellationExcpetion();
            }
        }

        public void Report(string message)
        {
            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(0, message);
            }
        }

        public void Report(string format, params object[] arg)
        {
            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(0, string.Format(format, arg));
            }
        }

        public void Report(int percentProgress, string message)
        {
            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(percentProgress, message);
            }
        }

        public void Report(int percentProgress, string format, params object[] arg)
        {
            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(percentProgress, string.Format(format, arg));
            }
        }

        public void ReportWithCancellationCheck(string message)
        {
            this.ThrowIfCancellationPending();

            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(0, message);
            }
        }

        public void ReportWithCancellationCheck(string format, params object[] arg)
        {
            this.ThrowIfCancellationPending();

            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(0, string.Format(format, arg));
            }
        }

        public void ReportWithCancellationCheck(int percentProgress, string message)
        {
            this.ThrowIfCancellationPending();

            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(percentProgress, message);
            }
        }

        public void ReportWithCancellationCheck(int percentProgress, string format, params object[] arg)
        {
            this.ThrowIfCancellationPending();

            if (this.Worker.WorkerReportsProgress)
            {
                this.Worker.ReportProgress(percentProgress, string.Format(format, arg));
            }
        }
    }
}