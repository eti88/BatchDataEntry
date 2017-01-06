using System;
using System.IO;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void TestGetDirectorySize()
        {
            long res;
            bool v = false;
            try
            {
                res = BatchDataEntry.Business.Utility.GetDirectorySize(Directory.GetCurrentDirectory());
                if (res > 0)
                    v = true;
                   
                Assert.IsTrue(v);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestCountFiles()
        {
            try
            {
                File.Create(Path.Combine(Directory.GetCurrentDirectory(), "test.pdf")).Dispose();
                int res = BatchDataEntry.Business.Utility.CountFiles(Directory.GetCurrentDirectory(), TipoFileProcessato.Pdf);
                if(res == 1)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);

                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "test.pdf"));
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestConvertSize()
        {
            long bytes = 1048576;
            double kb = BatchDataEntry.Business.Utility.ConvertSize(bytes, "KB");
            double mb = BatchDataEntry.Business.Utility.ConvertSize(bytes, "MB");
            double gb = BatchDataEntry.Business.Utility.ConvertSize(bytes, "GB");
            Assert.AreEqual(1024, kb);
            Assert.AreEqual(1, mb);
            Assert.AreEqual(0.00098, gb, 0.00005);
        }

        [TestMethod]
        public void TestOnlyNum()
        {
            string a = "123456789";
            string b = "1abc467op";
            Assert.IsTrue(BatchDataEntry.Business.Utility.ContainsOnlyNumbers(a));
            Assert.IsFalse(BatchDataEntry.Business.Utility.ContainsOnlyNumbers(b));
        }

        [TestMethod]
        public void Test()
        {
            string a = "DOC000001";
            string pat = "DOC";
            string exp = "000001";

            Assert.AreEqual(BatchDataEntry.Business.Utility.RemovePatternFromString(a, pat), exp);
        }
    }
}
