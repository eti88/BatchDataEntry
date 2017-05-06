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
        Abstracts.AbsDbHelper dbs;
        ViewModelNuovoModello vm;
        ViewModelNuovoModello vm2;
        ViewModelNuovoModello vm3;
        ViewModelNuovoModello vm4;

        [SetUp]
        public void ViewModelNuovoModelloTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            dbs = new DatabaseHelperSqlServer(user, user, server, dbname);
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
            vm3 = new ViewModelNuovoModello(dbs ,m, true);
            Assert.IsNotNull(vm);
        }
        
        [Test(), Order(5)]
        public void AddNewItemTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            var db = new DatabaseHelperSqlServer(user, user, server, dbname);
            var vm = new ViewModelNuovoModello(db);
            Assert.IsNotNull(vm);
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            campi.Add(new Campo(1, "campo1a", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false));
            campi.Add(new Campo(1, "campo2a", 0, string.Empty, string.Empty, false, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false));
            campi.Add(new Campo(1, "campo3a", 0, string.Empty, string.Empty, false, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false));
            campi.Add(new Campo(1, "campo4a", 0, string.Empty, string.Empty, false, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false));
            Modello m = new Modello("unitTest_01",true,campi);
            vm.SelectedModel = m;
            Assert.IsNotNull(vm.SelectedModel);
            try
            {
                vm.AddNewItem();
            }
            catch (NullReferenceException)
            {
            }
            Assert.IsTrue(vm.SelectedModel.Id > 0);
        }
    }
}
