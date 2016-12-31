using SQLite;

namespace BatchDataEntry.DBModels
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Campo")]
    public class Campo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        [MaxLength(255)]
        public string Nome { get; set; }
        public int Posizione { get; set; }
        public bool SalvaValori { get; set; }
        [MaxLength(255)]
        public string ValorePredefinito { get; set; }
        public bool IndicePrimario { get; set; }
        public int TipoCampo { get; set; }
        [Indexed]
        public int IdModello { get; set; }

        public Campo()
        {
            
        }

        public Campo(Models.Campo c)
        {
            this.Id = c.Id;
            this.Nome = c.Nome;
            this.Posizione = c.Posizione;
            this.SalvaValori = c.SalvaValori;
            this.ValorePredefinito = c.ValorePredefinito;
            this.IndicePrimario = c.IndicePrimario;
            this.TipoCampo = c.TipoCampo;
            this.IdModello = c.IdModello;
        }
    }
}
