﻿using NUnit.Framework;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BatchDataEntry.Helpers.Tests
{
    [TestFixture()]
    public class DatabaseHelperSqlServerTests
    {
        private DatabaseHelperSqlServer db;

        [Test()]
        [SetUp]
        public void DatabaseHelperSqlServerTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            db = new DatabaseHelperSqlServer(user, user, server, dbname);
            Assert.IsNotNull(db);
        }

        [Test(), Order(1)]
        public void InsertTest()
        {
            Modello m = new Modello("testModel", false, new System.Collections.ObjectModel.ObservableCollection<Campo>());
            int insertId = db.Insert(m);
            Assert.IsTrue(insertId > 0);
        }

        [Test(), Order(2)]
        public void InsertTest1()
        {
            Campo c1 = new Campo("testCampo",false,"aaaa",true);
            Modello mTmp = db.ModelloQuery(@"");
            Assert.IsNotNull(mTmp);
            c1.IdModello = mTmp.Id;
            Campo c2 = new Campo("testCampo", false, "aaaa", true);
            c2.IdModello = mTmp.Id;
            int insertId = db.Insert(c1);
            Assert.IsTrue(insertId > 0);
        }

        [Test(), Order(3)]
        public void InsertTest2()
        {
            Batch b = new Batch("testUnitBatch",TipoFileProcessato.Pdf, @"C:\\input", @"C:\\output");
            b.IdModello = 1;
            int insertId = db.Insert(b);
            Assert.IsTrue(insertId > 0);
        }

        [Test(), Order(4)]
        public void UpdateTest()
        {
            Batch b = new Batch("testBatchA", TipoFileProcessato.Pdf, @"C:\\input", @"C:\\output");
            b.Id = 1;
            b.IdModello = 1;
            db.Update(b);
            Batch b2 = db.GetBatchById(1);
            Assert.IsTrue(b2.Nome.Equals("testBatchA"));
        }

        [Test(), Order(5)]
        public void DeleteFromTableTest()
        {
            db.DeleteFromTable("Campi", @"Id = '1'");
            Campo c = db.GetCampoById(1);
            Assert.IsTrue(c.Id == 0 && string.IsNullOrWhiteSpace(c.Nome));
        }

        [Test(), Order(6)]
        public void UpdateTest1()
        {
            Campo c1 = new Campo("testCampoA", false, "bbbb", true);
            c1.Id = 1;
            c1.IdModello = 1;
            db.Update(c1);
            Campo c2 = db.GetCampoById(1);
            Assert.IsNotNull(c2);
            Assert.IsTrue(c2.Nome.Equals("testCampoA") && c2.ValorePredefinito.Equals("bbbb"));
        }

        [Test(), Order(7)]
        public void UpdateTest2()
        {
            Modello m = new Modello("testModel", false, new ObservableCollection<Campo>());
            m.Nome = "testModelABC";
            m.Id = 1;
            db.Update(m);
            Modello m2 = db.GetModelloById(1);
            Assert.IsNotNull(m2);
            Assert.IsTrue(m2.Nome.Equals("testModelABC"));
        }

        [Test(), Order(8)]
        public void GetBatchByIdTest()
        {
            Batch batch = db.GetBatchById(1);
            Assert.IsNotNull(batch);
            Assert.IsTrue(batch.Id > 0);
        }

        [Test(), Order(9)]
        public void GetCampoByIdTest()
        {
            Campo c = db.GetCampoById(1);
            Assert.IsNotNull(c);
            Assert.IsTrue(c.Id == 1); ;
        }

        [Test(), Order(10)]
        public void GetModelloByIdTest()
        {
            Modello m = db.GetModelloById(1);
            Assert.IsNotNull(m);
            Assert.IsTrue(m.Id == 1);
        }

        [Test(), Order(11)]
        public void GetBatchRecordsTest()
        {
            ObservableCollection<Batch> batches = db.GetBatchRecords();
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count > 0);
        }

        [Test(), Order(12)]
        public void GetCampoRecordsTest()
        {
            ObservableCollection<Campo> campi = db.GetCampoRecords();
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count > 0);
        }

        [Test(), Order(13)]
        public void GetModelloRecordsTest()
        {
            ObservableCollection<Modello> modello = db.GetModelloRecords();
            Assert.IsNotNull(modello);
            Assert.IsTrue(modello.Count > 0);
        }

        [Test(), Order(14)]
        public void BatchQueryTest()
        {
            ObservableCollection<Batch> batches = db.BatchQuery("SELECT * FROM Batch WHERE Id = 1");
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count == 1);
        }

        [Test(), Order(15)]
        public void CampoQueryTest()
        {
            ObservableCollection<Campo> campi = db.CampoQuery("SELECT * FROM Campi WHERE Id = 1");
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count == 1);
        }

        [Test(), Order(16)]
        public void ModelloQueryTest()
        {
            ObservableCollection<Modello> modelli = db.ModelloQuery("SELECT * FROM Modelli WHERE Id = 1");
            Assert.IsNotNull(modelli);
            Assert.IsTrue(modelli.Count == 1);
        }

        [Test(), Order(17)]
        public void IEnumerableModelliTest()
        {
            IEnumerable<Modello> modelli = db.IEnumerableModelli();
            Assert.IsNotNull(modelli);
        }

        [Test(), Order(18)]
        public void CountTest()
        {
            int n = db.Count("SELECT COUNT(*) FROM Batch");
            Assert.IsTrue(n > 0);
        }

        [TearDown]
        public void DropValue()
        {
            db.DropAllRowsFromTable("Modelli");
            db.DropAllRowsFromTable("Campi");
            db.DropAllRowsFromTable("Batch");
        }
    }
}