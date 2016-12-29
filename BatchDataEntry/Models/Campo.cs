using SQLite;

namespace BatchDataEntry.Models
{

    [System.ComponentModel.DataAnnotations.Schema.Table("Campo")]
    public class Campo : BaseModel
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
        public int TipoCampo { get; set; }  // Utilizzato per future implementazioni (es textbox[0], combobox[1], checkbox[2])
        [Indexed]
        public int IdModello { get; set; }

        /*
         SalvaValori permette di tenere traccia dei dati nel medesimo campo velocizzando
         futuri inserimenti.
         */

        public Campo()
        {
            this.TipoCampo = 0;
            //this.FKModello = -1;
        }

        public Campo(string nome, bool sv, string vp, bool ip)
        {
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            //this.FKModello = -1;
        }

        public Campo(string nome, bool sv, string vp, bool ip, int fk)
        {
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            //this.FKModello = fk;
        }

        public Campo(int id, string nome, bool sv, string vp, bool ip)
        {
            this.Id = id;
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            //this.FKModello = -1;
        }

        public Campo(int id, string nome, bool sv, string vp, bool ip, int fk)
        {
            this.Id = id;
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            //this.FKModello = fk;
        }
    }
}
