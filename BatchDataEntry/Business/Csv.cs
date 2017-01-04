using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Business
{
    public static class Csv
    {
        public static void CreateCsv(string fullPath)
        {
            File.Create(fullPath);
        }

        public static void AddRow(string fullPath, string row)
        {
            CsvWriter writer = new CsvWriter(new StreamWriter(fullPath));
            writer.WriteRecord(row);          
        }

        public static List<string> ReadRows(string fullPath)
        {
            return new CsvReader(new StreamReader(fullPath)).GetRecords<string>().ToList<string>();
        }

        public static List<string> ReadColumn(string fullPath, int columnNumber)
        {
            List<string> result = new List<string>();
            using (var fr = File.OpenText(fullPath))
            {
                using (CsvReader reader = new CsvReader(fr))
                {
                    while (reader.Read())
                    {
                        string field = reader.GetField<string>(columnNumber);
                        result.Add(field);
                    }
                }
            }

            return result;
        }

        public static bool DeleteRow(string file, int line)
        {

            try
            {
                string[] values = File.ReadAllLines(file);
                StreamWriter Writer = new StreamWriter(file);
                for (int i = 0; i < values.Length; i++)
                {
                    if (i == line)
                    {
                        continue;
                    }
                    Writer.WriteLine(values[i]);
                }

                Writer.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public static bool DeleteRow(string file, string line)
        {

            try
            {
                string[] values = File.ReadAllLines(file);
                StreamWriter Writer = new StreamWriter(file);
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Contains(line))
                        continue;
                    Writer.WriteLine(values[i]);
                }

                Writer.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool UpdateRow(string file, int line, string content)
        {
            try
            {
                string[] values = File.ReadAllLines(file);
                StreamWriter Writer = new StreamWriter(file);
                for (int i = 0; i < values.Length; i++)
                {
                    if (i == line)
                    {
                        Writer.WriteLine(content);
                        continue;
                    }
                    Writer.WriteLine(values[i]);
                }

                Writer.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateRow(string file, string original, string replace)
        {
            try
            {
                string[] values = File.ReadAllLines(file);
                StreamWriter Writer = new StreamWriter(file);
                for (int i = 0; i < values.Length; i++)
                {
                    if(values[i].Contains(original))
                        Writer.WriteLine(replace);

                    Writer.WriteLine(values[i]);
                }

                Writer.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
