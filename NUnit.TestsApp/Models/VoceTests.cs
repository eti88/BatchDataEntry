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
    public class VoceTests
    {
        [Test()]
        public void VoceTest()
        {
            Voce v1 = new Voce();
            Assert.NotNull(v1);
        }

        [Test()]
        public void VoceTest1()
        {
            Voce v1 = new Voce(1, "key1");
            Assert.NotNull(v1);
        }

        [Test()]
        public void VoceTest2()
        {
            Voce v1 = new Voce("key1", "val1");
            Assert.NotNull(v1);
        }

        [Test()]
        public void VoceTest3()
        {
            Voce v1 = new Voce(1, "key1", "val1");
            Assert.NotNull(v1);
        }

        [Test()]
        public void VoceTest4()
        {
            Voce v1 = new Voce(1, "key1", false, "NULL");
            Assert.NotNull(v1);
        }

        [Test()]
        public void VoceTest5()
        {
            Voce v1 = new Voce(1, "key1", "valll", false, "NULL");
            Assert.NotNull(v1);
        }
    }
}