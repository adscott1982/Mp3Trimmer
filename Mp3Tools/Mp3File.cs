﻿// Unit tests required:
// - When source file does not exist or is invalid, exception is generated
// - When source file is not a valid MP3, exception is generated
// - When target location is not valid, exception is generated
// - When startPosition is after the length of the MP3, exception is generated
// - When endPosition is before the startPosition, exception is generated
// - When all parameters are valid, file is generated
// - When all parameters are valid, but endPosition is after the end of the file, file is generated

using System.Collections.Generic;

namespace Mp3Tools
{
    using System;
    using System.IO;
    using NAudio.Wave;
    using AndyTools.Utilities;

    /// <summary>
    /// Class which provides MP3 file operations.
    /// </summary>
    public class Mp3File
    {
        public string FilePath { get; }
        public string FileName { get; }
        public TimeSpan Length { get; }
        public float SizeMb { get; }

        public Mp3File(string path)
        {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists) throw new FileNotFoundException($"File not found: {path}");
            if (fileInfo.Extension.ToLowerInvariant() != ".mp3") throw new FileFormatException("File extension must be .MP3");

            this.FilePath = fileInfo.FullName;
            this.FileName = fileInfo.Name;
            this.SizeMb = fileInfo.Length / 1000000f;

            using (var mp3FileReader = new Mp3FileReader(path))
            {
                this.Length = mp3FileReader.TotalTime;
            }
        }

        /// <summary>
        /// Will trim an MP3 file to a target file based on provided parameters.
        /// </summary>
        /// <param name="sourceFile">The source MP3 file.</param>
        /// <param name="targetFile">The target MP3 file.</param>
        /// <param name="startPosition">The start position of the trim.</param>
        /// <param name="endPosition">The end position of the trim.</param>
        /// <param name="progressHandler"></param>
        public static void Trim(string sourceFile, string targetFile, TimeSpan startPosition, TimeSpan endPosition, ProgressManager progressManager)
        {
            using (var mp3FileReader = new Mp3FileReader(sourceFile))
            using (var writer = File.Create(targetFile))
            {
                mp3FileReader.CurrentTime = startPosition;

                while (mp3FileReader.CurrentTime < endPosition)
                {
                    var frame = mp3FileReader.ReadNextFrame();
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
        public static void Trim(string sourceFile, string targetFile, TimeSpan startPosition, TimeSpan endPosition, TimeSpan splitDuration, ProgressManager progressManager)
        {
            using (var mp3FileReader = new Mp3FileReader(sourceFile))
            {
                // Start at defined start position
                mp3FileReader.CurrentTime = startPosition;
                var currentSplit = 1;

                while (mp3FileReader.CurrentTime < endPosition)
                {
                    var splitTime = mp3FileReader.CurrentTime + splitDuration;
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
                        while (mp3FileReader.CurrentTime < splitTime)
                        {
                            var frame = mp3FileReader.ReadNextFrame();
                            if (frame == null) break;
                            writer.Write(frame.RawData, 0, frame.RawData.Length);
                        }
                    }

                    var relativeCurrTime = mp3FileReader.CurrentTime - startPosition;
                    var relativeEndTime = endPosition - startPosition;
                    var progress = Math.Round((double)relativeCurrTime.Ticks / relativeEndTime.Ticks * 100);

                    progressManager.ProgressPercentage.Report((int)progress);
                    progressManager.ProgressString.Report($"Written split [{currentSplit}]: {fileName}");
                    currentSplit++;
                }
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
    }
}
