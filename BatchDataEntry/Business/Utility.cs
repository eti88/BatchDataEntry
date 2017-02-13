using System;
using System.IO;
using System.Linq;
using BatchDataEntry.Models;

namespace BatchDataEntry.Business
{
    public static class Utility
    {
        public static long GetDirectorySize(string fullDirectoryPath)
        {
            long startDirectorySize = 0;
            if (!Directory.Exists(fullDirectoryPath))
                return startDirectorySize; //Return 0 while Directory does not exist.

            var currentDirectory = new DirectoryInfo(fullDirectoryPath);
            //Add size of files in the Current Directory to main size.
            currentDirectory.GetFiles().ToList().ForEach(f => startDirectorySize += f.Length);

            //Loop on Sub Direcotries in the Current Directory and Calculate it's files size.
            currentDirectory.GetDirectories().ToList()
                .ForEach(d => startDirectorySize += GetDirectorySize(d.FullName));

            return startDirectorySize;  //Return full Size of this Directory.
        }

        public static int CountFiles(string path, TipoFileProcessato ext)
        {
            DirectoryInfo dinfo = new DirectoryInfo(path);
            return dinfo.GetFiles(string.Format("*.{0}", ext)).Length;
        }

        public static double ConvertSize(double bytes, string type)
        {
            try
            {
                const int CONVERSION_VALUE = 1024;
                //determine what conversion they want
                switch (type)
                {
                    case "BY":
                        //convert to bytes (default)
                        return bytes;
                    case "KB":
                        //convert to kilobytes
                        return (bytes / CONVERSION_VALUE);
                    case "MB":
                        //convert to megabytes
                        return (bytes / CalculateSquare(CONVERSION_VALUE));
                    case "GB":
                        //convert to gigabytes
                        return (bytes / CalculateCube(CONVERSION_VALUE));
                    default:
                        //default
                        return bytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private static double CalculateSquare(Int32 number)
        {
            return Math.Pow(number, 2);
        }

        private static double CalculateCube(Int32 number)
        {
            return Math.Pow(number, 3);
        }

        public static void CopiaPdf(string source_file, string destination_path, string newFileName)
        {
            string destination = Path.Combine(destination_path, newFileName);
            if(!File.Exists(destination))
                File.Copy(source_file, destination);
        }

        public static void DeletePdf(string pathFile)
        {
            if (File.Exists(pathFile))
                File.Delete(pathFile);
        }

        public static bool ContainsOnlyNumbers(string text)
        {
            if (text.Length == 0)
                return true;
            if (Char.IsLetter(text.ElementAt(text.Length - 1)))
                return false;
            else
                return ContainsOnlyNumbers(text.Remove(text.Length - 1));
        }

        public static string RemovePatternFromString(string origin, string pattern)
        {
            return origin.Replace(pattern, "");
        }
    }
}
