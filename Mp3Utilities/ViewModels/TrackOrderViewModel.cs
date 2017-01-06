using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AndyTools.Wpf;
using Ookii.Dialogs.Wpf;

namespace Mp3Utilities.ViewModels
{
    public class TrackOrderViewModel : ViewModelBase
    {
        public ICommand SelectFolderCommand { get; }

        private ObservableCollection<FileInfo> _mp3Files;

        public ObservableCollection<FileInfo> Mp3Files
        {
            get
            {
                return this._mp3Files;
            }
            set
            {
                if (this._mp3Files == value) return;
                this._mp3Files = value;
                this.OnPropertyChanged();
            }
        }

        private DirectoryInfo _folder;

        public DirectoryInfo Folder
        {
            get { return this._folder; }
            set
            {
                if (value == _folder) return;
                this._folder = value;
                this.OnPropertyChanged();
            }
        }

        public TrackOrderViewModel()
        {
            SelectFolderCommand = new CustomCommand(SelectFolder, CanSelectFolder);
        }

        private bool CanSelectFolder(object obj)
        {
            return true;
        }

        private void SelectFolder(object obj)
        {
            var folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Select the folder containing the MP3 files";
            folderDialog.UseDescriptionForTitle = true;

            if (folderDialog.ShowDialog() == true)
            {
                this.Folder = new DirectoryInfo(folderDialog.SelectedPath);
                var files = this.Folder.EnumerateFiles().Where(f => f.Extension == ".mp3");
                this.Mp3Files = new ObservableCollection<FileInfo>(files);
            }
        }
    }
}
