using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class CsvTest
    {
        private static readonly string _FILENAME = "test.csv";
        private readonly string _PATHFILE = Path.Combine(System.Environment.CurrentDirectory, _FILENAME);
        private readonly string _PATHFILE2 = Path.Combine(System.Environment.CurrentDirectory, "test2.csv");
        // 4 colonne per i test

        [TestMethod]
        public void CreateTest()
        {
            try
            {
                BatchDataEntry.Business.Csv.CreateCsv(_PATHFILE);
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void AddRowTest()
        {
            try
            {
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000001;Mario;Rossi;44");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000002;Luigi;Bianchi;51");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000003;Marco;Verdi;35");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000004;Luca;Rossi;19");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000005;Mario;Bianchi;44");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000006;Luigi;Verdi;51");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000007;Marco;Bianchi;35");
                BatchDataEntry.Business.Csv.AddRow(_PATHFILE, "000008;Luca;Verdi;19");
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void AddRowsTest()
        {
            try
            {
                List<string> records = new List<string>();
                records.Add("000001;Mario;Rossi;44");
                records.Add("000002;Luigi;Bianchi;51");
                records.Add("000003;Marco;Verdi;35");
                records.Add("000004;Luca;Rossi;19");
                
                BatchDataEntry.Business.Csv.AddRows(_PATHFILE2, records);
                
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void ReadRowsTest()
        {
            try
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                foreach (string r in lst)
                {
                    Console.WriteLine(r);
                }

                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void ReadColumnTest1()
        {
            try
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadColumn(_PATHFILE, 0);
                foreach (string r in lst)
                {
                    Console.WriteLine(r);
                }

                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void ReadColumnTest2()
        {
            try
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadColumn(_PATHFILE, 3);
                foreach (string r in lst)
                {
                    Console.WriteLine(r);
                }

                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void DeleteRowTest1()
        {
            if (BatchDataEntry.Business.Csv.DeleteRow(_PATHFILE, 7))
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                if (lst.Contains("000008"))
                    Assert.IsTrue(false);

                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void DeleteRowTest2()
        {
            if (BatchDataEntry.Business.Csv.DeleteRow(_PATHFILE, "000001;Mario;Rossi;44"))
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                if (lst.Contains("000001"))
                    Assert.IsTrue(false);

                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void DeleteRowTest3()
        {
            if (BatchDataEntry.Business.Csv.DeleteRow(_PATHFILE, "000006", 0))
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                if (lst.Contains("000006"))
                    Assert.IsTrue(false);

                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void UpdateRowTest1()
        {
            if (BatchDataEntry.Business.Csv.UpdateRow(_PATHFILE, 3, "999933;Jonh;Doe;33"))
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                
                if (lst.Any(s => s.Contains("999933")))
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void UpdateRowTest2()
        {
            if (BatchDataEntry.Business.Csv.UpdateRow(_PATHFILE, "000002;Luigi;Bianchi;51", "092222;Luigi;Bianchi;51"))
            {
                List<string> lst = BatchDataEntry.Business.Csv.ReadRows(_PATHFILE);
                
                if (lst.Any(s => s.Contains("092222")))
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void SplitRowTest()
        {
            string[] expected = new string[] {"000001", "Mario", "Rossi", "44"};

            string line = "000001;Mario;Rossi;44";
            string[] cells = BatchDataEntry.Business.Csv.CSVRowToStringArray(line);
            if(cells.Length == 0)
                Assert.IsTrue(false, "Lunghezza array celle uguale a 0");
            for (int i = 0; i < cells.Length; i++)
            {
                if(string.IsNullOrEmpty(cells[i]))
                    Assert.IsTrue(false, "Contenuto celle vuoto");
                if(cells[i] != expected[i])
                    Assert.IsTrue(false, "Contenuto celle diverso da quello previsto");
            }
            Assert.IsTrue(true);
        }
    }
}
