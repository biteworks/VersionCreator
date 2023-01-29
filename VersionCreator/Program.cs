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
                    Console.WriteLine(passedObj);

                    //Regex patterns for the date formats YYYY-mm-dd, YYYYmmdd and YYmmdd
                    string[] patterns = { 
                        @"\b([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))",
                        @"\b[0-9][0-9][0-9][0-9][0-1][0-9][0-3][0-9]",
                        @"\b[0-9][0-9][0-1][0-9][0-3][0-9]"
                    };

                    string versionPattern = @"_[0-9][0-9][0-9]\b";

                    //File
                    if (!attr.HasFlag(FileAttributes.Directory))
                    {
                        string pathStripped = Path.GetDirectoryName(passedObj);
                        string fileName = Path.GetFileName(passedObj);
                        string newFilePath;

                        // Dateformat YYYY-mm-dd
                        if (Regex.IsMatch(fileName, patterns[0]))
                        {
                            Console.WriteLine("Pattern 1");
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 10);
                        }
                        // Dateformat YYYYmmdd
                        else if (Regex.IsMatch(fileName, patterns[1]))
                        {
                            Console.WriteLine("Pattern 2");
                            string date = DateTime.Now.ToString("yyyyMMdd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 8);
                        }
                        // Dateformat YYmmdd
                        else if (Regex.IsMatch(fileName, patterns[2]))
                        {
                            Console.WriteLine("Pattern 3");
                            string date = DateTime.Now.ToString("yyMMdd");
                            newFilePath = pathStripped + "\\" + date + fileName.Remove(0, 6);
                        }
                        else
                        {
                            Console.WriteLine("Kein pattern?!");
                            string date = DateTime.Now.ToString("yyyy-MM-dd");
                            newFilePath = pathStripped + "\\" + date + "_" + fileName;
                        }

                        // Check if file with new name exist
                        if (!File.Exists(@newFilePath))
                        {
                            File.Copy(passedObj, newFilePath);
                            Console.WriteLine("File Duplicated with todays date.");
                        }
                        // If file already exists, increment version number
                        else
                        {
                            if (Regex.IsMatch(newFilePath, versionPattern))
                            {
                                Match currentVersion = Regex.Match(newFilePath, versionPattern);
                                string newFilePathWithVersionNumber = IncrementVersionNumber(newFilePath, currentVersion.Value);
                                File.Copy(passedObj, newFilePathWithVersionNumber);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR:\n\nThe file already exists and the version number cannot be incremented.\nProbably the pattern of the version numbering does not fit.");
                            }
                        }
                    }
                    //Folder
                    /*else
                    {
                        string folderAbove = System.IO.Directory.GetParent(passedObj).ToString();
                        string folderName = passedObj.Replace(folderAbove + "\\", "");
                        string newFolderPath;

                        if (Regex.IsMatch(folderName, patterns[0]))
                        {
                            newFolderPath = folderAbove + "\\" + date + folderName.Remove(0, 10);
                        }
                        else
                        {
                            newFolderPath = folderAbove + "\\" + date + "_" + folderName;
                        }

                        if (!Directory.Exists(@newFolderPath))
                        {
                            CopyFolder(passedObj, newFolderPath);
                            Console.WriteLine("Folder Duplicated with todays date.");
                        }
                        else
                        {
                            // TODO add version counter at the end
                            Console.WriteLine("Folder already exists!");
                        }

                    }*/
                }
            }
            else
            {
                Console.WriteLine("No arguments passed.");
            }
            Console.ReadLine();
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
