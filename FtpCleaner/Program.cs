using FtpCleaner.Services;
using System;

namespace FtpCleaner
{
    class MainClass
    {
        private static string url;// = "ftp://sl76.web.hostpoint.ch";
        private static string username;// = "fahrxl@yarx.ch";
        private static string password;// = "zrNyKXnDE8y238cmvkdjVwr8CzNW4TdC";        

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
