using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NUnit.TestsApp.ViewModels
{
    [TestFixture()]
    public class ViewModelApplicazioneTests
    {
        protected ViewModelApplicazione vm;
        protected ViewModelApplicazione vm2; // sqlite db
        protected AbsDbHelper db;

        [Test()]
        [SetUp]
        public void ViewModelApplicazioneTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            db = new DatabaseHelperSqlServer(user, user, server, dbname);
            vm = new ViewModelApplicazione(db);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(db);
        }

        [Test(), Order(1)]
        public void ViewModelApplicazioneTest1()
        {
            vm2 = new ViewModelApplicazione(new DatabaseHelper());
            Assert.IsNotNull(vm);
        }

        [Test(), Order(2)]
        public void LoadModelsTest()
        {
            vm.Modelli = new ObservableCollection<Modello>();
            vm.LoadModels(db);
            Assert.IsTrue(vm.Modelli.Count > 0);
        }

        [Test(), Order(2)]
        public void LoadModelsTestSQLite()
        {
            vm2.Modelli = new ObservableCollection<Modello>();
            vm2.LoadModels(db);
            Assert.IsTrue(vm.Modelli.Count > 0);
        }

        [Test(), Order(3)]
        public void CopyModelTest()
        {
            if (vm.Modelli == null || vm.Modelli.Count == 0) vm.LoadModels(db);
            vm.SelectedModel = vm.Modelli[0];
            vm.CopyModel();
            ObservableCollection<Modello> fresh = db.GetModelloRecords();
            Assert.IsTrue(vm.Modelli.Count == fresh.Count);
            Assert.IsTrue(fresh[fresh.Count - 1].Nome.Contains("Copia"));
        }

        [Test(), Order(5)]
        public void RemoveModelItemTest()
        {
            if (vm.Modelli == null || vm.Modelli.Count == 0) vm.LoadModels(db);
            Modello tmp = new Modello(vm.Modelli[0]);
            vm.SelectedModel = vm.Modelli[0];
            vm.RemoveModelItem();
            List<Modello> tmpl = new List<Modello>(db.GetModelloRecords());
            Assert.IsTrue(!tmpl.Exists(x => x.Id == tmp.Id));
        }
    }
}
