using NUnit.Framework;
using BatchDataEntry.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BatchDataEntry.Models;
using BatchDataEntry.Components;
using System.Collections.ObjectModel;

namespace BatchDataEntry.Helpers.Tests
{
    [TestFixture()]
    public class DatabaseHelperTests
    {
        private DatabaseHelper testDb;
        private DatabaseHelper cacheTestDb;

        [SetUp]
        public void Init()
        {
            testDb = new DatabaseHelper(Path.Combine(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", @"test.db3"));
            cacheTestDb = new DatabaseHelper(Path.Combine(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", @"cacheTest.db3"));
            testDb.InitTabs();
            cacheTestDb.CreateCacheDb(new List<string>());
            testDb.InsertRecordBatch(new Batch("utest", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output"));
            Modello m = new Modello();
            m.Nome = "utestModel";
            m.OrigineCsv = false;
            testDb.InsertRecordModello(m);
            testDb.InsertRecordCampo(new Campo("campo1", false, "ciao", true));
            cacheTestDb.InsertRecordAutocompletamento(new Autocompletamento(0, "valore1"));
        }

        [Test(), Order(1)]
        public void InitTabsTest()
        {
            int countTable = Convert.ToInt32(testDb.ExecuteScalar(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'"));
            Assert.IsNotNull(testDb);
            Assert.IsTrue(countTable == 4); // 4 perché c'è anche la tabella di sistema
        }

        [Test(), Order(2)]
        public void CreateCacheDbTest()
        {
            int countTable = Convert.ToInt32(cacheTestDb.ExecuteScalar(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'"));
            Assert.IsNotNull(cacheTestDb);
            Assert.IsTrue(countTable == 3); // 3 perché c'è anche la tabella di sistema
        }

        [Test(), Order(3)]
        public void GetDataTableTest()
        {
            DataTable dt = testDb.GetDataTable("Batch");
            Assert.IsNotNull(dt);
        }

        [Test(), Order(4)]
        public void GetDataTableDocumentiTest()
        {
            DataTable dt = cacheTestDb.GetDataTableDocumenti();
            Assert.IsNotNull(dt);
        }

        [Test(), Order(5)]
        public void convertQuotesTest()
        {
            string a = "piu'";
            Assert.IsTrue(DatabaseHelper.convertQuotes(a) == "piu''");
        }
        
        [Test(), Order(6)]
        public void InsertRecordDocumentoTest()
        {
            Document dc = new Document(1, "ciao", @"C:\\Input\ciao.pdf", false);
            Assert.IsTrue(cacheTestDb.InsertRecordDocumento(new Batch("test", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output"),dc) > 0); 
        }

        [Test(), Order(7)]
        public void UpdateRecordBatchTest()
        {
            Batch b = new Batch("utest", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output");
            b.IdModello = 1;
            testDb.UpdateRecordBatch(b);
            Batch expected = testDb.GetBatchById(1);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.IdModello > 0);
        }

        [Test(), Order(8)]
        public void UpdateRecordCampoTest()
        {
            Campo c = new Campo("campo1", false, "ciao", true);
            c.IdModello = 1;
            c.IsDisabled = true;
            testDb.UpdateRecordCampo(c);
            Campo expected = testDb.GetCampoById(1);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.IdModello > 0 && expected.IsDisabled == true);
        }

        [Test(), Order(9)]
        public void UpdateRecordModelloTest()
        {
            Modello m = new Modello();
            m.Nome = "utestModel";
            m.OrigineCsv = true;
            m.Separatore = ";";
            m.PathFileCsv = "D:\\test\\aaa.csv";
            testDb.UpdateRecordModello(m);
            Modello expected = testDb.GetModelloById(1);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.OrigineCsv == true && expected.Separatore.Equals(";") && expected.PathFileCsv.Equals("D:\\test\\aaa.csv"));
        }

        [Test(), Order(10)]
        public void UpdateRecordDocumentoTest()
        {
            Document dc = new Document(1, "ciao", @"C:\\Input\ciao.pdf", false);
            dc.IsIndexed = true;
            cacheTestDb.UpdateRecordDocumento(dc);
            var expected = cacheTestDb.GetDocumento(1);
            Assert.IsNotNull(dc);
            Assert.IsTrue(expected.ContainsKey(3) && expected.ContainsValue("1"));
        }

        [Test(), Order(11)]
        public void GetDataTableWithQueryTest()
        {
            DataTable dt = testDb.GetDataTableWithQuery("SELECT * FROM Batch WHERE Id = 1");
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Columns.Count > 0);
        }
        
        [Test(), Order(12)]
        public void GetDocumentoTest1()
        {
            var expected = cacheTestDb.GetDocumento("ciao");
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
        }

        [Test(), Order(13)]
        public void GetDocumentsTest()
        {
            NavigationList<Dictionary<int, string>> nav = cacheTestDb.GetDocuments();
            Assert.IsNotNull(nav);
            Assert.IsTrue(nav.Count == 1);
        }

        [Test(), Order(14)]
        public void GetAutocompleteListTest()
        {
            List<string> suggestions = cacheTestDb.GetAutocompleteList(0);
            Assert.IsNotNull(suggestions);
            Assert.IsTrue(suggestions.Count > 0);
        }

        [Test(), Order(15)]
        public void GetAutocompleteListObTest()
        {
            ObservableCollection<Suggestion> suggestions = cacheTestDb.GetAutocompleteListOb(0);
            Assert.IsNotNull(suggestions);
            Assert.IsTrue(suggestions.Count > 0);
        }

        [Test(), Order(16)]
        public void GetBatchRecordsTest()
        {
            ObservableCollection<Batch> batches = testDb.GetBatchRecords();
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count > 0 && batches[0].Nome.Equals("utest"));
        }

        [Test(), Order(17)]
        public void GetDocumentsListPartialTest()
        {
            List<Document> docs = cacheTestDb.GetDocumentsListPartial();
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 1);
        }

        [Test(), Order(18)]
        public void GetDocumentsListPartialTest1()
        {
            List<Document> docs = cacheTestDb.GetDocumentsListPartial("SELECT * FROM Documenti WHERE Id = 1");
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 1);
        }

        [Test(), Order(19)]
        public void GetCampoRecordsTest()
        {
            ObservableCollection<Campo> campi = testDb.GetCampoRecords();
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count > 0);
        }

        [Test(), Order(20)]
        public void GetModelloRecordsTest()
        {
            ObservableCollection<Modello> modello = testDb.GetModelloRecords();
            Assert.IsNotNull(modello);
            Assert.IsTrue(modello.Count > 0);
        }

        [Test(), Order(21)]
        public void BatchQueryTest()
        {
            ObservableCollection<Batch> batches = testDb.BatchQuery("SELECT * FROM Batch WHERE Id = 1");
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count == 1);
        }

        [Test(), Order(22)]
        public void CampoQueryTest()
        {
            ObservableCollection<Campo> campi = testDb.CampoQuery("SELECT * FROM Campo WHERE Id = 1");
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count == 1);
        }

        [Test(), Order(23)]
        public void ModelloQueryTest()
        {
            ObservableCollection<Modello> modelli = testDb.ModelloQuery("SELECT * FROM Modello WHERE Id = 1");
            Assert.IsNotNull(modelli);
            Assert.IsTrue(modelli.Count == 1);
        }

        [Test(), Order(24)]
        public void IEnumerableModelliTest()
        {
            IEnumerable<Modello> modelli = testDb.IEnumerableModelli();
            Assert.IsNotNull(modelli);
        }

        [TearDown]
        public void Clean()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"test.db3"));
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"cacheTest.db3"));
        }
    }
}