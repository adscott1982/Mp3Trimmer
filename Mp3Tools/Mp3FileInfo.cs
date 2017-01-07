using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Id3v2;

namespace Mp3Tools
{
    public class Mp3FileInfo
    {
        public FileInfo FileInfo { get; set; }
        public Tag Id3V2Tag { get; set; }

        public Mp3FileInfo(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
            this.Id3V2Tag = (Tag)TagLib.File.Create(fileInfo.FullName).GetTag(TagLib.TagTypes.Id3v2);
        }
    }
}
