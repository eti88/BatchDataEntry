using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BatchDataEntry.Components;
using System.IO;
using BatchDataEntry.Models;

namespace BatchDataEntry.Helpers.Tests
{
    [TestFixture()]
    public class CacheDocumentReceiverTests
    {
        NavigationList<Dictionary<int, string>> nav;

        [SetUp()]
        public void Setup()
        {
            nav = new NavigationList<Dictionary<int, string>>();
            try
            {
                var dbCache = new DatabaseHelper(@"C:\Users\etien\Desktop\testing_dir\eurobrico_outtest\cache.db3", @"C:\Users\etien\Desktop\testing_dir\eurobrico_outtest");
                nav = dbCache.GetDocuments();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (nav == null || nav.Count == 0)
                {
                    Console.WriteLine("La lista dei documenti risulta vuota o è fallita per qualche motivo(Errore Precedente): LoadDocsList()");
                }
            }
        }

        [Test()]
        public void FireInitTest()
        {
            CacheDocumentReceiver c = new CacheDocumentReceiver();
            Assert.IsTrue(nav.Count > 0);
            List<string> tmp = new List<string>();
            if (nav.hasPrevious)
            {
                tmp.Add(new Document(nav[nav.CurrentIndex - 1]).Path);
            }

            tmp.Add(new Document(nav[nav.CurrentIndex]).Path);

            if (nav.hasNext)
            {
                tmp.Add(new Document(nav[nav.CurrentIndex + 1]).Path);
            }
            c.FireDocChanged(tmp);

            string[] fileArray = Directory.GetFiles(Path.Combine(Path.GetTempPath(), "_batchtmp"), "*.pdf");
            Assert.IsTrue(fileArray.Count() > 0);
        }

        [Test()]
        public void PrintTmpDirTest()
        {
            string file = @"C:\Users\etien\Desktop\testing_dir\aaaeuro\00001.tif";
            string outex = Path.Combine(Path.GetTempPath(), "_batchtmp", "00001.pdf");
            CacheDocumentReceiver c = new CacheDocumentReceiver();
            string t = c.TempFilePath(file);
            Assert.IsTrue(t.Equals(outex));
        }
    }
}