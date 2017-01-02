using System;
using System.Collections.ObjectModel;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class ModelloTest
    {
        [TestMethod]
        public void TestEqualsTrue()
        {
            Modello modello = new Modello(0, "ModelloTest", false, new ObservableCollection<Campo>(), string.Empty, string.Empty);
            Modello copy = new Modello(modello);
            bool actual = modello.Equals(copy);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void TestEqualsFalse()
        {
            Modello modello = new Modello(0, "ModelloTest1", true, new ObservableCollection<Campo>(), "C:/test/1/ac.csv", ";");
            Modello modello2 = new Modello(9, "ModelloTest2", true, new ObservableCollection<Campo>(), "D:/test/3/ac.csv", ";");
            bool actual = modello.Equals(modello2);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestRevert()
        {
            Modello modello = new Modello(0, "ModelloTest1", true, new ObservableCollection<Campo>(), "C:/test/1/ac.csv", ";");
            Modello mc = new Modello(modello);
            modello.Nome = "AAAAAAAAAAAAAAAAA";
            modello.OrigineCsv = false;
            modello.Revert();
            bool actual = modello.Equals(mc);
            Assert.AreEqual(true, actual);
        }

    }
}
