using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BatchDataEntry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BatchDataEntry.Helpers;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class DbHelperTest
    {
        private DatabaseHelper dbh = new DatabaseHelper();

        [TestMethod]
        public async Task InitTaskTest()
        {
            dbh.InitTabs();
        }

        [TestMethod]
        public async Task InsertCampoTaskTest()
        {
            Campo c1 = new Campo("colonnaIndice", false, "", true);
            Campo c2 = new Campo("colonna2", true, "prova", true);
            await dbh.InsertAsync(c1);
            await dbh.InsertAsync(c2);
        }
        
        [TestMethod]
        public async Task InsertModelloTaskTest()
        {
            Campo c1 = new Campo("colonnaIndice", false, "", true);
            Campo c2 = new Campo("colonna2", true, "prova", true);
            Campo c3 = new Campo("colonna3", false, "", true);

            List<Campo> listCampi = new List<Campo>();
            listCampi.Add(c1);
            listCampi.Add(c2);
            listCampi.Add(c3);

            Modello m = new Modello("test modello", Batch.TipoFileProcessato.Pdf, false, listCampi, new FileCSV());
            await dbh.InsertAsync(m);
        }

        [TestMethod]
        public async Task InsertOrigineCsvTaskTest()
        {
            FileCSV fc = new FileCSV("null/path", ';');
            await dbh.InsertAsync(fc);
        }

        [TestMethod]
        public async Task InsertBatchTaskTest()
        {
            Campo c1 = new Campo("colonnaIndiceb", false, "", true);
            Campo c2 = new Campo("colonna2b", true, "prova", true);
            Campo c3 = new Campo("colonna3b", false, "", true);

            List<Campo> listCampi = new List<Campo>();
            listCampi.Add(c1);
            listCampi.Add(c2);
            listCampi.Add(c3);

            FileCSV fc = new FileCSV("null/path", ';');

            Modello m = new Modello("test modello b", Batch.TipoFileProcessato.Pdf, false, listCampi, fc);
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "output_test"));
            Batch b = new Batch("Unit test batch", Batch.TipoFileProcessato.Pdf, 
                Path.Combine(Directory.GetCurrentDirectory(), "pdf_test"), 
                Path.Combine(Directory.GetCurrentDirectory(), "output_test"), 
                m, 0, 0, 0L, 0, 0);
            await dbh.InsertAsync(b);
        }

        [TestMethod]
        public async Task GetRecordsCampo()
        {
            var res = dbh.GetRecordsAsync(typeof(Batch));
            Console.WriteLine(res);
        }

        [TestMethod]
        public async Task GetRecordsFile()
        {
            var res = dbh.GetRecordsAsync(typeof(Batch));
            Console.WriteLine(res);
        }

        [TestMethod]
        public async Task GetRecordsModello()
        {
            var res = dbh.GetRecordsAsync(typeof(Batch));
            Console.WriteLine(res);
        }

        [TestMethod]
        public async Task GetRecordsBatch()
        {
            var res = dbh.GetRecordsAsync(typeof(Batch));
            Console.WriteLine(res);
        }
    }
}
