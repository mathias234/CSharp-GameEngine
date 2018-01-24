using System;

namespace FileSystem
{
    internal class ExtensionAttribute : Attribute
    {
        private string v;

        public ExtensionAttribute(string v)
        {
            this.v = v;
        }
    }
}