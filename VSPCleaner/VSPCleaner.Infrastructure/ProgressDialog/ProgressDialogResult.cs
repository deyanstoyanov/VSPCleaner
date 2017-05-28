namespace VSPCleaner.Infrastructure.ProgressDialog
{
    using System;
    using System.ComponentModel;

    public class ProgressDialogResult
    {
        public ProgressDialogResult(RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.Cancelled = true;
            }
            else if (e.Error != null)
            {
                this.Error = e.Error;
            }
            else
            {
                this.Result = e.Result;
            }
        }

        public object Result { get; private set; }

        public bool Cancelled { get; private set; }

        public Exception Error { get; }

        public bool OperationFailed => this.Error != null;
    }
}