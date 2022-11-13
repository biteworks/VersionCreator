using System;
using System.IO;

namespace VersionCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) { 
                foreach (var passendFile in args)
                {
                    FileAttributes attr = File.GetAttributes(@passendFile);
                    Console.WriteLine(passendFile);
                }
            }
            else
            {
                Console.WriteLine("No arguments passed.");
            }
            Console.ReadLine();
        }
        static public void CopyFolder(string sourceFolder, string destFolder)
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
    }
}
