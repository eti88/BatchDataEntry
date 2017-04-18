using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelNuovoModelloTests
    {
        private DatabaseHelperSqlServer dbs;
        private ViewModelNuovoModello vm;
        private ViewModelNuovoModello vm2;
        private ViewModelNuovoModello vm3;
        private ViewModelNuovoModello vm4;

        [Test(), Order(1)]
        public void ViewModelNuovoModelloTest()
        {
            vm = new ViewModelNuovoModello();
            Assert.IsNotNull(vm);
        }

        [Test(), Order(2)]
        public void ViewModelNuovoModelloTest1()
        {
            vm2 = new ViewModelNuovoModello(dbs);
            Assert.IsNotNull(vm);
        }

        [Test(), Order(3)]
        public void ViewModelNuovoModelloTest2()
        {
            Modello m = new Modello();
            m.Nome = Utility.GetRandomAlphanumericString(19);
            m.OrigineCsv = false;
            vm3 = new ViewModelNuovoModello(m, true);
            Assert.IsNotNull(vm);
        }

        [Test(), Order(4)]
        public void ViewModelNuovoModelloTest3()
        {
            Modello m = new Modello();
            m.Nome = Utility.GetRandomAlphanumericString(19);
            m.OrigineCsv = false;
            vm4 = new ViewModelNuovoModello(dbs,m, true);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.SelectedModel);
        }

        [Test(), Order(5)]
        public void AddNewItemTest()
        {
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.SelectedModel);
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            campi.Add(new Campo("Campo1",true,string.Empty,true));
            campi.Add(new Campo("Campo2", true, string.Empty, false));
            campi.Add(new Campo("Campo3", true, string.Empty, false));
            campi.Add(new Campo("Campo4", true, string.Empty, false));
            Modello m = new Modello("unitTest_01",true,campi);
            vm.SelectedModel = m;
            try
            {
                vm.AddNewItem();
            }
            catch (NullReferenceException)
            {
            }
            Assert.IsTrue(vm.SelectedModel.Id > 0);
        }

        [Test(), Order(6)]
        public void AddNewItemTestSQLite()
        {
            Assert.IsNotNull(vm2);
            Assert.IsNotNull(vm2.SelectedModel);
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            campi.Add(new Campo("Campo1", true, string.Empty, true));
            campi.Add(new Campo("Campo2", true, string.Empty, false));
            campi.Add(new Campo("Campo3", true, string.Empty, false));
            campi.Add(new Campo("Campo4", true, string.Empty, false));
            Modello m = new Modello("unitTest_02", true, campi);
            vm2.SelectedModel = m;
            try
            {
                vm2.AddNewItem();
            }
            catch (NullReferenceException)
            {
            }
            Assert.IsTrue(vm2.SelectedModel.Id > 0);
        }

        [Test(), Order(7)]
        public void UpdateItemTest()
        {
            Assert.IsNotNull(vm3);
            Assert.IsNotNull(vm3.SelectedModel);
            vm3.SelectedModel = vm2.SelectedModel;
            vm3.SelectedModel.PathFileCsv = @"C:\123\abc\aaa.csv";
            try
            {
                vm3.AddNewItem();
            }
            catch (NullReferenceException)
            {
            }
            Modello m = dbs.GetModelloById(vm3.SelectedModel.Id);
            Assert.IsTrue(vm3.SelectedModel.PathFileCsv.Equals(m.PathFileCsv));
        }

        [Test(), Order(8)]
        public void UpdateItemTestSQLite()
        {
            Assert.IsNotNull(vm4);
            Assert.IsNotNull(vm4.SelectedModel);
            vm4.SelectedModel = vm.SelectedModel;
            vm4.SelectedModel.PathFileCsv = @"C:\123\abc\aaa.csv";
            try
            {
                vm4.AddNewItem();
            }
            catch (NullReferenceException)
            {
            }
            Modello m = dbs.GetModelloById(vm4.SelectedModel.Id);
            Assert.IsTrue(vm4.SelectedModel.PathFileCsv.Equals(m.PathFileCsv));
        }
    }
}
