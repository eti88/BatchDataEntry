using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class NavigationListTest
    {
        [TestMethod]
        public void TestBase()
        {
            NavigationList<string> list = new NavigationList<string>();
            list.Add("A");
            list.Add("B");
            list.Add("C");
            
            Assert.AreEqual(list.Current, "A");
            Assert.AreEqual(list.MoveNext, "B");
            Assert.AreEqual(list.MovePrevious, "A");
            Assert.AreEqual(list.MoveNext, "B");
            Assert.AreEqual(list.MoveNext, "C");
            Assert.AreEqual(list.MoveNext, "C");
        }

        [TestMethod]
        public void TestHasNextPrev()
        {
            NavigationList<string> list = new NavigationList<string>();
            list.Add("A");
            list.Add("B");
            Assert.IsFalse(list.hasPrevious);
            Assert.AreEqual(list.MoveNext, "B");
            Assert.IsFalse(list.hasNext);
            Assert.IsTrue(list.hasPrevious);
        }
    }
}
