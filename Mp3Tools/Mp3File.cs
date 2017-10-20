// Unit tests required:
// - When source file does not exist or is invalid, exception is generated
// - When source file is not a valid MP3, exception is generated
// - When target location is not valid, exception is generated
// - When startPosition is after the length of the MP3, exception is generated
// - When endPosition is before the startPosition, exception is generated
// - When all parameters are valid, file is generated
// - When all parameters are valid, but endPosition is after the end of the file, file is generated


namespace Mp3Tools
{
    using System;
    using System.IO;
    using NAudio.Wave;
    using AndyTools.Utilities;
    using System.Collections.Generic;
    using Tag = TagLib.Id3v2.Tag;

    /// <summary>
    /// Class which provides MP3 file operations.
    /// </summary>
    public class Mp3File : IDisposable
    {
        private Mp3FileReader mp3Reader { get; set; }
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public TimeSpan Length { get; private set; }
        public float SizeMb { get; private set; }

        public Mp3File(string path)
        {
            Load(path);
        }

        public void Load(string path)
        {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists) throw new FileNotFoundException($"File not found: {path}");
            if (fileInfo.Extension.ToLowerInvariant() != ".mp3") throw new FileFormatException("File extension must be .MP3");

            this.FilePath = fileInfo.FullName;
            this.FileName = fileInfo.Name;
            this.SizeMb = fileInfo.Length / 1000000f;

            this.mp3Reader = new Mp3FileReader(path);
            Length = this.mp3Reader.TotalTime;
        }

        /// <summary>
        /// Will trim an MP3 file to a target file based on provided parameters.
        /// </summary>
        /// <param name="sourceFile">The source MP3 file.</param>
        /// <param name="targetFile">The target MP3 file.</param>
        /// <param name="startPosition">The start position of the trim.</param>
        /// <param name="endPosition">The end position of the trim.</param>
        /// <param name="progressManager"></param>
        public void Trim(string sourceFile, string targetFile, TimeSpan startPosition, TimeSpan endPosition, ProgressManager progressManager)
        {
            using (var writer = File.Create(targetFile))
            {
                this.mp3Reader.CurrentTime = startPosition;

                while (this.mp3Reader.CurrentTime < endPosition)
                {
                    var frame = this.mp3Reader.ReadNextFrame();
                    if (frame == null) break;
                    writer.Write(frame.RawData, 0, frame.RawData.Length);
                }
            }
        }

        /// <summary>
        /// Will trim an MP3 file to a target file based on provided parameters, splitting files based on split duration.
        /// </summary>
        /// <param name="sourceFile">The source MP3 file.</param>
        /// <param name="targetFile">The target MP3 file.</param>
        /// <param name="startPosition">The start position of the trim.</param>
        /// <param name="endPosition">The end position of the trim.</param>
        /// <param name="splitDuration">The duration of each split.</param>
        /// <param name="progressHandler"></param>
        public void Trim(string sourceFile, string targetFile, TimeSpan startPosition, TimeSpan endPosition, TimeSpan splitDuration, ProgressManager progressManager)
        {
            // Start at defined start position
            this.mp3Reader.CurrentTime = startPosition;
            var currentSplit = 1;

            while (this.mp3Reader.CurrentTime < endPosition)
            {
                var splitTime = this.mp3Reader.CurrentTime + splitDuration;
                var albumName = Path.GetFileNameWithoutExtension(targetFile);
                var directoryPath = Path.GetDirectoryName(targetFile);
                var trackName = albumName + $"-{currentSplit:D3}";
                var fileName = trackName + ".mp3";
                var filePath = Path.Combine(directoryPath, fileName);

                using (var writer = File.Create(filePath))
                {
                    // Write tags to file
                    var tags = GetTags(currentSplit, trackName, albumName);
                    writer.Write(tags.RawData, 0, tags.RawData.Length);

                    // Write audio to file
                    while (this.mp3Reader.CurrentTime < splitTime)
                    {
                        var frame = this.mp3Reader.ReadNextFrame();
                        if (frame == null) break;
                        writer.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                }

                var relativeCurrTime = this.mp3Reader.CurrentTime - startPosition;
                var relativeEndTime = endPosition - startPosition;
                var progress = Math.Round((double)relativeCurrTime.Ticks / relativeEndTime.Ticks * 100);

                progressManager.ProgressPercentage.Report((int)progress);
                progressManager.ProgressString.Report($"Written split [{currentSplit}]: {fileName}");
                currentSplit++;
            }
        }

        private static Id3v2Tag GetTags(int currentSplit, string trackName, string albumName)
        {
            var tagDictionary = new Dictionary<string, string>
            {
                {"TRCK", currentSplit.ToString()}, // Track number
                {"TIT2", trackName}, // Track name
                {"TALB", albumName} // Album name
            };

            var tags = Id3v2Tag.Create(tagDictionary);
            return tags;
        }

        public void SetId3Tag(string fileName, string key, string value)
        {
            this.mp3Reader?.Close();

            TagLib.File file = TagLib.File.Create(fileName);
            var tag = (Tag)file.GetTag(TagLib.TagTypes.Id3v2);
            tag.SetTextFrame(key, value);
            file.Save();
        }

        public void Dispose()
        {
            this.mp3Reader?.Dispose();
        }
    }
}
