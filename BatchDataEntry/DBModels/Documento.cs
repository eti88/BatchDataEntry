using SQLite;

namespace BatchDataEntry.DBModels
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Documento")]
    public  class Documento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        [MaxLength(255)]
        public string FileName { get; set; }
        public string Path { get; set; }
        public bool isIndicizzato { get; set; }

        public Documento() { }

        public Documento(int id, string name, string path, bool indicizzato)
        {
            this.Id = id;
            this.FileName = name;
            this.Path = path;
            this.isIndicizzato = indicizzato;
        }

        public Documento(Models.Doc doc)
        {
            this.FileName = doc.FileName;
            this.Path = doc.Path;
            this.isIndicizzato = doc.IsIndexed;
        }
    }
}
