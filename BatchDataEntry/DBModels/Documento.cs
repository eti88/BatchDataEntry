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
    }
}
