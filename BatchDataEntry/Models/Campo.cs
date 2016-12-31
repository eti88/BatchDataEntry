using SQLite;

namespace BatchDataEntry.Models
{

    public class Campo : BaseModel
    {
        private int _id;
        private string _nome;
        private int _pos;
        private bool _salva;
        private string _val;
        private bool _primary;
        private int _tipo;
        private int _idModello;

        public int Id {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        public string Nome {
            get { return _nome; }
            set
            {
                if (value != _nome)
                {
                    _nome = value;
                    OnPropertyChanged("Nome");
                }
            }
        }
        public int Posizione {
            get { return _pos; }
            set
            {
                if (value != _pos)
                {
                    _pos = value;
                    OnPropertyChanged("Posizione");
                }
            }
        }
        public bool SalvaValori {
            get { return _salva; }
            set
            {
                if (value != _salva)
                {
                    _salva = value;
                    OnPropertyChanged("SalvaValori");
                }
            }
        }
        public string ValorePredefinito {
            get { return _val; }
            set
            {
                if (value != _val)
                {
                    _val = value;
                    OnPropertyChanged("ValorePredefinito");
                }
            }
        }
        public bool IndicePrimario {
            get { return _primary; }
            set
            {
                if (value != _primary)
                {
                    _primary = value;
                    OnPropertyChanged("IndicePrimario");
                }
            }
        }
        public int TipoCampo
        { // Utilizzato per future implementazioni (es textbox[0], combobox[1], checkbox[2])
            get { return _tipo; }
            set
            {
                if (value != _tipo)
                {
                    _tipo = value;
                    OnPropertyChanged("TipoCampo");
                }
            }
        }  
        public int IdModello {
            get { return _idModello; }
            set
            {
                if (value != _idModello)
                {
                    _idModello = value;
                    OnPropertyChanged("IdModello");
                }
            }
        }

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

        public Campo(DBModels.Campo c)
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
