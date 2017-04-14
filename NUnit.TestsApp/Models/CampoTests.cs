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
    public class CampoTests
    {
        [Test()]
        public void CampoTest()
        {
            Campo c1 = new Campo();
            Assert.NotNull(c1);
        }

        [Test()]
        public void CampoTest1()
        {
            Campo c1 = new Campo("campo1", false,"asd", true);
            Assert.NotNull(c1);
        }

        [Test()]
        public void CampoTest2()
        {
            Campo c1 = new Campo("campo1", false, "asd", true, false, false);
            Assert.NotNull(c1);
        }

        [Test()]
        public void CampoTest3()
        {
            Campo c1 = new Campo(1, "campo1", 0, false, "asd", false, false, true, 1);
            Assert.NotNull(c1);
        }

        [Test()]
        public void CampoTest4()
        {
            Campo c1 = new Campo(1, "campo1", 0, false, "asd", false, false, true, 1);
            Campo c2 = new Campo(c1);
            Assert.NotNull(c2);
        }

        [Test()]
        public void RevertTest()
        {
            Campo c1 = new Campo("campo1", false, "asd", true);
            c1.Nome = "campo2";
            c1.Revert();
            Assert.IsNotNull(c1);
            Assert.IsTrue(c1.Nome.Equals("campo1"));
        }

        [Test()]
        public void EqualsTest()
        {
            Campo c1 = new Campo(1, "campo1", 0, false, "asd", false, false, true, 1);
            Campo c2 = new Campo(2, "campo2", 0, false, "asd", false, false, true, 1);
            Campo c3 = new Campo(c1);
            Assert.AreNotEqual(c1, c2);
            Assert.AreEqual(c1, c3);
        }
    }
}