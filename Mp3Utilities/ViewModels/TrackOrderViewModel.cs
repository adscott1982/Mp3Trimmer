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

        public ICommand ConfirmCommand { get; }

        #region Properties

        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            set
            {
                if (this._isBusy == value) return;
                this._isBusy = value;
                this.OnPropertyChanged();
            }
        }

        private bool _hasRandomized;

        public bool HasRandomized
        {
            get
            {
                return this._hasRandomized;
            }
            set
            {
                if (this._hasRandomized == value) return;
                this._hasRandomized = value;
                this.OnPropertyChanged();
            }
        }

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
            this.SelectFolderCommand = new CustomCommand(SelectFolder, CanSelectFolder);
            this.RandomizeOrderCommand = new CustomCommand(RandomizeOrder, CanRandomizeOrder);
            this.ConfirmCommand = new CustomCommand(Confirm, CanConfirm);
        }

        private bool CanConfirm(object obj)
        {
            return this.HasRandomized && !this.IsBusy;
        }

        private void Confirm(object obj)
        {
            SaveRandomTrackOrder();
            LoadFolder(this.Folder.FullName);
        }

        private void SaveRandomTrackOrder()
        {
            for (var i = 1; i <= this.Mp3Files.Count; i++)
            {
                var file = this.Mp3Files[i - 1];
                file.Id3V2Tag.Track = (uint)i;
                file.SetTagByFrameId("TALB", "ANDY ALBUM Yo");
                file.SaveTag();
            }
        }

        private bool CanRandomizeOrder(object obj)
        {
            return this.Mp3Files != null && this.Mp3Files.Count > 1 && !this.IsBusy;
        }

        private void RandomizeOrder(object obj)
        {
            var shuffledMp3Files = this.Mp3Files.Shuffle();
            this.Mp3Files = new ObservableCollection<Mp3FileInfo>(shuffledMp3Files);
            this.HasRandomized = true;
        }

        private bool CanSelectFolder(object obj)
        {
            return !this.IsBusy;
        }

        private void SelectFolder(object obj)
        {
            var folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Select the folder containing the MP3 files";
            folderDialog.UseDescriptionForTitle = true;

            if (folderDialog.ShowDialog() == true)
            {
                this.LoadFolder(folderDialog.SelectedPath);
            }
        }

        private void LoadFolder(string path)
        {
            this.Folder = new DirectoryInfo(path);

            var files = this.Folder.EnumerateFiles()
                .Where(f => f.Extension == ".mp3")
                .Select(f => new Mp3FileInfo(f))
                .OrderBy(f => f.Id3V2Tag.Track);

            this.Mp3Files = new ObservableCollection<Mp3FileInfo>(files);

            this.HasRandomized = false;
        }
    }
}
