using SQLite;

namespace BatchDataEntry.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("FileCSV")]
    public class FileCSV
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string Path { get; set; }
        [MaxLength(1)]
        public string Separatore { get; set; }

        public FileCSV()
        {
        }

        public FileCSV(string p, string s)
        {
            this.Path = p;
            this.Separatore = s;
        }

        public FileCSV(long id, string p, string s)
        {
            this.Id = id;
            this.Path = p;
            this.Separatore = s;
        }
    }
}
