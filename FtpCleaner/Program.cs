using FtpCleaner.Services;
using System;

namespace FtpCleaner
{
    class MainClass
    {
        private static string url;
        private static string username;
        private static string password;      

        public static void Main(string[] args)
        {
            if (args.Length != 3)
                throw new Exception("Please specify Url, Username and Password as arguments.");

            url = args[0];
            username = args[1];
            password = args[2];

            var ftpService = new FtpService(url, username, password);

            Console.WriteLine("start ftp cleanup");
            var dir = ftpService.GetDirectory("/");
            Console.WriteLine("---------------------");
            ftpService.CleanupDirectory(dir);
            Console.WriteLine("finished ftp cleanup");
        }
    }
}
