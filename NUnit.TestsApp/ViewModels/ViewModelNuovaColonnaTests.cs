using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using NUnit.Framework;

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
            Campo c = new Campo(1, "campo12345", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            ViewModelNuovaColonna vm2 = new ViewModelNuovaColonna(c, false, dbsql);
            Assert.IsNotNull(vm2);
            Assert.IsNotNull(vm2.SelectedCampo);
        }
        
    }
}
