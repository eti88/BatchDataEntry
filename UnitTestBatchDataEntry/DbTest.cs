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
        private DatabaseHelper testDb = new DatabaseHelper();

        [TestMethod]
        public void TestDbInit()
        {
            try
            {
                if (testDb != null)
                    testDb.InitTabs();
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
            Campo campo1 = new Campo(1, "colonna test", 0, false, "", true, 1);
            Campo campo2 = new Campo(2, "colonna test 2", 1, false, "", false, 1);
            Modello modello1 = new Modello(1, "Modello test", false, new ObservableCollection<Campo>());
            Modello modello2 = new Modello(2, "Modello test 2", false, new ObservableCollection<Campo>());
            Batch batch = new Batch(1, "batch test", TipoFileProcessato.Pdf, "C:/input/", "C:/output/", modello1, 1, 1, 0, 0, 0, 1);

            try
            {
                testDb.InsertRecord(campo1);
                testDb.InsertRecord(campo2);
                testDb.InsertRecord(modello1);
                testDb.InsertRecord(modello2);
                testDb.InsertRecord(batch);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestUpdateRecords()
        {
            Campo campo = new Campo(2, "colonna blablabla", 1, false, "", false, 1);
            Modello modello = new Modello(2, "Modello ffffff", false, new ObservableCollection<Campo>());
            try
            {
                testDb.UpdateRecord(campo);
                testDb.UpdateRecord(modello);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestDeleteRecords()
        {
            try
            {
                testDb.DeleteRecord(new Campo(), 2);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestGetBatchRecord()
        {
            try
            {
                Batch b = testDb.GetBatchById(1);
                if (b == null)
                    Assert.IsTrue(false);              
                else
                    Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestGetCampoRecord()
        {
            try
            {
                Campo c = testDb.GetCampoById(1);
                if (c == null)
                    Assert.IsTrue(false);
                else if (c.Id != 1)
                    Assert.IsTrue(false);
                else if (string.IsNullOrEmpty(c.Nome))
                    Assert.IsTrue(false);
                else if (c.Posizione < 0)
                    Assert.IsTrue(false);
                else
                    Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestGetModelloRecord()
        {
            try
            {
                Modello m = testDb.GetModelloById(1);
                if (m == null)
                    Assert.IsTrue(false);
                else if (m.Id != 1)
                    Assert.IsTrue(false);
                else if (string.IsNullOrEmpty(m.Nome))
                    Assert.IsTrue(false);
                else
                    Assert.IsTrue(true);
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
                if(collection.Count > 0)
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
                string query = "SELECT * FROM Campo WHERE IndicePrimario = 1";
                ObservableCollection<Campo> collection = testDb.CampoQuery(query);
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
        public void TestIEnumerableModels()
        {
            try
            {
                IEnumerable<BatchDataEntry.DBModels.Modello> res = testDb.IEnumerableModelli();

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
