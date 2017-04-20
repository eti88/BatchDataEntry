using BatchDataEntry.ViewModels;
using BatchDataEntry.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using BatchDataEntry.Models;
using System;
using System.IO;

namespace NUnit.TestsApp.ViewModels
{
    [TestFixture()]
    public class ViewModelBatchSelectedTests
    {
        protected DatabaseHelperSqlServer db;
        protected ViewModelBatchSelected vm;
        private string basepath = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\origin\tiff\";
        //private string tiff = @"CCITT_1.TIF";
        private string pdf = @"out.pdf";


        [SetUp]
        [Test()]
        public void ViewModelBatchSelectedTest()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            db = new DatabaseHelperSqlServer(user, user, server, dbname);

            Batch b = db.GetFirstBatch();
            Assert.IsNotNull(b);
            vm = new ViewModelBatchSelected(b, db);
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.CurrentBatch);
            Assert.IsNotNull(vm.CurrentBatch.Applicazione);
            Assert.IsTrue(vm.SelectedRowIndex == 0);
        }

        [Test()]
        public void CheckBatchTest()
        {
            Assert.IsNotNull(vm);
            try
            {
                vm.CheckBatch();
                Assert.IsTrue(true);
            }
            catch(Exception e)
            {
                Assert.IsTrue(false, e.ToString());
            }
        }

        [Test()]
        public void ConvertTiffToPdfTest()
        {
            string t = basepath;
            string p = Path.Combine(basepath, @"conv", pdf);
            Assert.IsNotNull(vm);
            vm.ConvertTiffToPdf(t, p);
            Assert.IsTrue(File.Exists(p) && new FileInfo(p).Length > 0);
        }

        //[Test()]
        //public void GenerateListTxtTest()
        //{
        //    string user = @"unitTest";
        //    string server = @"localhost\SQLEXPRESS";
        //    string dbname = @"db_BatchDataEntry_unitTest";
        //    db = new DatabaseHelperSqlServer(user, user, server, dbname);
        //    Batch b = db.GetFirstBatch();
        //    Assert.IsNotNull(b);
        //    vm = new ViewModelBatchSelected(b, db);
        //    Assert.IsNotNull(vm);
        //    Assert.IsNotNull(vm.SelectedBatch);
        //    vm.SelectedBatch.DirectoryInput = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\in";
        //    vm.SelectedBatch.DirectoryOutput = @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles\out";
        //    vm.GenerateListTxt();
        //    Assert.IsTrue(File.Exists(Path.Combine(vm.SelectedBatch.DirectoryOutput, @"LISTA.txt")));
        //}
    }
}
