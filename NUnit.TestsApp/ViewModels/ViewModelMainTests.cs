using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NUnit.Framework;
using System.Collections.Generic;

namespace NUnit.TestsApp.ViewModels
{
    [TestFixture()]
    public class ViewModelMainTests
    {
        protected ViewModelMain viewModel;
        protected DatabaseHelperSqlServer db;

        [Test()]
        [SetUp]
        public void ViewModelMainTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            db = new DatabaseHelperSqlServer(user, user, server, dbname);
            viewModel = new ViewModelMain();
            Assert.IsNotNull(viewModel);
        }

        [Test(), Order(1)]
        public void LoadBatchesTest()
        {
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(db, "Connessione al database fallita");
            viewModel.LoadBatches(db);
            Assert.IsTrue(viewModel.Batches.Count > 0);
        }

        [Test(), Order(2)]
        public void DeleteBatchTest()
        {
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(db);
            if(viewModel.Batches == null) viewModel.LoadBatches(db);
            Assert.IsTrue(viewModel.Batches.Count > 0);
            Batch b = db.GetFirstBatch();
            viewModel.SelectedBatch = b;
            viewModel.DeleteBatchItem(db);
            List<Batch> l = new List<Batch>(viewModel.Batches);
            Assert.IsTrue(!l.Exists(x => x.Id == b.Id));
        }
    }
}
