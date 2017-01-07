using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AndyTools.Utilities;
using AndyTools.Wpf;
using Mp3Tools;
using Ookii.Dialogs.Wpf;

namespace Mp3Utilities.ViewModels
{
    public class TrackOrderViewModel : ViewModelBase
    {
        public ICommand SelectFolderCommand { get; }

        public ICommand RandomizeOrderCommand { get; }

        #region Properties

        private ObservableCollection<Mp3FileInfo> _mp3Files;

        public ObservableCollection<Mp3FileInfo> Mp3Files
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

        #endregion

        public TrackOrderViewModel()
        {
            SelectFolderCommand = new CustomCommand(SelectFolder, CanSelectFolder);
            this.RandomizeOrderCommand = new CustomCommand(RandomizeOrder, CanRandomizeOrder);
        }

        private bool CanRandomizeOrder(object obj)
        {
            if (this.Mp3Files != null && this.Mp3Files.Count > 1)
            {
                return true;
            }

            return false;
        }

        private void RandomizeOrder(object obj)
        {
            var shuffledMp3Files = this.Mp3Files.Shuffle();
            this.Mp3Files = new ObservableCollection<Mp3FileInfo>(shuffledMp3Files);
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

                var files = this.Folder.EnumerateFiles()
                    .Where(f => f.Extension == ".mp3")
                    .Select(f => new Mp3FileInfo(f))
                    .OrderBy(f => f.Id3V2Tag.Track);

                this.Mp3Files = new ObservableCollection<Mp3FileInfo>(files);
            }
        }

    }
}
