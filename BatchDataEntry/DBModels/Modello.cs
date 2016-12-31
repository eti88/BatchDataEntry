using SQLite;

namespace BatchDataEntry.DBModels
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Modello")]
    public class Modello
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool OrigineCsv { get; set; }
        public string PathFileCsv { get; set; }
        [MaxLength(1)]
        public string Separatore { get; set; }

        public Modello() { }

        public Modello(Models.Modello m)
        {
            this.Id = m.Id;
            this.Nome = m.Nome;
            this.OrigineCsv = m.OrigineCsv;
            this.PathFileCsv = m.PathFileCsv;
            this.Separatore = m.Separatore;
        }
    }
}
