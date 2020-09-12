using FluentFTP;
using FtpCleaner.Models;
using FtpCleaner.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FtpCleaner
{
    class MainClass
    {
        private static string url;
        private static string username;
        private static string password;

        public static async Task Main(string[] args)
        {
            if (args.Length < 3 || args.Length == 4 || args.Length > 5)
                throw new Exception("Please specify Url, Username and Password or Action, Path, Url, Username and Password as arguments.");

            if (args.Length == 3)
            {
                url = args[0];
                username = args[1];
                password = args[2];

                var token = new CancellationToken();
                using (var conn = new FtpClient(url, username, password))
                {
                    await conn.ConnectAsync(token);

                    // Remove the directory and all files and subdirectories inside it
                    await conn.DeleteDirectoryAsync("/", token);
                }
            }

            if (args.Length == 5)
            {
                FtpAction action = Enum.Parse<FtpAction>(args[0], true);
                var path = args[1];
                url = args[2];
                username = args[3];
                password = args[4];

                if (action == FtpAction.Upload)
                {
                    var current = Environment.CurrentDirectory;
                    var uploadDir = path == "/" ? current : System.IO.Path.Combine(current, path);

                    var token = new CancellationToken();
                    using (var ftp = new FtpClient(url, username, password))
                    {
                        await ftp.ConnectAsync(token);
                        await ftp.UploadDirectoryAsync(uploadDir, "/", FtpFolderSyncMode.Mirror);
                    }
                }
            }
        }
    }
}
