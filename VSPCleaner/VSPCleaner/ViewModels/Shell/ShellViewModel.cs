namespace VSPCleaner.ViewModels.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;

    using PropertyChanged;

    using VSPCleaner.Infrastructure.Commands;
    using VSPCleaner.Infrastructure.DeletionService;
    using VSPCleaner.Models;

    using Application = System.Windows.Application;

    [ImplementPropertyChanged]
    public class ShellViewModel
    {
        private ICollection<Folder> folders;

        public ShellViewModel()
        {
            this.ExitCommand = new DelegateCommand(this.OnExitCommand);
            this.ImportDirectoryCommand = new DelegateCommand(this.OnImportDirectoryCommand);
            this.CleanDirectoryCommand = new DelegateCommand(this.OnCleanDirectoryCommand, () => this.Folders.Any());
            this.Folders = new ObservableCollection<Folder>();
        }

        public ICommand ExitCommand { get; set; }

        public ICommand ImportDirectoryCommand { get; set; }

        public ICommand CleanDirectoryCommand { get; set; }

        public ICollection<Folder> Folders
        {
            get
            {
                return this.folders;
            }

            set
            {
                this.folders = value;
                ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsFolderImported { get; set; }

        public string ImportedFoldersStatusBarItemText { get; set; }

        private void OnExitCommand() => Application.Current.Shutdown();

        private void OnImportDirectoryCommand()
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                return;
            }

            var selectedPath = folderDialog.SelectedPath;
            if (this.Folders.Any(x => x.Path == selectedPath))
            {
                MessageBox.Show(
                    $"{selectedPath} is already imported!", 
                    "Duplicate folders", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                return;
            }

            var index = selectedPath.LastIndexOf(@"\", StringComparison.Ordinal);
            var folderName = selectedPath.Substring(index + 1);
            var folder = new Folder { Path = selectedPath, Name = folderName };

            this.Folders.Add(folder);

            this.IsFolderImported = true;
            this.ImportedFoldersStatusBarItemText = $"{this.Folders.Count} imported folder(s)";
            ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
        }

        private void OnCleanDirectoryCommand()
        {
            foreach (var folder in this.Folders)
            {
                DeletionService.DeleteFoldersRecursively(folder.Path);
            }

            this.Folders.Clear();
            ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
        }
    }
}