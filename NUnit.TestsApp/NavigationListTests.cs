using BatchDataEntry.Components;
using NUnit.Framework;

namespace NUnit.TestsApp
{
    [TestFixture()]
    public class NavigationListTests
    {
        private NavigationList<string> list;

        [SetUp]
        public void Init()
        {
            list = new NavigationList<string>();
            for(int i = 0; i < 1000; i++)
            {
                list.Add(i.ToString("D8"));
            }   
        }

        [Test()]
        public void CurrentIndexTest()
        {
            Assert.IsNotNull(list);
            Assert.IsTrue(list.CurrentIndex == 0);
        }

        [Test()]
        public void MoveNextTest()
        {
            Assert.IsNotNull(list);
            string a = list.Current;
            string b = list.MoveNext;
            Assert.IsNotNull(b);
            Assert.IsTrue(a != b);
        }

        [Test()]
        public void MovePreviousTest()
        {
            Assert.IsNotNull(list);
            string a = list.Current;
            string b = list.MovePrevious;
            Assert.IsNotNull(b);
            Assert.IsTrue(a != b);
        }

    }
}
