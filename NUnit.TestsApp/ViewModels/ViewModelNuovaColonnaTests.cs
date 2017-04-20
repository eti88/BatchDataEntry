using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelNuovaColonnaTests
    {
        public DatabaseHelperSqlServer dbsql;
        public ViewModelNuovaColonna vm;
        public ViewModelNuovaColonna vm2;

        [SetUp]
        public void init()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            dbsql = new DatabaseHelperSqlServer(user, user, server, dbname);
        }

        [Test(), Order(1)]
        public void ViewModelNuovaColonnaTest()
        {
            Assert.IsNotNull(dbsql);
            ViewModelNuovaColonna v = new ViewModelNuovaColonna();
            Assert.IsNotNull(v);
        }

        [Test(), Order(2)]
        public void ViewModelNuovaColonnaTest1()
        {
            Assert.IsNotNull(dbsql);
            ViewModelNuovaColonna vm = new ViewModelNuovaColonna(dbsql);
            Assert.IsNotNull(vm);
        }

        [Test(), Order(3)]
        public void ViewModelNuovaColonnaTest2()
        {
            Assert.IsNotNull(dbsql);
            Campo c = new Campo("campo1234", false, string.Empty, true);
            ViewModelNuovaColonna vm2 = new ViewModelNuovaColonna(c, false, dbsql);
            Assert.IsNotNull(vm2);
            Assert.IsNotNull(vm2.SelectedCampo);
        }

        //[Test(), Order(4)]
        //public void AddNewItemTest()
        //{
        //    Assert.IsNotNull(dbsql);
        //    Campo c = new Campo("campo1234fdkshfd", false, string.Empty, true);
        //    ViewModelNuovaColonna vm2 = new ViewModelNuovaColonna(c, false, dbsql);
        //    vm2.SelectedCampo = c;
        //    try
        //    {
        //        vm2.AddNewItem();
        //    }
        //    catch (NullReferenceException)
        //    {

        //    }
        //    Campo resp = dbsql.CampoQuery(string.Format("SELECT * FROM Campi WHERE Nome = {0}", c.Nome)).First();
        //    Assert.IsNotNull(resp);
        //    Assert.IsTrue(resp.Nome.Equals(c.Nome));
        //}
    }
}
