using BatchDataEntry.Models;
using SQLite;

namespace BatchDataEntry.DBModels
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Batch")]
    public class Batch
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public TipoFileProcessato TipoFile { get; set; }
        public string DirectoryInput { get; set; }
        public string DirectoryOutput { get; set; }
        public int IdModello { get; set; }
        public int NumDoc { get; set; }
        public int NumPages { get; set; }
        public int DocCorrente { get; set; }
        public int UltimoIndicizzato { get; set; }

        public Batch() { }

        public Batch(Models.Batch b)
        {
            this.Id = b.Id;
            this.Nome = b.Nome;
            this.TipoFile = b.TipoFile;
            this.DirectoryInput = b.DirectoryInput;
            this.DirectoryOutput = b.DirectoryOutput;
            this.IdModello = b.IdModello;
            this.NumDoc = b.NumDoc;
            this.NumPages = b.NumPages;
            this.DocCorrente = b.DocCorrente;
            this.UltimoIndicizzato = b.UltimoIndicizzato;
        }
    }
}
