using System;
using System.IO;

namespace PH.Babel2.Events
{
    public class SourceEntriesChangedEventArgs : EventArgs
    {
        public string FileChangedFullName { get; set; }
        public DateTime UtcFileLastWrite { get; set; }
        internal  FileSystemEventArgs BaseArgs { get; set; }
    }
}