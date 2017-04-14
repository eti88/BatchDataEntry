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
    public class DocumentTests
    {
        [Test()]
        public void IsDirectoryTest()
        {
            Document doc = new Document(1, "aaaa", @"C:\Users\etien\Documents\Visual Studio 2015\Projects\BatchDataEntry\NUnit.TestsApp\bin\testFiles", false);
            Assert.IsTrue(doc.IsDirectory());
        }

        [Test()]
        public void DocumentTest()
        {
            Document doc = new Document(new Batch(), new Dictionary<int, string>());
            Assert.IsNotNull(doc);
        }

        [Test()]
        public void DocumentTest1()
        {
            Document doc = new Document(1, "aaaa", @"C:\\doc1.pdf", false);
            Assert.IsNotNull(doc);
        }

        [Test()]
        public void DocumentTest2()
        {
            Document doc = new Document(1, "aaaa", @"C:\\doc1.pdf", false);
            Document dco2 = new Document(doc);
            Assert.IsNotNull(dco2);
        }

        [Test()]
        public void DocumentTest3()
        {
            Document doc = new Document();
            Assert.IsNotNull(doc);
        }

        [Test()]
        public void AddInputsToPanelTest()
        {
            Document doc = new Document(1, "aaaa", @"C:\\doc1.pdf", false);
            Batch b = new Batch("aaa", TipoFileProcessato.Pdf, @"C:\\input", @"C:\\out");
            Modello m = new Modello("bbb", false, new System.Collections.ObjectModel.ObservableCollection<Campo>());
            Campo c1 = new Campo("campo1", false, string.Empty, true);
            Campo c2 = new Campo("campo1", false, string.Empty, true);
            m.Campi.Add(c1);
            m.Campi.Add(c2);
            b.Applicazione = m;
            doc.AddInputsToPanel(b, new Helpers.DatabaseHelper());
            Assert.IsTrue(doc.Voci.Count == 2);
        }
    }
}