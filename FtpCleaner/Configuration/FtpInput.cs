using FtpCleaner.Models;

namespace FtpCleaner.Configuration
{
    public class FtpInput
    {
        public FtpAction Action { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
