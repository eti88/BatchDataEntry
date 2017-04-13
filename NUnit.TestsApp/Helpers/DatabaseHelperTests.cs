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
            testDb = new DatabaseHelper(Path.Combine(Directory.GetCurrentDirectory(), @"test.db3"));
            cacheTestDb = new DatabaseHelper(Path.Combine(Directory.GetCurrentDirectory(), @"cacheTest.db3"));
        }

        [Test()]
        public void InitTabsTest()
        {
            testDb.InitTabs();
            int countTable = Convert.ToInt32(testDb.ExecuteScalar(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'"));
            Assert.IsNotNull(testDb);
            Assert.IsTrue(countTable == 4); // 4 perché c'è anche la tabella di sistema
        }

        [Test()]
        public void CreateCacheDbTest()
        {
            cacheTestDb.CreateCacheDb(new List<string>());
            int countTable = Convert.ToInt32(testDb.ExecuteScalar(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'"));
            Assert.IsNotNull(cacheTestDb);
            Assert.IsTrue(countTable == 3); // 3 perché c'è anche la tabella di sistema
        }

        [Test()]
        public void GetDataTableTest()
        {
            DataTable dt = testDb.GetDataTable("Batch");
            Assert.IsNotNull(dt);
        }

        [Test()]
        public void GetDataTableDocumentiTest()
        {
            DataTable dt = cacheTestDb.GetDataTableDocumenti();
            Assert.IsNotNull(dt);
        }

        [Test()]
        public void convertQuotesTest()
        {
            string a = "piu'";
            Assert.IsTrue(DatabaseHelper.convertQuotes(a) == "piu''");
        }

        [Test()]
        public void InsertRecordBatchTest()
        {
            Batch b = new Batch("utest", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output");
            Assert.IsTrue(testDb.InsertRecordBatch(b) > 0);
        }

        [Test()]
        public void InsertRecordModelloTest()
        {
            Modello m = new Modello();
            m.Nome = "utestModel";
            m.OrigineCsv = false;
            Assert.IsTrue(testDb.InsertRecordModello(m) > 0);
        }

        [Test()]
        public void InsertRecordCampoTest()
        {
            Campo c = new Campo("campo1", false, "ciao", true);
            Assert.IsTrue(testDb.InsertRecordCampo(c) > 0);
        }

        [Test()]
        public void InsertRecordAutocompletamentoTest()
        {
            Autocompletamento au = new Autocompletamento(0, "valore1");
            Assert.IsTrue(cacheTestDb.InsertRecordAutocompletamento(au) > 0);
        }
        
        [Test()]
        public void InsertRecordDocumentoTest()
        {
            Document dc = new Document(1, "ciao", @"C:\\Input\ciao.pdf", false);
            Assert.IsTrue(cacheTestDb.InsertRecordDocumento(new Batch("test", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output"),dc) > 0); 
        }

        [Test()]
        public void UpdateRecordBatchTest()
        {
            Batch b = new Batch("utest", TipoFileProcessato.Pdf, @"C:\\Input", @"C:\\Output");
            b.IdModello = 1;
            testDb.UpdateRecordBatch(b);
            Batch expected = testDb.GetBatchById(1);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.IdModello == 1);
        }

        [Test()]
        public void UpdateRecordCampoTest()
        {
            Campo c = new Campo("campo1", false, "ciao", true);
            c.IdModello = 1;
            c.IsDisabled = true;
            testDb.UpdateRecordCampo(c);
            Campo expected = testDb.GetCampoById(1);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.IdModello == 1 && expected.IsDisabled == true);
        }

        [Test()]
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
            Assert.IsTrue(expected.OrigineCsv == true && expected.Separatore.Equals(";") && expected.PathFileCsv.Equals("D:\\test\\aaa.csv"))
        }

        [Test()]
        public void UpdateRecordDocumentoTest()
        {
            Document dc = new Document(1, "ciao", @"C:\\Input\ciao.pdf", false);
            dc.IsIndexed = true;
            cacheTestDb.UpdateRecordDocumento(dc);
            var expected = cacheTestDb.GetDocumento(1);
            Assert.IsNotNull(dc);
            Assert.IsTrue(expected.ContainsKey(3) && expected.ContainsValue("1"));
        }

        [Test()]
        public void GetDataTableWithQueryTest()
        {
            DataTable dt = testDb.GetDataTableWithQuery("SELECT * FROM Batch WHERE Id = 1");
            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Columns.Count == 10);
        }
        
        [Test()]
        public void GetDocumentoTest1()
        {
            var expected = cacheTestDb.GetDocumento("ciao");
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
        }

        [Test()]
        public void GetDocumentsTest()
        {
            NavigationList<Dictionary<int, string>> nav = cacheTestDb.GetDocuments();
            Assert.IsNotNull(nav);
            Assert.IsTrue(nav.Count == 1);
        }

        [Test()]
        public void GetAutocompleteListTest()
        {
            List<string> suggestions = cacheTestDb.GetAutocompleteList(0);
            Assert.IsNotNull(suggestions);
            Assert.IsTrue(suggestions.Count == 1);
        }

        [Test()]
        public void GetAutocompleteListObTest()
        {
            ObservableCollection<Suggestion> suggestions = cacheTestDb.GetAutocompleteListOb(0);
            Assert.IsNotNull(suggestions);
            Assert.IsTrue(suggestions.Count == 1);
        }

        [Test()]
        public void GetBatchRecordsTest()
        {
            ObservableCollection<Batch> batches = testDb.GetBatchRecords();
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count == 1 && batches[0].Nome.Equals("utest"));
        }

        [Test()]
        public void GetDocumentsListPartialTest()
        {
            List<Document> docs = cacheTestDb.GetDocumentsListPartial();
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 1);
        }

        [Test()]
        public void GetDocumentsListPartialTest1()
        {
            List<Document> docs = cacheTestDb.GetDocumentsListPartial("SELECT * FROM Documenti WHERE Id = 1");
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 1);
        }

        [Test()]
        public void GetCampoRecordsTest()
        {
            ObservableCollection<Campo> campi = testDb.GetCampoRecords();
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count == 1);
        }

        [Test()]
        public void GetModelloRecordsTest()
        {
            ObservableCollection<Modello> modello = testDb.GetModelloRecords();
            Assert.IsNotNull(modello);
            Assert.IsTrue(modello.Count == 1);
        }

        [Test()]
        public void BatchQueryTest()
        {
            ObservableCollection<Batch> batches = testDb.BatchQuery("SELECT * FROM Batch WHERE Id = 1");
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count == 1);
        }

        [Test()]
        public void CampoQueryTest()
        {
            ObservableCollection<Campo> campi = testDb.CampoQuery("SELECT * FROM Campo WHERE Id = 1");
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count == 1);
        }

        [Test()]
        public void ModelloQueryTest()
        {
            ObservableCollection<Modello> modelli = testDb.ModelloQuery("SELECT * FROM Modello WHERE Id = 1");
            Assert.IsNotNull(modelli);
            Assert.IsTrue(modelli.Count == 1);
        }

        [Test()]
        public void IEnumerableModelliTest()
        {
            IEnumerable<Modello> modelli = testDb.IEnumerableModelli();
            Assert.IsNotNull(modelli);
        }

        [Test()]
        public void ClearTableTest()
        {
            Assert.IsTrue(testDb.ClearTable("Batch"));
        }

        [Test()]
        public void ClearDBTest()
        {
            Assert.Fail();
        }

        [TearDown]
        public void Clean()
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"test.db3"));
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"cacheTest.db3"));
        }
    }
}