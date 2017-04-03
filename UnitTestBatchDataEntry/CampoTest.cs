using System;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class CampoTest
    {
        [TestMethod]
        public void CampoHashCodeTestDiff()
        {
            Random rnd = new Random();
            List<int> HashCampi = new List<int>();
            for(int i=0; i < 50; i++)
            {
                Campo c = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(10),
                rnd.Next(0, 2) == 0,
                (rnd.Next(0, 2) == 0) ? BatchDataEntry.Business.Utility.GetRandomAlphanumericString(7) : string.Empty,
                rnd.Next(0, 2) == 0,
                rnd.Next(0, 2) == 0,
                rnd.Next(0, 2) == 0);
                HashCampi.Add(c.GetHashCode());
            }
            HashCampi.Sort();
            Assert.AreEqual(HashCampi.Count, HashCampi.Distinct().Count());     
        }

        [TestMethod]
        public void CampoEqualsTrue1Test()
        {
            Campo c1 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(10), false, string.Empty, true);
            Campo c2 = c1;
            Assert.AreEqual(c1, c2);           
        }

        [TestMethod]
        public void CampoEqualsTrue2Test()
        {
            Campo c1 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(10), false, string.Empty, true);
            Campo c2 = new Campo(c1);
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void CampoEqualsFalse1Test()
        {
            Campo c1 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(10), false, string.Empty, true);
            Campo c2 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(16), true, string.Empty, false);
            Assert.AreNotEqual(c1, c2);
        }

        [TestMethod]
        public void CampoEqualsFalse2Test()
        {
            Campo c1 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(8), false, BatchDataEntry.Business.Utility.GetRandomAlphanumericString(5), true);
            Campo c2 = new Campo(BatchDataEntry.Business.Utility.GetRandomAlphanumericString(6), true, BatchDataEntry.Business.Utility.GetRandomAlphanumericString(18), false);
            Assert.AreNotEqual(c1, c2);
        }
    }
}
