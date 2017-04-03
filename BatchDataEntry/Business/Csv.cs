using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;

namespace BatchDataEntry.Business
{
    public static class Csv
    {
        public static void CreateCsv(string fullPath)
        {
            File.Create(fullPath).Dispose();
        }

        public static void AddRow(string fullPath, string line)
        {
            using (CsvFileWriter writer = new CsvFileWriter(fullPath, true))
            {
                writer._separator = ';';
                CsvRow row = new CsvRow();
                row.Add(line);
                writer.WriteRow(row);
            }       
        }

        public static void AddRows(string fullPath, IEnumerable records)
        {
            using (CsvFileWriter writer = new CsvFileWriter(fullPath))
            {
                writer._separator = ';';
                foreach (var record in records)
                {
                    CsvRow row = new CsvRow();
                    row.Add(record.ToString());
                    writer.WriteRow(row);
                }
            }
        }

        public static List<string> ReadRows(string fullPath, char separator)
        {
            List < string > result = new List<string>();
            using (CsvFileReader reader = new CsvFileReader(fullPath))
            {
                reader._separator = separator;
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    string line = "";
                    for(int i=0;i<row.Count;i++)
                    {
                        if (i == (row.Count - 1))
                            line += row[i];
                        else
                            line += row[i] + reader._separator;
                    }
                    result.Add(line);
                }
                return result;
            }
        }

        public static List<string> ReadColumn(string fullPath, int columnNumber)
        {
            List<string> result = new List<string>();
            using (CsvFileReader reader = new CsvFileReader(fullPath))
            {
                reader._separator = ';';
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    result.Add(row[columnNumber]);
                }
            }
            return result;
        }

        public static string[,] ReadColumn(string fullPath, int col1, int col2)
        {
            if (!File.Exists(fullPath))
                return null;

            string[,] result = new string[File.ReadAllLines(fullPath).Length, 2];
            using (var sr = new StreamReader(fullPath))
            {
                var csv = new CsvReader(sr);
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasHeaderRecord = false;
                int i = 0;
                while (csv.Read())
                {
                    result[i, 0] = String.Format("{0}", csv.GetField<string>(col1));
                    result[i, 1] = String.Format("{0}", csv.GetField<string>(col2));
                    i++;
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

        public static bool DeleteRow(string file, string val, int colToSearch)
        {

            try
            {
                string[] values = File.ReadAllLines(file);
                StreamWriter Writer = new StreamWriter(file);
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Contains(val))
                    {
                        string[] cells = values[i].Split(';');
                        if (cells[colToSearch] == val)
                        {
                            continue;
                        }
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

        public static bool UpdateRow(string file, int line, string content)
        {
            try
            {
                List<string> rows = ReadRows(file, ';');
                using (CsvFileWriter writer = new CsvFileWriter(file))
                {
                    writer._separator = ';';

                    for (int i = 0; i < rows.Count; i++)
                    {
                        CsvRow r = new CsvRow();
                        if (i == line)
                        {
                            r.Add(content);
                        }
                        else
                        {
                            r.Add(rows[i]);
                        }
                        writer.WriteRow(r);
                    }
                    writer.Close();
                    writer.Dispose();
                }             
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
                List<string> rows = ReadRows(file, ';');
                using (CsvFileWriter writer = new CsvFileWriter(file))
                {
                    writer._separator = ';';

                    for (int i = 0; i < rows.Count; i++)
                    {
                        CsvRow r = new CsvRow();
                        if (rows[i] == original)
                        {
                            r.Add(replace);
                        }
                        else
                        {
                            r.Add(rows[i]);
                        }
                        writer.WriteRow(r);
                    }
                    writer.Close();
                    writer.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<string> SearchRow(string file, string searchText, int col, char separator = ';')
        {
            List<string> row = new List<string>();

            using (CsvFileReader reader = new CsvFileReader(file))
            {
                reader._separator = separator;
                CsvRow r = new CsvRow();
                while (reader.ReadRow(r))
                {
                    if (r.Contains(searchText))
                    {
                        if (col < r.Count)
                        {
                            if (r[col].Equals(searchText))
                            {
                                row = r.ToList();
                                return row;
                            }
                        }
                    }
                }
            }

            return row;
        }

        public static List<string> SearchRow(string file, string searchTextCol1, string searchTextCol2, int col1, int col2, char separator = ';')
        {
            List<string> row = new List<string>();

            using (CsvFileReader reader = new CsvFileReader(file))
            {
                reader._separator = separator;
                CsvRow r = new CsvRow();
                while (reader.ReadRow(r))
                {
                    if (r.Contains(searchTextCol1) && r.Contains(searchTextCol2))
                    {
                        if (r[col1].Equals(searchTextCol1) && r[col2].Equals(searchTextCol2))
                        {
                            row = r.ToList();
                            return row;
                        }
                    }
                }
            }

            return row;
        }

        public static string[] CSVRowToStringArray(string r, char fieldSep = ';', char stringSep = '\"')
        {
            bool bolQuote = false;
            StringBuilder bld = new StringBuilder();
            List<string> retAry = new List<string>();

            foreach (char c in r.ToCharArray())
                if ((c == fieldSep && !bolQuote))
                {
                    retAry.Add(bld.ToString());
                    bld.Clear();
                }
                else
                    if (c == stringSep)
                    bolQuote = !bolQuote;
                else
                    bld.Append(c);

            /* to solve the last element problem */
            retAry.Add(bld.ToString()); /* added this line */
            return retAry.ToArray();
        }
    }
}
