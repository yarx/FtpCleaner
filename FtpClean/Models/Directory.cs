using System.Collections.Generic;

namespace FtpCleaner.Models
{
    public class Directory
    {
        public Directory(string name)
        {
            Name = name;
            Directories = new List<Directory>();
            Files = new List<File>();
        }

        public string Name { get; private set; }
        public List<Directory> Directories { get; private set; }
        public List<File> Files { get; private set; }
    }
}
