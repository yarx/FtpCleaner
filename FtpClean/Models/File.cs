namespace FtpCleaner.Models
{
    public class File
    {
        public File(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
