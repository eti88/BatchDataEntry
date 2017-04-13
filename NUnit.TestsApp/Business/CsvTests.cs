using NUnit.Framework;
using BatchDataEntry.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BatchDataEntry.Business.Tests
{
    [TestFixture()]
    public class CsvTests
    {
        private static readonly string _FILENAME = "test.csv";
        private readonly string _PATHFILE = Path.Combine(Directory.GetCurrentDirectory(), _FILENAME);
        private readonly string _PATHFILE2 = Path.Combine(Directory.GetCurrentDirectory(), "test2.csv");

        [Test()]

        public void CreateCsvTest()
        {
            Csv.CreateCsv(_PATHFILE);
            Assert.IsTrue(File.Exists(_PATHFILE));
        }

        [Test()]
        public void AddRowTest()
        {
            Csv.AddRow(_PATHFILE, "000001;Mario;Rossi;44");
            Csv.AddRow(_PATHFILE, "000002;Luigi;Bianchi;51");
            Csv.AddRow(_PATHFILE, "000003;Marco;Verdi;35");
            Csv.AddRow(_PATHFILE, "000004;Luca;Rossi;19");
            Csv.AddRow(_PATHFILE, "000005;Mario;Bianchi;44");
            Csv.AddRow(_PATHFILE, "000006;Luigi;Verdi;51");
            Csv.AddRow(_PATHFILE, "000007;Marco;Bianchi;35");
            Csv.AddRow(_PATHFILE, "000008;Luca;Verdi;19");
            int lineCount = File.ReadLines(_PATHFILE).Count();
            Assert.IsTrue(lineCount == 8);
        }

        [Test()]
        public void AddRowsTest()
        {
            List<string> records = new List<string>();
            records.Add("000001;Mario;Rossi;44");
            records.Add("000002;Luigi;Bianchi;51");
            records.Add("000003;Marco;Verdi;35");
            records.Add("000004;Luca;Rossi;19");

            Csv.AddRows(_PATHFILE2, records);
            int lineCount = File.ReadLines(_PATHFILE2).Count();
            Assert.IsTrue(lineCount == 4);
        }

        [Test()]
        public void ReadRowsTest()
        {
            List<string> rows = Csv.ReadRows(_PATHFILE, ';');
            Assert.NotNull(rows);
            Assert.IsTrue(rows.Count == 8);
        }

        [Test()]
        public void ReadColumnTest()
        {
            List<string> rows = Csv.ReadColumn(_PATHFILE, 0);
            List<string> expected = new List<string>() {
                "000001",
                "000002",
                "000003",
                "000004",
                "000005",
                "000006",
                "000007",
                "000008"
            };
            Assert.NotNull(rows);
            Assert.AreEqual(rows.Count, expected.Count);
            Assert.IsTrue(expected.SequenceEqual(rows));
        }

        [Test()]
        public void ReadColumnTest1()
        {
            List<string> rows = Csv.ReadColumn(_PATHFILE, 3);
            List<string> expected = new List<string>() {
                "44",
                "51",
                "35",
                "19",
                "44",
                "51",
                "35",
                "19"        
            };
            Assert.NotNull(rows);
            Assert.AreEqual(rows.Count, expected.Count);
            Assert.IsTrue(expected.SequenceEqual(rows));
        }

        [Test()]
        public void DeleteRowTest()
        {
            Csv.DeleteRow(_PATHFILE, 6);
            List<string> lst = Csv.ReadRows(_PATHFILE, ';');
            Assert.IsFalse(lst.Contains("000007"));
        }

        [Test()]
        public void DeleteRowTest1()
        {
            Csv.DeleteRow(_PATHFILE, "000001;Mario;Rossi;44");
            List<string> lst = Csv.ReadRows(_PATHFILE, ';');
            Assert.IsFalse(lst.Contains("000001"));
        }

        [Test()]
        public void DeleteRowTest2()
        {
            Csv.DeleteRow(_PATHFILE, "000006", 0);
            List<string> lst = Csv.ReadRows(_PATHFILE, ';');
            Assert.IsFalse(lst.Contains("000006"));
        }

        [Test()]
        public void UpdateRowTest()
        {
            Csv.UpdateRow(_PATHFILE, 3, "999933;Jonh;Doe;33");
            List<string> lst = Csv.ReadRows(_PATHFILE, ';');
            Assert.IsTrue(lst.Any(s => s.Contains("999933")));
        }

        [Test()]
        public void UpdateRowTest1()
        {
            Csv.UpdateRow(_PATHFILE, "000002;Luigi;Bianchi;51", "092222;Luigi;Bianchi;51");
            List<string> lst = Csv.ReadRows(_PATHFILE, ';');
            Assert.IsTrue(lst.Any(s => s.Contains("092222")));
        }

        [Test()]
        public void SearchRowTest()
        {
            List<string> results = Csv.SearchRow(_PATHFILE, "999933", 0, ';');
            List<string> expected = new List<string>() {
                "999933;Jonh;Doe;33"
            };
            Assert.NotNull(results);
            Assert.AreEqual(results.Count, expected.Count);
            Assert.IsTrue(expected.SequenceEqual(results));
        }

        [Test()]
        public void SearchRowTest1()
        {
            List<string> results = Csv.SearchRow(_PATHFILE, "092222", "Luigi", 0, 1, ';');
            List<string> expected = new List<string>() {
                "092222;Luigi;Bianchi;51"
            };
            Assert.NotNull(results);
            Assert.AreEqual(results.Count, expected.Count);
            Assert.IsTrue(expected.SequenceEqual(results));
        }

        [Test()]
        public void CSVRowToStringArrayTest()
        {
            string[] test = Csv.CSVRowToStringArray("999933;Jonh;Doe;33", ';');
            string[] expected = new string[]
            {
                "999933","Jonh","Doe","33"
            };
            Assert.NotNull(test);
            Assert.AreEqual(test.Length, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(test));
        } 
    }
}