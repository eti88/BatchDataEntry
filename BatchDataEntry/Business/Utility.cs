using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using BatchDataEntry.Models;
using NLog;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

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
                        break;
                    case "KB":
                        //convert to kilobytes
                        return (bytes / CONVERSION_VALUE);
                        break;
                    case "MB":
                        //convert to megabytes
                        return (bytes / CalculateSquare(CONVERSION_VALUE));
                        break;
                    case "GB":
                        //convert to gigabytes
                        return (bytes / CalculateCube(CONVERSION_VALUE));
                        break;
                    default:
                        //default
                        return bytes;
                        break;
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
            File.Copy(source_file, destination);
        }

        public static void DeletePdf(string pathFile)
        {
            File.Delete(pathFile);
        }

        public static string ConvertTiffToPdf(string source_dir, string output_dir, string newFileName)
        {
            string destination = Path.Combine(output_dir, newFileName);
            string[] files = Directory.GetFiles(source_dir, "*.tiff");
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                Image[] tiffs = new Image[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    tiffs[i] = Image.FromFile(files[i]);
                }

                PdfDocument doc = new PdfDocument();
                for (int index = 0; index < tiffs.Length; index++)
                {
                    PdfPage page = new PdfPage();
                    XImage img = XImage.FromGdiPlusImage(tiffs[index]);

                    if (img.PointWidth > img.PointHeight)
                    {
                        page.Orientation = PageOrientation.Landscape;
                    }
                    else
                    {
                        page.Orientation = PageOrientation.Portrait;
                    }

                    page.Width = img.PointWidth;
                    page.Height = img.PointHeight;
                    doc.Pages.Add(page);
                    XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[index]);
                }
                doc.Save(destination);
                doc.Close();
                #if DEBUG
                Console.WriteLine("Generato pdf: " + destination);
                #endif
                return destination;
            }
            catch (Exception e)
            {
                logger.Error(string.Format("[{0}] {1}", destination, e.ToString()));
                return null;
            }
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

        public static DynamicClass GenerateClass(ObservableCollection<Campo> campi)
        {
            var fields = new List<Field>();
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                foreach (Campo c in campi)
                {
                    fields.Add(new Field(c.Nome, typeof(string)));
                }

                if (fields.Count == 0)
                    return null;

                dynamic obj = new DynamicClass(fields);
                return obj;
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(@"=== Error Generate Dynamic Class ===");
                Console.WriteLine(e.ToString());
                #endif
                logger.Error(e.ToString());
            }
            
            return null;
        }
    }
}
