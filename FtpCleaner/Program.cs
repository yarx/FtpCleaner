using FtpCleaner.Configuration;
using FtpCleaner.Models;
using FtpCleaner.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FtpCleaner
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-path", "Path" },
                { "-host", "Url" },
                { "-user", "Username" },
                { "-pw", "Password" },
            };

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(typeof(MainClass).Assembly)
                .AddCommandLine(args, switchMappings)
                .Build();

            // check if ftp action is specified;
            FtpAction action;
            try { action = Enum.Parse<FtpAction>(args[0], true); }
            catch { throw new Exception("Please specify FTP Action (clean or upload) as a first argument."); }

            var ftpInput = new FtpInput
            {
                Action = action,
                Path = configuration["Path"],
                Url = configuration["Url"],
                Username = configuration["Username"],
                Password = configuration["Password"]
            };

            var ftpService = new FtpService(ftpInput.Url, ftpInput.Username, ftpInput.Password);

            await ((ftpInput.Action, ftpInput.Path) switch
            {
                (FtpAction.Clean, _) => ftpService.Clean(),
                (FtpAction.Upload, null) => ftpService.Upload(),
                (FtpAction.Upload, _) => ftpService.Upload(ftpInput.Path),
                _ => throw new Exception("Input seems not to be correct. Please verify command line arguments.")
            });
        }
    }
}
