using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib.Id3v2;

namespace Mp3Tools
{
    public class Mp3FileInfo
    {
        public FileInfo FileInfo { get; set; }
        public Tag Id3V2Tag { get; set; }
        private TagLib.File file;

        public Mp3FileInfo(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            this.file = TagLib.File.Create(fileInfo.FullName);
            this.Id3V2Tag = (Tag)file.GetTag(TagLib.TagTypes.Id3v2);
        }

        public void SetTagByFrameId(string frameId, string value)
        {
            Id3V2Tag.SetTextFrame(frameId, value);
        }

        public void SaveTag()
        {
            this.file.Save();
        }

        public static IEnumerable<Mp3FileInfo> GetMp3FilesFromPath(DirectoryInfo folder)
        {
            var files = folder.EnumerateFiles()
                .Where(f => f.Extension == ".mp3")
                .Select(f => new Mp3FileInfo(f))
                .OrderBy(f => f.Id3V2Tag.Track);

            return files;
        }
    }
}
