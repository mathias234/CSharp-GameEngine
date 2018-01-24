using System;

namespace FileSystem
{
    internal class FileExtensionAttribute : Attribute
    {
        public string Extension { get; set; }

        public FileExtensionAttribute(string ext)
        {
            Extension = ext;
        }
    }
}