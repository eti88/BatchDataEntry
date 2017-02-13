using System;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class CampoTest
    {
        [TestMethod]
        public void TestEqualTrue()
        {
            Campo campo = new Campo(0, "ColonnaTest", 1, false, string.Empty, true, false, 0);
            Campo campoCopy = new Campo(campo);
            bool actual = campo.Equals(campoCopy);
            Assert.AreEqual(true, actual);          
        }

        [TestMethod]
        public void TestEqualFalse()
        {
            Campo campo = new Campo(3, "Colonna1", 1, false, string.Empty, true, false, 0);
            Campo campo2 = new Campo(1, "Colonna2", 2, false, string.Empty, true, false, 1);
            bool actual = campo.Equals(campo2);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestRevert()
        {
            Campo campo = new Campo(0, "ColonnaTest", 1, false, string.Empty, true, false, 0);
            Campo campoCopy = new Campo(campo);         
            campo.Nome = "Nuovo nome";
            campo.Posizione = 99;
            campo.SalvaValori = true;
            campo.Revert();
            bool actual = campo.Equals(campoCopy);
            Assert.AreEqual(true, actual);
        }
    }
}
