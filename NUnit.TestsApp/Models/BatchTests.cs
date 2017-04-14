using NUnit.Framework;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models.Tests
{
    [TestFixture()]
    public class BatchTests
    {
        [Test()]
        public void BatchTest()
        {
            Batch b = new Batch();
            Assert.NotNull(b);
        }

        [Test()]
        public void BatchTest1()
        {
            Batch b = new Batch("test1", TipoFileProcessato.Pdf, @"C:\\In", @"C:\\out");
            Assert.NotNull(b);
        }

        [Test()]
        public void BatchTest2()
        {
            Batch b = new Batch("test1", TipoFileProcessato.Pdf, @"C:\\In", @"C:\\out", new Modello());
            Assert.NotNull(b);
        }

        [Test()]
        public void BatchTest3()
        {
            Batch b = new Batch("test1", TipoFileProcessato.Pdf, @"C:\\In", @"C:\\out", new Modello(), 1, 2, 0L, 3, 4, "aeio");
            Assert.NotNull(b);
        }

        [Test()]
        public void BatchTest4()
        {
            Batch b = new Batch(1, "test1", TipoFileProcessato.Pdf, @"C:\\In", @"C:\\out", new Modello(), 1, 2, 0L, 3, 4, "aeio");
            Assert.NotNull(b);
        }

        [Test()]
        public void BatchTest5()
        {
            Batch a = new Batch("test2", TipoFileProcessato.Tiff, @"C:\\Inaaa", @"C:\\outbbb", new Modello(), 1, 2, 0L, 3, 4, "aeio");
            Batch c = new Batch(a);
            Assert.NotNull(c);
        }

    }
}