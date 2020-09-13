using FluentFTP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FtpCleaner.Services
{
    public class FtpService
    {
        private readonly FtpClient _ftpClient;

        public FtpService(string url, string username, string password)
        {
            if (url is null || username is null || password is null)
            {
                throw new Exception("Please specify correct credentials '-host', '-user' and '-pw'");
            }
            _ftpClient = new FtpClient(url, username, password);
        }

        public async Task Clean()
        {
            Console.WriteLine("connect to FTP");
            await _ftpClient.ConnectAsync();
            Console.WriteLine("successfully connected to FTP");
            await _ftpClient.DeleteDirectoryAsync("/");
            Console.WriteLine("deleted all files on FTP");
        }

        public async Task Upload()
        {
            var currentDirectory = Environment.CurrentDirectory;
            await UploadFiles(currentDirectory);
        }

        public async Task Upload(string path)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var fullPath = Path.Combine(currentDirectory, path);
            await UploadFiles(fullPath);
        }

        private async Task UploadFiles(string path)
        {
            Console.WriteLine("connect to FTP");
            await _ftpClient.ConnectAsync();
            Console.WriteLine("successfully connected to FTP");
            await _ftpClient.UploadDirectoryAsync(path, "/", FtpFolderSyncMode.Mirror);
            Console.WriteLine("uploaded all files to FTP");
        }
    }
}
