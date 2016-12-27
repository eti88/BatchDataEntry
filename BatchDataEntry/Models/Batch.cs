using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;


namespace BatchDataEntry.Models
{
    [Table("Batch")]
    public class Batch
    {
        public enum TipoFileProcessato
        {
            Tiff = 0,
            Pdf = 1
        }

        public event EventHandler BatchChanged;

        public long Id { get; set; }

        public string Nome { get; set; }
        public TipoFileProcessato TipoFile { get; set; }
        public string DirectoryInput { get; set; }
        public string DirectoryOutput { get; set; }

        public Modello Applicazione { get; set; }
        public int NumDoc { get; set; }
        public int NumPages { get; set; }
        public long Dimensioni { get; set; }
        public int DocCorrente { get; set; }
        public int UltimoIndicizzato { get; set; }
        public int FKModello { get; set; }

        /*
         - Nel caso della cancellazione di un documento oltre all'eliminazione del record bisogna eliminare anche il documento di origine (file)
         - Nella cartella di input si genera un file .ini per associare:
            * indice -> cartella (nel caso dei Tiff)
            * indice -> pdf (nel caso del pdf)
         - Nella cartella do output vengono generati:
            * copia del pdf inserito
            * file cache (ini)
            * file db (csv
         */

        public Batch()
        {
            Applicazione = new Modello();
            NumDoc = 0;
            NumPages = 0;
            Dimensioni = 0L;
            DocCorrente = 0;
            UltimoIndicizzato = 0;
            this.FKModello = -1;
        }

        public Batch(string Nome, TipoFileProcessato t, string input, string output)
        {
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = new Modello();
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;
            this.FKModello = -1;
        }

        public Batch(string Nome, TipoFileProcessato t, string input, string output, Modello m)
        {
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;
            this.FKModello = -1;
        }

        public Batch(string Nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
            this.FKModello = -1;
        }

        public Batch(string Nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
            this.FKModello = fk;
        }

        public Batch(int id, string Nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Id = id;
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
        }

        public Batch(int id, string Nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Id = id;
            this.Nome = Nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
            this.FKModello = fk;
        }

        void StausTipoFileProcessatoChangeTick(object stato)
        {
            TipoFile = TipoFile == TipoFileProcessato.Tiff ? TipoFileProcessato.Pdf : TipoFileProcessato.Tiff;
        }

        public void GeneraDirOutput() { }
        public void ImportaPdf(string sorgente) { }
        public void ConvertiTiffInPdf() { }
        public void RimuoviDocumento(int indice) { }

    }
}
