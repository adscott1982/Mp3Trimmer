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
        public static void Trim(Mp3File sourceFile, string targetFile, TimeSpan startPosition, TimeSpan endPosition)
        {
            using (var mp3FileReader = new Mp3FileReader(sourceFile.FilePath))
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
    }
}
