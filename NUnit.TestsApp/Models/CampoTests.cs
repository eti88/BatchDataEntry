using NUnit.Framework;

namespace BatchDataEntry.Models.Tests
{
    [TestFixture()]
    public class CampoTests
    {
        [Test()]
        public void CampoTest4()
        {
            Campo c1 = new Campo(1, "campo1", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1,false, false);
            Campo c2 = new Campo(c1);
            Assert.NotNull(c2);
        }

        [Test()]
        public void RevertTest()
        {
            Campo c1 = new Campo(1, "campo1", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            c1.Nome = "campo2";
            Assert.IsNotNull(c1);
            Assert.IsTrue(c1.Nome.Equals("campo1"));
        }

        [Test()]
        public void EqualsTest()
        {
            Campo c1 = new Campo(1, "campo1", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            Campo c2 = new Campo(1, "campo2", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            Campo c3 = new Campo(c1);
            Assert.AreNotEqual(c1, c2);
            Assert.AreEqual(c1, c3);
        }
    }
}