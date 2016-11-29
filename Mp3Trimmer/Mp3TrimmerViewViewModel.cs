namespace Mp3Trimmer
{
    using System.Windows.Input;
    using AndyTools.Wpf;
    using System.Runtime.CompilerServices;
    using System.ComponentModel;
    using Mp3Tools;
    using System;

    using AndyTools.Utilities;

    using Ookii.Dialogs.Wpf;
    using Microsoft.Win32;

    public class Mp3TrimmerViewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoadMp3FileCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand ProcessFileCommand { get; }

        #region Properties

        private Mp3File _mp3FileLoaded;
        public Mp3File Mp3FileLoaded
        {
            get
            {
                return this._mp3FileLoaded;
            }
            set
            {
                if (value == this._mp3FileLoaded) return;
                this._mp3FileLoaded = value;
                this.OnPropertyChanged();
            }
        }

        private TimeSpan _splitDuration;
        public TimeSpan SplitDuration
        {
            get
            {
                return this._splitDuration;
            }
            set
            {
                this._splitDuration = ValidateSplitTime(value);
                this.OnPropertyChanged();
            }
        }

        private TimeSpan _trimStartPosition;
        public TimeSpan TrimStartPosition
        {
            get
            {
                return this._trimStartPosition;
            }
            set
            {
                this._trimStartPosition = ValidateTrimStartPosition(value);
                this.OnPropertyChanged();
            }
        }

        private TimeSpan _trimEndPosition;
        public TimeSpan TrimEndPosition
        {
            get
            {
                return this._trimEndPosition;
            }
            set
            {
                this._trimEndPosition = ValidateTrimEndPosition(value);
                this.OnPropertyChanged();
            }
        }

        public TimeSpan TrimDuration => TrimEndPosition - TrimStartPosition;

        private int _splitCount;
        public int SplitCount
        {
            get
            {
                return this._splitCount;
            }
            set
            {
                this._splitCount = value;
                this.OnPropertyChanged();
            }
        }

        private string _outputPath;
        public string OutputPath
        {
            get
            {
                return this._outputPath;
            }
            set
            {
                if (value == this._outputPath) return;
                this._outputPath = value;
                this.OnPropertyChanged();
            }
        }

        private string _mp3FileNameLabel;
        public string Mp3FileNameLabel
        {
            get
            {
                return this._mp3FileNameLabel;
            }
            set
            {
                this._mp3FileNameLabel = $"File name:\t{value}";
                this.OnPropertyChanged();
            }
        }

        private string _mp3FileSizeLabel;
        public string Mp3FileSizeLabel
        {
            get
            {
                return this._mp3FileSizeLabel;
            }
            set
            {
                this._mp3FileSizeLabel = $"File size:\t\t{value}";
                this.OnPropertyChanged();
            }
        }

        private string _mp3FileLengthLabel;
        public string Mp3FileLengthLabel
        {
            get
            {
                return this._mp3FileLengthLabel;
            }
            set
            {
                this._mp3FileLengthLabel = $"File length:\t{value}";
                this.OnPropertyChanged();
            }
        }

        private string _outputFolderLabel;
        public string OutputFolderLabel
        {
            get
            {
                return this._outputFolderLabel;
            }
            set
            {
                this._outputFolderLabel = $"Output folder:\t{value}";
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public Mp3TrimmerViewViewModel()
        {
            LoadMp3FileCommand = new CustomCommand(LoadMp3File, CanLoadMp3File);
            SelectFolderCommand = new CustomCommand(SelectFolder, CanSelectFolder);
            ProcessFileCommand = new CustomCommand(ProcessFile, CanProcessFile);
        }

        #endregion

        #region Commands

        private bool CanLoadMp3File(object obj)
        {
            return true;
        }

        private void LoadMp3File(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                this.Mp3FileLoaded = new Mp3File(openFileDialog.FileName);
            }
        }

        private bool CanSelectFolder(object obj)
        {
            return true;
        }

        private void SelectFolder(object obj)
        {
            var folderDialog = new VistaFolderBrowserDialog();
            folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            folderDialog.Description = "Select the output folder for the trimmed MP3 file(s)";
            folderDialog.ShowNewFolderButton = true;
            folderDialog.UseDescriptionForTitle = true;

            if (folderDialog.ShowDialog() == true)
            {
                OutputPath = folderDialog.SelectedPath;
            }
        }

        private bool CanProcessFile(object obj)
        {
            var isFileLoaded = Mp3FileLoaded != null;
            var isValidTrim = !TrimStartPosition.Equals(TrimEndPosition);

            if (isFileLoaded && isValidTrim)
            {
                return true;
            }

            return false;
        }

        private void ProcessFile(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        private TimeSpan ValidateTrimStartPosition(TimeSpan value)
        {
            var maxValue = new TimeSpan(99, 59, 59);
            var minValue = new TimeSpan(0, 0, 0);

            if (this.Mp3FileLoaded != null)
            {
                maxValue = Mp3FileLoaded.Length;
            }

            var result = value.Clamp(minValue, maxValue);

            if (result > TrimEndPosition)
            {
                TrimEndPosition = result;
            }

            return result;
        }

        private TimeSpan ValidateTrimEndPosition(TimeSpan value)
        {
            var maxValue = new TimeSpan(99, 59, 59);
            var minValue = new TimeSpan(0, 0, 0);

            if (this.Mp3FileLoaded != null)
            {
                maxValue = Mp3FileLoaded.Length;
            }

            var result = value.Clamp(minValue, maxValue);

            if (result < TrimStartPosition)
            {
                TrimStartPosition = result;
            }

            return result;
        }

        private TimeSpan ValidateSplitTime(TimeSpan value)
        {
            var maxValue = this.TrimDuration;
            var minValue = new TimeSpan(0, 0, 0);

            var result = value.Clamp(minValue, maxValue);

            return result;
        }

        private void UpdateMp3FileLabels()
        {
            Mp3FileNameLabel = this.Mp3FileLoaded.FileName;
            Mp3FileSizeLabel = $"{this.Mp3FileLoaded.SizeMb:N2} MB";

            var length = this.Mp3FileLoaded.Length;
            var lengthString = "";
            lengthString += length.Hours > 0 ? $"{length.Hours}h " : "";
            lengthString += $"{length.Minutes}m ";
            lengthString += $"{length.Seconds}s";

            Mp3FileLengthLabel = lengthString;
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "Mp3FileLoaded")
            {
                this.UpdateMp3FileLabels();

                // The following lines will force the properties to re-validate based on the new MP3 file
                TrimStartPosition = TrimStartPosition;
                TrimEndPosition = TrimEndPosition;
            }

            if (propertyName == "OutputPath")
            {
                this.OutputFolderLabel = OutputPath;
            }

            if (propertyName == "SplitCount")
            {
                var splitTimeInTicks = TrimDuration.Ticks/SplitCount;
                this.SplitDuration = new TimeSpan(splitTimeInTicks);
            }
        }

        #endregion
    }
}
