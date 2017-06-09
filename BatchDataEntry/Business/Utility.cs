using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BatchDataEntry.Models;
using System.Security.Cryptography;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace BatchDataEntry.Business
{
    public static class Utility
    {
        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        public static IEnumerable<string> CustomSort(this IEnumerable<string> list)
        {
            int maxLen = list.Select(s => s.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }

        public static IEnumerable<Document> CustomSort(this IEnumerable<Document> list)
        {
            int maxLen = list.Select(s => s.FileName.Length).Max();

            return list.Select(s => new
            {
                OrgStr = s,
                SortStr = Regex.Replace(s.FileName, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, char.IsDigit(m.Value[0]) ? ' ' : '\xffff'))
            })
            .OrderBy(x => x.SortStr)
            .Select(x => x.OrgStr);
        }

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

        public static void CopiaFile(string source_file, string destination_path, string newFileName)
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

        public static bool IsNotVoid(string text)
        {
            bool res;
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                res = false;
            else
                res = true;
            return res;
        }

        public static bool IsValidTelephone(string num)
        {
            return IsNumeric(num) && num.Length == 10;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (char.IsSymbol(email[0])) return false;
                if (!email.Contains("@")) return false;
                char c = email[email.IndexOf('@') - 1];
                char d = email[email.IndexOf('@') + 1];
                if (char.IsSymbol(c) || c == '.' || c == ';' || c == ':' || c == '-') return false;
                if (char.IsSymbol(d) || d == '.' || d == ';' || d == ':' || d == '-') return false;
                if (email.Contains(" ")) return false;
                if (email.Contains("..")) return false;
                if (!addr.Host.Contains(".")) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidDate(string date, string dateFormat)
        {
            bool res = false;
            DateTime dt;
            if (DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                res = true;
            }
            return res;
        }

        private static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static string GetTmpPathFile(string file)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string tmppath = Path.GetTempPath();
            return Path.Combine(tmppath, string.Format("{0}.pdf", filename));
        }
    }
}
