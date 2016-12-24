using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

        public int Id { get; set; }

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
