using BatchDataEntry.Helpers;
using System;

namespace BatchDataEntry.Models
{

    public class Campo : BaseModel
    {   
        private int _id;
        public int Id
        {
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

        private string _nome;   
        public string Nome
        {
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

        private int _pos;
        public int Posizione
        {
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

        private bool _salva;
        public bool SalvaValori
        {
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

        private string _predefval;
        public string ValorePredefinito
        {
            get { return _predefval; }
            set
            {
                if (value != _predefval)
                {
                    _predefval = value;
                    OnPropertyChanged("ValorePredefinito");
                }
            }
        }

        private bool _primary;
        public bool IndicePrimario
        {
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

        private bool _secondary;
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

        private EnumTypeOfCampo _tipo;
        public EnumTypeOfCampo TipoCampo
        { 
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

        private int _idModello;
        public int IdModello
        {
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

        private bool _riproponi;
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

        private bool _isDisabled;
        public bool IsDisabilitato
        {
            get { return _isDisabled; }
            set
            {
                if (value != _isDisabled)
                {
                    _isDisabled = value;
                    OnPropertyChanged("IsDisabilitato");
                }
            }
        }

        private string _source;
        public string TabellaSorgente {
            get { return _source; }
            set { if (value != _source)
                {
                    _source = value;
                    OnPropertyChanged("TabellaSorgente");
                } }
        }

        private int _sourceTableColumn;
        public int SourceTableColumn
        {
            get { return _sourceTableColumn; }
            set
            {
                if (value != _sourceTableColumn)
                {
                    _sourceTableColumn = value;
                    OnPropertyChanged("SourceTableColumn");
                }
            }
        }

        public Campo(){
            Id = 0;
            Nome = string.Empty;
            Posizione = -1;
            ValorePredefinito = string.Empty;
            TabellaSorgente = string.Empty;
            IndicePrimario = false;
            IndiceSecondario = false;
            TipoCampo = EnumTypeOfCampo.Normale;
            IdModello = 0;
            Riproponi = false;
            IsDisabilitato = false;
            SourceTableColumn = 0;
        }

        public Campo(int id, string nome, int posizione, string valorepredef, string tabella, bool primario, bool secondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool disabilitato) {
            Id = id;
            Nome = nome;
            Posizione = posizione;
            ValorePredefinito = valorepredef;
            TabellaSorgente = tabella;
            IndicePrimario = primario;
            IndiceSecondario = secondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = disabilitato;
            SourceTableColumn = 0;
        }

        public Campo(Campo campo) {
            Id = campo.Id;
            Nome = campo.Nome;
            Posizione = campo.Posizione;
            ValorePredefinito = campo.ValorePredefinito;
            TabellaSorgente = campo.TabellaSorgente;
            SourceTableColumn = campo.SourceTableColumn;
            IndicePrimario = campo.IndicePrimario;
            IndiceSecondario = campo.IndiceSecondario;
            TipoCampo = campo.TipoCampo;
            IdModello = campo.IdModello;
            Riproponi = campo.Riproponi;
            IsDisabilitato = campo.IsDisabilitato;
        }

        public void SetConfigBasedOnType()
        {
            if (this.TipoCampo == EnumTypeOfCampo.Normale)
                this.SalvaValori = false;
            else
                this.SalvaValori = true;
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
            if (this.TabellaSorgente != campo.TabellaSorgente) return false;
            if (this.TipoCampo != campo.TipoCampo) return false;
            if (this.IdModello != campo.IdModello) return false;
            if(this.Riproponi != campo.Riproponi) return false;
            if (this.IsDisabilitato != campo.IsDisabilitato) return false;
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
            resutl = (resutl * 7) + ((string.IsNullOrEmpty(this.TabellaSorgente)) ? 0 : this.TabellaSorgente.GetHashCode());
            resutl = (resutl * 7) + this.Riproponi.GetHashCode();
            resutl = (resutl * 7) + this.IsDisabilitato.GetHashCode();
            return resutl;
        }

        public override string ToString()
        {
            return String.Format("{this.Id},{this.TipoCampo},{this.Nome},{this.Valore}),{this.SalvaValori}");
        }
    }
}
