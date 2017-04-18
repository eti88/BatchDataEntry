using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelNewBatchTests
    {
        protected ViewModelNewBatch vm;
        protected ViewModelNewBatch vm2;
        protected DatabaseHelperSqlServer dbsql;
        private static string UnitTestPath = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles";
        private static string UnitTestPath2 = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\sub";
        private static string _DBNAME_ = @"unitTest.db3";

        [SetUp]
        public void InitDb()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            dbsql = new DatabaseHelperSqlServer(user, user, server, dbname);
        }

        [Test(), Order(1)]
        public void ViewModelNewBatchTest()
        {
            ViewModelNewBatch v = new ViewModelNewBatch();
            Assert.IsNotNull(v);
            Assert.IsNotNull(v.CurrentBatch);
        }

        [Test(), Order(2)]
        public void ViewModelNewBatchTest1()
        {
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            campi.Add(new Campo("colonnaA",false,string.Empty,true));
            campi.Add(new Campo("colonnaB", true, string.Empty, false));
            campi.Add(new Campo("colonnac", false, string.Empty, false));
            campi.Add(new Campo("colonnaD", true, string.Empty, false));
            Modello m = new Modello("unitModel", true, campi);
            m.OrigineCsv = true;
            m.PathFileCsv = Path.Combine(UnitTestPath, @"\origin\origin.csv");
            m.Separatore = ";";
            Batch b = new Batch("unitTestBatch",TipoFileProcessato.Pdf, Path.Combine(UnitTestPath, @"in"), Path.Combine(UnitTestPath, @"out"));
            b.Applicazione = m;
            for (int i = 0; i < 1000; i++)
                File.Copy(Path.Combine(UnitTestPath, @"in", @"origin.pdf"), Path.Combine(UnitTestPath, @"in", string.Format("{0}.pdf",i.ToString("D8"))));
            ViewModelNewBatch v = new ViewModelNewBatch(b);
            Assert.IsNotNull(v);
            Assert.IsNotNull(v.CurrentBatch);
        }

        [Test(), Order(3)]
        public void ViewModelNewBatchTest2()
        {
            vm = new ViewModelNewBatch(dbsql);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.CurrentBatch);
        }

        [Test(), Order(4)]
        public void ViewModelNewBatchTest3()
        {
            Batch b = new Batch("unitTestBatch", TipoFileProcessato.Pdf, Path.Combine(UnitTestPath, @"in"), Path.Combine(UnitTestPath, @"out"));
            vm2 = new ViewModelNewBatch(b, dbsql);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.CurrentBatch);
        }

        [Test(), Order(5)]
        public void AddOrUpdateBatchItemTestAdd()
        {
            Assert.IsNotNull(vm);
            vm.CurrentBatch.Applicazione = dbsql.GetFirstModello();
            Assert.IsNotNull(vm.CurrentBatch.Applicazione);
            vm.AddOrUpdateBatchItem();
        }

        [Test(), Order(6)]
        public void AddOrUpdateBatchItemTestUpdate()
        {
            Assert.IsNotNull(vm2);
            vm2.CurrentBatch.Applicazione = dbsql.GetFirstModello();
            Assert.IsNotNull(vm2.CurrentBatch.Applicazione);
            vm2.AddOrUpdateBatchItem();
        }

        [Test(), Order(7)]
        public void PopulateComboboxModelsTest()
        {
            Assert.IsNotNull(vm);
            Assert.IsNotNull(dbsql);
            vm.PopulateComboboxModels(dbsql);
            Assert.IsNotNull(vm.Models);
        }

        [Test(), Order(8)]
        public void fillCacheDbTestAutomatic()
        {
            DatabaseHelper dbtest = new DatabaseHelper(_DBNAME_, UnitTestPath);
            vm2.IndexType = "Automatica";
            vm2.fillCacheDb(dbtest, dbsql, vm2.CurrentBatch);
            int records = dbtest.Count("SELECT Count(Id) FROM Documenti");
            Assert.IsTrue(records > 0);
            Assert.IsTrue(records == 1000);
        }

        [Test(), Order(9)]
        public void fillCacheDbTestManual()
        {
            DatabaseHelper dbtest = new DatabaseHelper(_DBNAME_, UnitTestPath2);
            vm2.IndexType = "Manuale";
            vm2.fillCacheDb(dbtest, dbsql, vm2.CurrentBatch);
            int records = dbtest.Count("SELECT Count(Id) FROM Documenti");
            Assert.IsTrue(records > 0);
            Assert.IsTrue(records == 1000);
        }

        [Test(), Order(10)]
        public void IncrementalfillCacheDbTestManual()
        {
            DatabaseHelper dbtest = new DatabaseHelper(_DBNAME_, UnitTestPath2);
            vm2.CurrentBatch.Applicazione.PathFileCsv = Path.Combine(UnitTestPath, @"\origin\inc.csv");
            vm2.CurrentBatch.DirectoryInput = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\origin\incrementalPdf";
            vm2.IndexType = "Manuale";
            int records = dbtest.Count("SELECT Count(Id) FROM Documenti");
            Assert.IsTrue(records > 0);
            Assert.IsTrue(records == 1500);
        }

        [Test(), Order(11)]
        public void CreateDbCsvTest()
        {
            vm2.CreateDbCsv(UnitTestPath2);
            Assert.IsTrue(File.Exists(Path.Combine(UnitTestPath2, @"db.csv")));
        }
    }
}
