using NUnit.Framework;
using System;
using System.IO;

namespace BatchDataEntry.ViewModels.Tests
{
    [TestFixture()]
    public class ViewModelToolsTests
    {
        private ViewModelTools vm;
        private string basepath = @"C:\Users\etien\Desktop\testing_dir\eurobrico_outtest\";
        private string dbname = "cache.db3";
        private string reportfile = "check-report.txt";

        [SetUp()]
        [Test()]
        public void InitTest()
        {
            vm = new ViewModelTools();
            vm.InputFilePath = Path.Combine(basepath, dbname); // Deve essere un sqlite
            vm.GenerateOutputFile = true;
            vm.CodiceAssociato = "00055";
            vm.CodiceNegozio = "00112";
        }

        [Test()]
        public void LoadFileTest()
        {
            Assert.NotNull(vm);
            vm.LoadFile();
            Assert.IsTrue(vm.Records.Count > 0);
        }

        [Test()]
        public void CheckFileTest()
        {
            string path = Path.Combine(basepath, reportfile);
            Assert.NotNull(vm);
            vm.CheckFile();
            Assert.IsTrue(File.Exists(path));
            Assert.IsFalse(new FileInfo(path).Length == 0);
        }

        [Test()]
        public void GeneraFilesTest()
        {
            Assert.NotNull(vm);
            vm.GeneraFiles();
            string FID = string.Format("FID{0}{1}{2}.FID", vm.CodiceAssociato, vm.CodiceNegozio, DateTime.Now.ToString("yyyyMMdd"));
            string CHK = string.Format("FID{0}{1}{2}.CHK", vm.CodiceAssociato, vm.CodiceNegozio, DateTime.Now.ToString("yyyyMMdd"));
            Assert.IsTrue(File.Exists(Path.Combine(basepath, FID)));
            Assert.IsTrue(File.Exists(Path.Combine(basepath, CHK)));
            Assert.IsFalse(new FileInfo(Path.Combine(basepath, FID)).Length == 0);
            Assert.IsFalse(new FileInfo(Path.Combine(basepath, CHK)).Length == 0);
        }
    }
}