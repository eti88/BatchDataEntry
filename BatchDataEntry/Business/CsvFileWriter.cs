using System.IO;
using System.Runtime.CompilerServices;
using System.Text;


namespace BatchDataEntry.Business
{
    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public char _separator;

        public CsvFileWriter(Stream stream) : base(stream)
        {
            this._separator = ';';
        }

        public CsvFileWriter(string filename) : base(filename)
        {
            this._separator = ';';
        }


        public CsvFileWriter(string filename, bool append) : base(filename, append)
        {      
            this._separator = ';';
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(_separator);
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                //if (value.IndexOfAny(new char[] { '"', separator }) != -1)
                //    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }
}
