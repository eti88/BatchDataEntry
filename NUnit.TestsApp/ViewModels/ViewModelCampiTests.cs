using NUnit.Framework;
using BatchDataEntry.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelCampiTests
    {
        private DatabaseHelperSqlServer dbc;
        private ViewModelCampi vm;
        private ViewModelCampi vm2;
        private ViewModelCampi vm3;
        private ViewModelCampi vm4;


        [SetUp]
        public void init() {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            dbc = new DatabaseHelperSqlServer(user, user, server, dbname);
        }

        [Test(), Order(1)]
        public void ViewModelCampiTest()
        {
            Assert.IsNotNull(dbc);
            vm = new ViewModelCampi();
            Assert.IsNotNull(vm);
        }

        [Test(), Order(2)]
        public void ViewModelCampiTest1()
        {
            Assert.IsNotNull(dbc);
            vm2 = new ViewModelCampi(dbc);
            Assert.IsNotNull(vm2);
        }

        [Test(), Order(3)]
        public void ViewModelCampiTest3()
        {
            Assert.IsNotNull(dbc);
            vm4 = new ViewModelCampi(dbc, 0);
            Assert.IsNotNull(vm4);
        }

        [Test(), Order(4)]
        public void GetColonneFromDbTest1()
        {
            Assert.IsNotNull(dbc);
            Assert.IsNotNull(vm2);
            int idMod = dbc.GetFirstModello().Id;
            vm2.GetColonneFromDb(dbc, idMod);
            Assert.IsNotNull(vm2.Colonne);
        }

        [Test(), Order(5)]
        public void DelItemTest()
        {
            Assert.IsNotNull(dbc);
            Assert.IsNotNull(vm2);
            Modello m = dbc.GetFirstModello();
            Campo c = dbc.CampoQuery(string.Format("SELECT * FROM Campi WHRE IdModello = {0}", m.Id)).FirstOrDefault();
            vm2.SelectedCampo = c;
            Assert.IsNotNull(vm2.SelectedCampo);
            vm2.DelItem();
            Assert.IsTrue(dbc.CampoQuery(string.Format(@"SELECT COUNT(Id) FROM Campi WHERE IdModello = {0}",m.Id)).Count == 0);
        }
    }
}
