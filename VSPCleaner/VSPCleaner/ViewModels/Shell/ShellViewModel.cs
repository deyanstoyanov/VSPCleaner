namespace VSPCleaner.ViewModels.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using System.Windows.Input;

    using PropertyChanged;

    using VSPCleaner.Infrastructure.Commands;
    using VSPCleaner.Infrastructure.DeletionService;
    using VSPCleaner.Models;

    using Application = System.Windows.Application;

    [ImplementPropertyChanged]
    public class ShellViewModel : INotifyPropertyChanged
    {
        private Folder selectedFolder;

        public ShellViewModel()
        {
            this.ExitCommand = new DelegateCommand(this.OnExitCommand);
            this.ImportDirectoryCommand = new DelegateCommand(this.OnImportDirectoryCommand);
            this.CleanDirectoryCommand = new DelegateCommand(this.OnCleanDirectoryCommand, () => this.Folders.Any());
            this.RemoveDirectoryCommand = new DelegateCommand(
                this.OnRemoveDirectoryCommand, 
                () => this.SelectedFolder != null);
            this.Folders = new ObservableCollection<Folder>();
        }

        public ICommand ExitCommand { get; set; }

        public ICommand ImportDirectoryCommand { get; set; }

        public ICommand CleanDirectoryCommand { get; set; }

        public ICommand RemoveDirectoryCommand { get; set; }

        public ICollection<Folder> Folders { get; set; }

        public Folder SelectedFolder
        {
            get
            {
                return this.selectedFolder;
            }

            set
            {
                this.selectedFolder = value;
                ((DelegateCommand)this.RemoveDirectoryCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsFolderImported => this.Folders.Any();

        public string ImportedFoldersStatusBarItemText => $"{this.Folders.Count} imported folder(s)";

        public event PropertyChangedEventHandler PropertyChanged;

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

            ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
            this.OnPropertyChanged(nameof(this.IsFolderImported));
            this.OnPropertyChanged(nameof(this.ImportedFoldersStatusBarItemText));
        }

        private void OnCleanDirectoryCommand()
        {
            foreach (var folder in this.Folders)
            {
                DeletionService.DeleteFoldersRecursively(folder.Path);
            }

            this.Folders.Clear();

            ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
            this.OnPropertyChanged(nameof(this.IsFolderImported));
            this.OnPropertyChanged(nameof(this.ImportedFoldersStatusBarItemText));
        }

        private void OnRemoveDirectoryCommand()
        {
            if (this.SelectedFolder != null)
            {
                this.Folders.Remove(this.SelectedFolder);

                ((DelegateCommand)this.CleanDirectoryCommand).RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.IsFolderImported));
                this.OnPropertyChanged(nameof(this.ImportedFoldersStatusBarItemText));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}