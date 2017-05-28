namespace VSPCleaner.Infrastructure.ProgressDialog
{
    public class ProgressDialogSettings
    {
        public static ProgressDialogSettings WithLabelOnly = new ProgressDialogSettings(false, false, true);

        public static ProgressDialogSettings WithSubLabel = new ProgressDialogSettings(true, false, true);

        public static ProgressDialogSettings WithSubLabelAndCancel = new ProgressDialogSettings(true, true, true);

        public ProgressDialogSettings()
        {
            this.ShowSubLabel = false;
            this.ShowCancelButton = false;
            this.ShowProgressBarIndeterminate = true;
        }

        public ProgressDialogSettings(bool showSubLabel, bool showCancelButton, bool showProgressBarIndeterminate)
        {
            this.ShowSubLabel = showSubLabel;
            this.ShowCancelButton = showCancelButton;
            this.ShowProgressBarIndeterminate = showProgressBarIndeterminate;
        }

        public bool ShowSubLabel { get; set; }

        public bool ShowCancelButton { get; set; }

        public bool ShowProgressBarIndeterminate { get; set; }
    }
}