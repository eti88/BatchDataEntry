using NUnit.Framework;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using BatchDataEntry.Business;
using System;

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
            Campo c1 = new Campo(1, "campo1abc", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            Modello mTmp = db.GetFirstModello();
            Assert.IsNotNull(mTmp);
            c1.IdModello = mTmp.Id;
            Campo c2 = new Campo(1, "campo2abc", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            c2.IdModello = mTmp.Id;
            int insertId = db.Insert(c1);
            Assert.IsTrue(insertId > 0);
        }

        [Test(), Order(3)]
        public void InsertTest2()
        {
            Batch b = new Batch("testUnitBatch",TipoFileProcessato.Pdf, @"C:\\input", @"C:\\output");
            Modello mTmp = db.GetFirstModello();
            Assert.IsNotNull(mTmp);
            b.IdModello = mTmp.Id;
            int insertId = db.Insert(b);
            Assert.IsTrue(insertId > 0);
        }

        [Test(), Order(4)]
        public void UpdateTest()
        {
            Batch b = db.GetFirstBatch();
            Assert.IsNotNull(b);
            Modello m = db.GetFirstModello();
            Assert.IsNotNull(m);
            b.IdModello = m.Id;
            b.Nome = Utility.GetRandomAlphanumericString(15);
            db.Update(b);
            Batch b2 = db.GetBatchById(b.Id);
            Assert.IsTrue(b2.Nome.Equals(b.Nome));
        }

        [Test(), Order(5)]
        public void DeleteFromTableTest()
        {
            Campo c = db.GetFirstCampo();
            Assert.IsNotNull(c);
            db.DeleteFromTable("Campi", string.Format( @"Id = '{0}'", c.Id));
            Campo c2 = db.GetFirstCampo();
            Assert.IsTrue(c.Id != c2.Id);
        }

        [Test(), Order(6)]
        public void UpdateTest1()
        {
            Campo c = db.GetFirstCampo();
            Assert.IsNotNull(c);
            c.Posizione = 99;
            db.Update(c);
            Campo c2 = db.GetCampoById(c.Id);
            Assert.IsNotNull(c2);
            Assert.IsTrue(c2.Posizione == c.Posizione);
        }

        [Test(), Order(7)]
        public void UpdateTest2()
        {
            Modello m = db.GetFirstModello();
            Assert.IsNotNull(m);
            m.Nome = Utility.GetRandomAlphanumericString(10);
            db.Update(m);
            Modello m2 = db.GetModelloById(m.Id);
            Assert.IsNotNull(m2);
            Assert.IsTrue(m2.Nome.Equals(m.Nome));
        }

        [Test(), Order(8)]
        public void GetBatchByIdTest()
        {
            Batch tmp = db.GetFirstBatch();
            Assert.IsNotNull(tmp);
            Batch batch = db.GetBatchById(tmp.Id);
            Assert.IsNotNull(batch);
            Assert.IsTrue(tmp.Id == batch.Id);
        }

        [Test(), Order(9)]
        public void GetCampoByIdTest()
        {
            Campo tmp = db.GetFirstCampo();
            Assert.IsNotNull(tmp);
            Campo c = db.GetCampoById(tmp.Id);
            Assert.IsNotNull(c);
            Assert.IsTrue(c.Id == tmp.Id); ;
        }

        [Test(), Order(10)]
        public void GetModelloByIdTest()
        {
            Modello tmp = db.GetFirstModello();
            Assert.IsNotNull(tmp);
            Modello m = db.GetModelloById(tmp.Id);
            Assert.IsNotNull(m);
            Assert.IsTrue(m.Id == tmp.Id);
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
            ObservableCollection<Batch> batches = db.BatchQuery("SELECT * FROM Batch WHERE Id > 1");
            Assert.IsNotNull(batches);
            Assert.IsTrue(batches.Count > 0);
        }

        [Test(), Order(15)]
        public void CampoQueryTest()
        {
            ObservableCollection<Campo> campi = db.CampoQuery("SELECT * FROM Campi WHERE Id > 1");
            Assert.IsNotNull(campi);
            Assert.IsTrue(campi.Count > 0);
        }

        [Test(), Order(16)]
        public void ModelloQueryTest()
        {
            ObservableCollection<Modello> modelli = db.ModelloQuery("SELECT * FROM Modelli WHERE Id > 1");
            Assert.IsNotNull(modelli);
            Assert.IsTrue(modelli.Count > 0);
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

        [Test(), Order(19)]
        public void GetTableListTest()
        {
            var r = db.GetTableList();
            Assert.IsNotNull(r);
            Assert.IsTrue(r.Count >= 0);
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