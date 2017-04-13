using NUnit.Framework;
using BatchDataEntry.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Models;
using System.IO;

namespace BatchDataEntry.Business.Tests
{
    [TestFixture()]
    public class UtilityTests
    {
        [Test()]
        public void GetRandomAlphanumericStringTest()
        {
            string a = Utility.GetRandomAlphanumericString(15);
            string b = Utility.GetRandomAlphanumericString(15);
            Assert.IsTrue(a != b, "Le due stringhe genrate sono uguali invece che essere diverse");
        }

        [Test()]
        public void GetRandomStringTest()
        {
            IEnumerable<char> chars = new List<char>() {
                'a', 'b','c','d','e','f','g','h','i','1','2','3','4','5','6','7','8','9','0'
            };
            string a = Utility.GetRandomString(3, chars);
            string b = Utility.GetRandomString(3, chars);
            Assert.IsTrue(a != b, "Le due stringhe genrate sono uguali invece che essere diverse");
        }

        [Test()]
        public void CustomSortTest()
        {
            List<string> input = new List<string>() {
                "000009",
                "0000022",
                "000002",
                "000008",         
                "0000010",
                "000001"
            };
            List<string> expected = new List<string>() {
                "000001",
                "000002",
                "000008",
                "000009",
                "0000010",
                "0000022"};
            List<string> actual = input.CustomSort().ToList();
            Assert.NotNull(actual);
            Assert.AreEqual(input.Count, actual.Count);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test()]
        public void CustomSortTest1()
        {
            List<Document> input = new List<Document>() {
                new Document(6, "000099", @"C:\input\000099.pdf", false),
                new Document(4, "000021", @"C:\input\000021.pdf", false),               
                new Document(2, "000002", @"C:\input\000002.pdf", false),
                new Document(3, "000011", @"C:\input\000011.pdf", false),
                new Document(1, "000001", @"C:\input\000001.pdf", false),
                new Document(5, "000033", @"C:\input\000033.pdf", false)                  
            };
            List<Document> expected = new List<Document>() {
                new Document(1, "000001", @"C:\input\000001.pdf", false),
                new Document(2, "000002", @"C:\input\000002.pdf", false),
                new Document(3, "000011", @"C:\input\000011.pdf", false),
                new Document(4, "000021", @"C:\input\000021.pdf", false),
                new Document(5, "000033", @"C:\input\000033.pdf", false),
                new Document(6, "000099", @"C:\input\000099.pdf", false)
            };
            List<Document> actual = input.CustomSort().ToList();
            Assert.NotNull(actual);
            Assert.AreEqual(input.Count, actual.Count);
            for(int i=0; i < actual.Count; i++)
                Assert.IsTrue(actual.ElementAt(i).FileName == expected.ElementAt(i).FileName);
        }

        [Test()]
        public void CountFilesTest()
        {
            for (int i = 1; i <= 5; i++)
                File.Create(Path.Combine(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", string.Format("file{0}.pdf", i))).Dispose();

            try
            {
                int countFile = Utility.CountFiles(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", TipoFileProcessato.Pdf);
                Assert.IsTrue(countFile >= 4);
            }
            finally
            {
                for (int i = 1; i <= 5; i++)
                    File.Delete(Path.Combine(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", string.Format("file{0}.pdf", i)));
            }
        }

        [Test()]
        public void ConvertSizeTest()
        {
            long bytes = 1048576;
            double kb = Utility.ConvertSize(bytes, "KB");
            double mb = Utility.ConvertSize(bytes, "MB");
            double gb = Utility.ConvertSize(bytes, "GB");
            Assert.AreEqual(1024, kb);
            Assert.AreEqual(1, mb);
            Assert.AreEqual(0.00098, gb, 0.00005);
        }

        [Test()]
        public void ContainsOnlyNumbersTest()
        {
            string a = "123456789";
            string b = "1abc467op";
            Assert.IsTrue(Utility.ContainsOnlyNumbers(a));
            Assert.IsFalse(Utility.ContainsOnlyNumbers(b));
        }

        [Test()]
        public void RemovePatternFromStringTest()
        {
            string a = "DOC000001";
            string pat = "DOC";
            string exp = "000001";
            Assert.AreEqual(Utility.RemovePatternFromString(a, pat), exp);
        }

        [TearDown]
        public void CleanDir()
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}