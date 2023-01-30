using System;
using System.IO;
using System.Text.RegularExpressions;

namespace VersionCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) { 
                foreach (var passedObj in args)
                {
                    // Get objects attributes
                    FileAttributes attr = File.GetAttributes(@passedObj);
                    Console.WriteLine("Passed obj: " + passedObj);

                    //Regex patterns for the date formats YYYY-mm-dd, YYYYmmdd and YYmmdd
                    string[] patterns = { 
                        @"\b([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))",
                        @"\b[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]",
                        @"\b[0-9][0-9][0-1][0-9][0-3][0-9]"
                    };

                    string versionPattern = @"_[0-9][0-9][0-9]\b";

                    //Is file
                    if (!attr.HasFlag(FileAttributes.Directory))
                    {
                        string pathStripped = Path.GetDirectoryName(passedObj);
                        string fileName = Path.GetFileName(passedObj);
                        string newFilePath;

                        // Date format YYYY-mm-dd
                        if (Regex.IsMatch(fileName, patterns[0]))
                        {
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 10);
                        }
                        // Date format YYYYmmdd
                        else if (Regex.IsMatch(fileName, patterns[1]))
                        {
                            string date = DateTime.Now.ToString("yyyyMMdd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 8);
                        }
                        // Date format YYmmdd
                        else if (Regex.IsMatch(fileName, patterns[2]))
                        {
                            string date = DateTime.Now.ToString("yyMMdd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 6);
                        }
                        // No date
                        else
                        {
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFilePath = pathStripped + "\\" + date + "_" + fileName;
                        }

                        // If file with current date doesnt exist, copy it
                        if (!File.Exists(@newFilePath))
                        {
                            File.Copy(passedObj, newFilePath);
                            Console.WriteLine("File Duplicated with todays date.");
                        }
                        // If file already exists, increment or add version number
                        else
                        {
                            if (Regex.IsMatch(newFilePath, versionPattern))
                            {
                                // Increment existing version number
                                Match currentVersion = Regex.Match(newFilePath, versionPattern);
                                string newFilePathWithVersionNumber = IncrementVersionNumber(newFilePath, currentVersion.Value);
                                File.Copy(passedObj, newFilePathWithVersionNumber);
                            }
                            else
                            {
                                // Add new version number of 002
                                var ext = Path.GetExtension(newFilePath);
                                var tempFileName = Path.GetFileNameWithoutExtension(newFilePath);
                                File.Copy(passedObj, newFilePath.Replace(tempFileName, tempFileName + "_002" + ext));
                            }
                        }
                    }
                    //Is folder
                    else
                    {
                        string folderAbove = System.IO.Directory.GetParent(passedObj).ToString();
                        string folderName = passedObj.Replace(folderAbove + "\\", "");
                        string newFolderPath;

                        // Date format YYYY-mm-dd
                        if (Regex.IsMatch(folderName, patterns[0]))
                        {
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFolderPath = folderAbove + "\\" + date + folderName.Remove(0, 10);
                        }
                        // Date format YYYYmmdd
                        else if (Regex.IsMatch(folderName, patterns[1]))
                        {
                            string date = DateTime.Now.ToString("yyyyMMdd");
                            newFolderPath = folderAbove + "\\" + date + folderName.Remove(0, 8);
                        }
                        // Date format YYmmdd
                        else if (Regex.IsMatch(folderName, patterns[2]))
                        {
                            string date = DateTime.Now.ToString("yyMMdd");
                            newFolderPath = folderAbove + "\\" + date + folderName.Remove(0, 6);
                        }
                        // No date
                        else
                        {
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFolderPath = folderAbove + "\\" + date + "_" + folderName;
                        }
                        // If folder with current date doesnt exist, copy it
                        if (!Directory.Exists(@newFolderPath))
                        {
                            CopyFolder(passedObj, newFolderPath);
                            Console.WriteLine("Folder Duplicated with todays date.");
                        }
                        // If folder already exists, increment or add version number
                        else
                        {
                            if (Regex.IsMatch(newFolderPath, versionPattern))
                            {
                                Match currentVersion = Regex.Match(newFolderPath, versionPattern);
                                string newFolderPathWithVersionNumber = IncrementVersionNumber(newFolderPath, currentVersion.Value);
                                CopyFolder(passedObj, newFolderPathWithVersionNumber);
                            }
                            else 
                            {
                                CopyFolder(passedObj, newFolderPath + "_002");
                            }
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("No arguments passed.");
            }
        }
        static private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        static private String IncrementVersionNumber(string fileName, string currentVersion)
        {
            string cleanedVersion = currentVersion.Replace("_", "").TrimStart('0');
            int newVersion = Int16.Parse(cleanedVersion) + 1;
            return fileName.Replace(currentVersion, "_" + newVersion.ToString("D3"));
        }
    }
}
