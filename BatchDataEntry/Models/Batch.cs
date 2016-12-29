using System;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;


namespace BatchDataEntry.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Batch")]
    public class Batch
    {

        public enum TipoFileProcessato
        {
            Tiff = 0,
            Pdf = 1
        }

        public event EventHandler BatchChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public TipoFileProcessato TipoFile { get; set; }
        public string DirectoryInput { get; set; }
        public string DirectoryOutput { get; set; }
        [Indexed]
        public int IdModello { get; set; }
        [Ignore]
        public Modello Applicazione { get; set; }
        public int NumDoc { get; set; }
        public int NumPages { get; set; }
        [Ignore]
        public long Dimensioni { get; set; }
        public int DocCorrente { get; set; }
        public int UltimoIndicizzato { get; set; }
        

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
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = new Modello();
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;      
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Nome = nome;
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

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Nome = nome;
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

        public Batch(int id, string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Id = id;
            this.Nome = nome;
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

        public Batch(int id, string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Id = id;
            this.Nome = nome;
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
