using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class DbTest
    {
        private DatabaseHelper testDb = new DatabaseHelper(Path.Combine(Directory.GetCurrentDirectory(), @"test.db3"));
        private DatabaseHelper cacheTestDb = new DatabaseHelper(Path.Combine(Directory.GetCurrentDirectory(), @"cacheTest.db3"));

        [TestMethod]
        public void TestDbInit()
        {
            try
            {
                if (testDb != null)
                    testDb.InitTabs();
                if (cacheTestDb != null)
                    cacheTestDb.CreateCacheDb(new List<string>() {"Column1", "Column2"});
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestInsertRecords()
        {
            Campo campo1 = new Campo(1, "Column1", 0, false, "", true, 1);
            Campo campo2 = new Campo(2, "Column2", 1, false, "", false, 1);
            Modello modello1 = new Modello(1, "Modello test", false, new ObservableCollection<Campo>());
            Modello modello2 = new Modello(2, "Modello test 2", false, new ObservableCollection<Campo>());
            Batch batch = new Batch(1, "batch test", TipoFileProcessato.Pdf, "C:/input/", "C:/output/", modello1, 1, 1, 0, 0, 0, 1);
            try
            {
                testDb.InsertRecordCampo(campo1);
                testDb.InsertRecordCampo(campo2);
                testDb.InsertRecordModello(modello1);
                testDb.InsertRecordModello(modello2);
                testDb.InsertRecordBatch(batch);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }          
        }

        [TestMethod]
        public void TestReadBatchRecords()
        {
            try
            {
                Batch b = testDb.GetBatchById(1);
                Assert.AreEqual(1, b.Id);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void TestReadCampoRecords()
        {
            try
            {
                Campo c = testDb.GetCampoById(2);
                Assert.AreEqual("Column2", c.Nome);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void TestReadModelRecords()
        {
            try
            {
                Modello c = testDb.GetModelloById(2);
                Assert.AreEqual("Modello test 2", c.Nome);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void TestUpdateBatchRecords()
        {
            try
            {
                Batch b = testDb.GetBatchById(1);
                Batch b2 = new Batch(b);
                b2.Nome = "Updated name";
                testDb.UpdateRecordBatch(b2);
                b2 = testDb.GetBatchById(1);
                Assert.AreNotEqual(b.Nome, b2.Nome);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestUpdateCampoRecords()
        {
            try
            {
                Campo c = testDb.GetCampoById(1);
                Campo c2 = new Campo(c);
                c2.Nome = "UnitTestColumn";
                testDb.UpdateRecordCampo(c2);
                c2 = testDb.GetCampoById(1);
                Assert.AreNotEqual(c.Nome, c2.Nome);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestUpdateModelloRecords()
        {
            try
            {
                Modello m = testDb.GetModelloById(1);
                Modello m2 = new Modello(m);
                m2.Nome = "Updated Model name";
                testDb.UpdateRecordModello(m2);
                m2 = testDb.GetModelloById(1);
                Assert.AreNotEqual(m.Nome, m2.Nome);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestBatchesList()
        {
            try
            {
                ObservableCollection<Batch> collection = testDb.GetBatchRecords();
                if (collection.Count > 0)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestFieldsList()
        {
            try
            {
                ObservableCollection<Campo> collection = testDb.GetCampoRecords();
                if (collection.Count > 0)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestModelsList()
        {
            try
            {
                ObservableCollection<Modello> collection = testDb.GetModelloRecords();
                if (collection.Count > 0)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestQueryBatch()
        {
            try
            {
                string query = "SELECT * FROM Batch WHERE TipoFile = 1";
                ObservableCollection<Batch> collection = testDb.BatchQuery(query);
                if (collection.Count >= 1)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestQueryModello()
        {
            try
            {
                string query = "SELECT * FROM Modello WHERE Id = 1";
                ObservableCollection<Modello> collection = testDb.ModelloQuery(query);
                if (collection.Count >= 1)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestQueryCampo()
        {
            try
            {
                string query = @"SELECT * FROM Campo WHERE Id=1";
                ObservableCollection<Campo> collection = testDb.CampoQuery(query);

                if (collection.Count == 1)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestIEnumerableModels()
        {
            try
            {
                IEnumerable<Modello> res = testDb.IEnumerableModelli();

                if (res != null)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}
