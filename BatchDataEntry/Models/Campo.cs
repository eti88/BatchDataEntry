using System;

namespace BatchDataEntry.Models
{

    public class Campo : BaseModel
    {
        private MementoCampo MyMemento;
        
        private int _id;
        private string _nome;
        private int _pos;
        private bool _salva;
        private string _val;
        private bool _primary;
        private bool _secondary;
        private int _tipo;
        private int _idModello;
        private bool _riproponi;
        private bool _isDisabled;

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

        public bool IndiceSecondario
        {
            get { return _secondary; }
            set
            {
                if (value != _secondary)
                {
                    _secondary = value;
                    OnPropertyChanged("IndiceSecondario");
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
        public bool Riproponi
        {
            get { return _riproponi; }
            set
            {
                if (value != _riproponi)
                {
                    _riproponi = value;
                    OnPropertyChanged("Riproponi");
                }
            }
        }

        public bool IsDisabled
        {
            get { return _isDisabled; }
            set {
                if (value != IsDisabled)
                {
                    _isDisabled = value;
                    OnPropertyChanged("IsDisabled");
                } }
        }

        /*
         SalvaValori permette di tenere traccia dei dati nel medesimo campo velocizzando
         futuri inserimenti.
         */

        public Campo() { }

        public Campo(string nome, bool sv, string vp, bool ip)
        {
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.IndiceSecondario = false;
            this.TipoCampo = 0;
            this.Riproponi = false;
            this.MyMemento = new MementoCampo(nome, this.Posizione, sv, vp, ip, this.IndiceSecondario);
        }

        public Campo(string nome, bool sv, string vp, bool ip, bool rip, bool disab)
        {
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            this.Riproponi = rip;
            this.IsDisabled = disab;
            this.MyMemento = new MementoCampo(nome, this.Posizione, sv, vp, ip, this.IndiceSecondario);
        }

        public Campo(int id, string nome, bool sv, string vp, bool ip, bool rip, bool disab)
        {
            this.Id = id;
            this.Nome = nome;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            this.Riproponi = rip;
            this.IsDisabled = disab;
            this.MyMemento = new MementoCampo(nome, this.Posizione, sv, vp, ip, this.IndiceSecondario);
        }

        public Campo(int id, string nome, int po, bool sv, string vp, bool ip, bool sc, bool disab, int fk)
        {
            this.Id = id;
            this.Nome = nome;
            this.Posizione = po;
            this.SalvaValori = sv;
            this.ValorePredefinito = vp;
            this.IndicePrimario = ip;
            this.TipoCampo = 0;
            this.IdModello = fk;
            this.IsDisabled = disab;
            this.IndiceSecondario = sc;
            this.MyMemento = new MementoCampo(nome, po, sv, vp, ip, sc);
        }

        public Campo(Campo _campo)
        {
            if(_campo == null) return;
            this.Id = _campo.Id;
            this.Nome = _campo.Nome;
            this.Posizione = _campo.Posizione;
            this.SalvaValori = _campo.SalvaValori;
            this.ValorePredefinito = _campo.ValorePredefinito;
            this.IndicePrimario = _campo.IndicePrimario;
            this.IndiceSecondario = _campo.IndiceSecondario;
            this.TipoCampo = _campo.TipoCampo;
            this.IdModello = _campo.IdModello;
            this.Riproponi = _campo.Riproponi;
            this.IsDisabled = _campo.IsDisabled;
            this.MyMemento = new MementoCampo(_campo.Nome, _campo.Posizione, _campo.SalvaValori, _campo.ValorePredefinito, _campo.IndicePrimario, _campo.IndiceSecondario);
        }

        public void Revert()
        {
            this.Nome = this.MyMemento.nome;
            this.Posizione = this.MyMemento.posizione;
            this.SalvaValori = this.MyMemento.salvaValori;
            this.ValorePredefinito = this.MyMemento.valPredefinito;
            this.IndicePrimario = this.MyMemento.isPrimary;
            this.IndiceSecondario = this.MyMemento.isSecondary;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            Campo campo = obj as Campo;

            if (this.Id != campo.Id) return false;
            if (this.Nome != campo.Nome) return false;
            if (this.Posizione != campo.Posizione) return false;
            if (this.SalvaValori != campo.SalvaValori) return false;
            if (this.ValorePredefinito != campo.ValorePredefinito) return false;
            if (this.IndiceSecondario != campo.IndiceSecondario) return false;
            if (this.IndicePrimario != campo.IndicePrimario) return false;
            if (this.TipoCampo != campo.TipoCampo) return false;
            if (this.IdModello != campo.IdModello) return false;
            if(this.Riproponi != campo.Riproponi) return false;
            if (this.IsDisabled != campo.IsDisabled) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int resutl = 13;
            resutl = (resutl *7) + this.Id.GetHashCode();
            resutl = (resutl * 7) + ((string.IsNullOrEmpty(this.Nome)) ? 0 : this.Nome.GetHashCode());
            resutl = (resutl * 7) + this.Posizione.GetHashCode();
            resutl = (resutl * 7) + this.SalvaValori.GetHashCode();
            resutl = (resutl * 7) + ((string.IsNullOrEmpty(this.ValorePredefinito) ? 0 : this.ValorePredefinito.GetHashCode()));
            resutl = (resutl * 7) + this.IndicePrimario.GetHashCode();
            resutl = (resutl * 7) + this.IndiceSecondario.GetHashCode();
            resutl = (resutl * 7) + this.TipoCampo.GetHashCode();
            resutl = (resutl * 7) + this.IdModello.GetHashCode();
            resutl = (resutl * 7) + this.Riproponi.GetHashCode();
            resutl = (resutl * 7) + this.IsDisabled.GetHashCode();
            return resutl;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}]", this.Id, this.Nome,
                this.Posizione, this.SalvaValori, this.ValorePredefinito, this.IndicePrimario, this.TipoCampo,
                this.IdModello);
        }
    }

    public class MementoCampo
    {
        // Applico il pattern memento solo alle proprietà che possono effettivamente essere modificate dall'ui
        public readonly string nome;
        public readonly int posizione;
        public readonly bool salvaValori;
        public readonly string valPredefinito;
        public readonly bool isPrimary;
        public readonly bool isSecondary;

        public MementoCampo(string _nome, int _pos, bool _salva, string _valp, bool _isp, bool sc)
        {
            this.nome = _nome;
            this.posizione = _pos;
            this.salvaValori = _salva;
            this.valPredefinito = _valp;
            this.isPrimary = _isp;
            this.isSecondary = sc;
        }
    }
}
