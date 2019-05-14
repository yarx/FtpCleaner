using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Directory = FtpCleaner.Models.Directory;
using File = FtpCleaner.Models.File;

namespace FtpCleaner.Services
{
    public class FtpService
    {
        private readonly string _url;
        private readonly NetworkCredential _credentials;

        // regex pattern which identifies all characters which are not part of the filename
        // https://stackoverflow.com/questions/39704256/parsing-file-name-out-of-listdirectorydetails-with-regex
        private readonly Regex _ftpRegex = new Regex("^(?:[^ ]+ +){8}(.*)$");

        public FtpService(string url, string username, string password)
        {
            _url = url;
            _credentials = new NetworkCredential(username, password);
        }

        public Directory GetDirectory(string path)
        {
            // create directory list request and read the content of the response
            FtpWebRequest listRequest = CreateFtpRequest(path, WebRequestMethods.Ftp.ListDirectoryDetails);
            FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();

            Stream responseStream = listResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string namesString = reader.ReadToEnd();

            reader.Close();
            listResponse.Close();

            // create an array which contains each line of the response
            var names = namesString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // create the top level directory
            var directory = new Directory(path);

            // fill up the whole directory recursively
            foreach (var entry in names)
            {
                // trim response line
                var trimmedString = entry.Trim('"');

                // determine if its a directory or file
                bool isDirectory = trimmedString[0] == 'd';

                // remove all characters which are not part of the name with regex pattern
                var nameMatch = _ftpRegex.Match(trimmedString);

                if (!nameMatch.Success)
                    continue;

                var name = nameMatch.Groups[1].Value;

                // do not list directories with . or ..
                if (name == "." || name == "..")
                    continue;

                // recursively go through all subdirectories
                if (isDirectory)
                {
                    Console.WriteLine($"found directory: {path}{name}/");
                    directory.Directories.Add(GetDirectory(path + name + "/"));
                }
                else
                {
                    Console.WriteLine($"found file: {name}");
                    directory.Files.Add(new File(name));
                }
            }

            return directory;
        }

        public Directory CleanupDirectory(Directory directory)
        {
            // deletes every file in the specified directory
            foreach (var file in directory.Files)
                DeleteFile(directory.Name + file.Name);

            // deletes recursively all files and at the end the directory
            foreach (var dir in directory.Directories)
            {
                var toDeleteDirectory = CleanupDirectory(dir);
                DeleteDirectory(toDeleteDirectory.Name);
            }

            return directory;
        }

        public void DeleteDirectory(string path)
        {
            var deleteDirRequest = CreateFtpRequest(path, WebRequestMethods.Ftp.RemoveDirectory);
            try
            {
                var response = (FtpWebResponse)deleteDirRequest.GetResponse();
                Console.WriteLine($"deleted directory: {_url}{path} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"could not delete directory: {_url}{path} | {ex.Message}");
            }
        }

        public void DeleteFile(string path)
        {
            var deleteRequest = CreateFtpRequest(path, WebRequestMethods.Ftp.DeleteFile);
            try
            {
                var response = (FtpWebResponse)deleteRequest.GetResponse();
                Console.WriteLine($"deleted file: {_url}{path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"could not delete file: {_url}{path} | {ex.Message}");
            }
        }

        private FtpWebRequest CreateFtpRequest(string path, string requestMethode)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_url + path);
            request.Credentials = _credentials;
            request.Method = requestMethode;
            return request;
        }

    }
}
