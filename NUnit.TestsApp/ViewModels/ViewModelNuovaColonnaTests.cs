using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.TestsApp.ViewModels
{
    [TestFixture()]
    public class ViewModelNuovaColonnaTests
    {
        protected ViewModelNuovaColonna vm;
        protected ViewModelNuovaColonna vm2;
        protected DatabaseHelperSqlServer dbc;

        [Test()]
        public void ViewModelNuovaColonnaTest()
        {
            ViewModelNuovaColonna tmp = new ViewModelNuovaColonna();
            Assert.IsNotNull(tmp);
        }

        [Test()]
        [SetUp]
        public void ViewModelNuovaColonnaTest1()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            dbc = new DatabaseHelperSqlServer(user, user, server, dbname);
            Campo c1 = new Campo("campoProva123", false, string.Empty, false);
            c1.IdModello = dbc.GetFirstModello().Id;
            vm = new ViewModelNuovaColonna(c1, false, dbc);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.SelectedCampo);
        }

        [Test()]
        public void ViewModelNuovaColonnaTest2()
        {
            Campo c1 = new Campo("campoProva123456789", false, string.Empty, false);
            c1.IdModello = dbc.GetFirstModello().Id;
            vm2 = new ViewModelNuovaColonna(c1, false, 3, dbc);
            Assert.IsNotNull(vm2);
            Assert.IsNotNull(vm2.SelectedCampo);
            Assert.IsTrue(vm2.SelectedCampo.Posizione == 3);
        }

        [Test()]
        public void AddNewItemTest()
        {
            Assert.IsNotNull(vm.SelectedCampo);
            vm.SelectedCampo.Nome = BatchDataEntry.Business.Utility.GetRandomAlphanumericString(19);
            try
            {
                vm.AddNewItem();
            }
            catch (NullReferenceException)
            {
                // l'eccezione è riferita alla funzione CloseWindow che non essendoci
                // nessun riferimento all'interfaccia generea l'exception
            }
            
            Campo c = dbc.GetCampoById(vm.SelectedCampo.Id);
            Assert.IsTrue(c.Nome.Equals(vm.SelectedCampo.Nome));
        }
    }
}
