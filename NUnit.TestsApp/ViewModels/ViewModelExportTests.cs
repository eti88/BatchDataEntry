using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelExportTests
    {
        private ViewModelExport vm;
        private static string local_path = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles";
        private static string sampleCsvFile = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\origin";
        private static string output_file_name = @"exported.csv";

        [Test(), Order(1)]
        public void ViewModelExportTest()
        {
            ViewModelExport v = new ViewModelExport();
            Assert.IsNotNull(v);
            Assert.IsNotNull(v.ColumnList);
        }

        [Test(), Order(2)]
        public void ViewModelExportTest1()
        {
            string path = Path.Combine(local_path, output_file_name);

            DataTable dt = GetDataTableFromCsv(sampleCsvFile, false);
            Assert.IsNotNull(dt);
            vm = new ViewModelExport(dt, path);

        }

        [Test(), Order(3)]
        public void GenerateCsvTest()
        {
            Assert.IsNotNull(vm);
            vm.GenerateCsv();
            Assert.IsTrue(File.Exists(Path.Combine(local_path, output_file_name)));
            Assert.IsTrue(File.ReadAllLines(sampleCsvFile).Count() == File.ReadAllLines(Path.Combine(local_path, output_file_name)).Count());
        }

        private static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}
