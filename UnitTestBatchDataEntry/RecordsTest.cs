using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class RecordsTest
    {
        private static string _path = Path.Combine(Directory.GetCurrentDirectory(), "testFile.bin ");

        [TestMethod]
        public void SaveTest()
        {
            try
            {   
                Records records = new Records();
                for (int i = 0; i < 10; i++)
                {
                    RecordRow r = new RecordRow();
                    r.Id = i;
                    r.Cells.Add("aaa", "1111");
                    r.Cells.Add("abb", "1234");
                    r.Cells.Add("abc", "ajkfjdklsjlk");
                    records.Rows.Add(r);
                }
                Task t = new Task(async () => await records.Save(_path));
                t.Start();              
            }
            catch (Exception)
            {   
                Assert.IsTrue(false);
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ReadTest()
        {
            try
            {
                Records records = new Records();
                records.Load(_path);
                
                foreach (RecordRow a in records.Rows)
                {
                    Console.WriteLine(string.Format("Id: {0}", a.Id));
                    foreach (var row in a.Cells)
                    {
                        Console.WriteLine(String.Format("[{0},{1}]", row.Key, row.Value));
                    }        
                }
            }
            catch (Exception)
            {
                Assert.IsTrue(false);
            }
            Assert.IsTrue(true);
        }
    }
}
