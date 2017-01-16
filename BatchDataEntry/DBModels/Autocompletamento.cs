using SQLite;

namespace BatchDataEntry.DBModels
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Autocompletamento")]
    public class Autocompletamento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int Colonna { get; set; }
        [NotNull]
        [MaxLength(255)]
        public string Valore { get; set; }

        public Autocompletamento() { }
    }
}
