using NUnit.Framework;

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
        public void AddInputsToPanelTest()
        {
            Document doc = new Document(1, "aaaa", @"C:\\doc1.pdf", false);
            Batch b = new Batch("aaa", TipoFileProcessato.Pdf, @"C:\\input", @"C:\\out");
            Modello m = new Modello("bbb", false, new System.Collections.ObjectModel.ObservableCollection<Campo>());
            Campo c1 = new Campo(1, "campo1", 0, string.Empty, string.Empty, true, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            Campo c2 = new Campo(1, "campo2", 0, string.Empty, string.Empty, false, false, Helpers.EnumTypeOfCampo.Normale, 1, false, false);
            m.Campi.Add(c1);
            m.Campi.Add(c2);
            b.Applicazione = m;
            doc.AddInputsToPanel(b, new Helpers.DatabaseHelper());
            Assert.IsTrue(doc.Voci.Count == 2);
        }
    }
}