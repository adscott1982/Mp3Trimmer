using System.IO;

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

        public ILogger Logger { get; set; }

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

                this.UpdateMp3FileLabels();

                // The following lines will force the properties to re-validate based on the new MP3 file
                TrimStartPosition = TrimStartPosition;
                TrimEndPosition = TrimEndPosition;

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
                UpdateSplitDuration();
                UpdateTrimDurationLabel();
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
                UpdateSplitDuration();
                UpdateTrimDurationLabel();
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

                UpdateSplitDuration();

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
                this.OutputFolderLabel = OutputPath;

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

        private string _trimDurationLabel;
        public string TrimDurationLabel
        {
            get
            {
                return this._trimDurationLabel;
            }
            set
            {
                this._trimDurationLabel = $"Trim duration:\t{value}";
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
            Logger = new Logger();
            LoadMp3FileCommand = new CustomCommand(LoadMp3File, CanLoadMp3File);
            SelectFolderCommand = new CustomCommand(SelectFolder, CanSelectFolder);
            ProcessFileCommand = new CustomCommand(ProcessFile, CanProcessFile);
            SplitCount = 1;
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
            var hasTarget = OutputPath != null;
            var isValidTrim = !TrimStartPosition.Equals(TrimEndPosition);

            if (isFileLoaded && isValidTrim && hasTarget)
            {
                return true;
            }

            return false;
        }

        private void ProcessFile(object obj)
        {
            
            var filename = Path.GetFileNameWithoutExtension(Mp3FileLoaded.FilePath) + "-trimmed.mp3";
            var targetPath = Path.Combine(OutputPath, filename);

            Logger.Add($"Output path - {targetPath}");
            Logger.Add("Starting trim process.");

            if (SplitCount > 1)
            {
                Mp3File.Trim(Mp3FileLoaded.FilePath, targetPath, TrimStartPosition, TrimEndPosition, SplitDuration);
            }
            else
            {
                Mp3File.Trim(Mp3FileLoaded.FilePath, targetPath, TrimStartPosition, TrimEndPosition);
            }

            Logger.Add("Trim complete.");
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

        private void UpdateSplitDuration()
        {
            var splitTimeInTicks = this.TrimDuration.Ticks / SplitCount;

            // Ensure that the final split is not ridiculously short from leftover ticks
            // Splits will always be slightly longer when a timespan does not divide perfectly
            if (TrimDuration.Ticks%SplitCount > 0)
            {
                splitTimeInTicks++;
            }

            this.SplitDuration = new TimeSpan(splitTimeInTicks);
        }

        private void UpdateMp3FileLabels()
        {
            Mp3FileNameLabel = this.Mp3FileLoaded.FileName;
            Mp3FileSizeLabel = $"{this.Mp3FileLoaded.SizeMb:N2} MB";
            Mp3FileLengthLabel = this.Mp3FileLoaded.Length.ToHourMinSec();
        }

        private void UpdateTrimDurationLabel()
        {
            this.TrimDurationLabel = TrimDuration.ToHourMinSec();
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
