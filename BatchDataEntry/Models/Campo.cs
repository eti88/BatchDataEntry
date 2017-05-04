using BatchDataEntry.Helpers;
using BatchDataEntry.Interfaces;
using System;
using System.Collections.Generic;

namespace BatchDataEntry.Models
{

    public class Campo : BaseModel, ICampo
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

        private string _value;
        public string Valore
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                    _value = value;
                OnPropertyChanged("Valore");
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

        private string _sourceTableAutocomplete;
        public string SourceTableAutocomplete
        {
            get { return _sourceTableAutocomplete; }
            set
            {
                if (value != _sourceTableAutocomplete)
                {
                    _sourceTableAutocomplete = value;
                    OnPropertyChanged("SourceTableAutocomplete");
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

        private ISuggestion _selected;
        public ISuggestion ElementoSelezionato {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnPropertyChanged("ElementoSelezionato");
                }
            }
        }

        private string _selectedValue;
        public string ElementoSelezionatoValore {
            get { return _selectedValue; }
            set
            {
                if (value != _selectedValue)
                {
                    _selectedValue = value;
                    OnPropertyChanged("ElementoSelezionatoValore");
                }
            }
        }

        private List<ISuggestion> _qprov;
        public List<ISuggestion> QueryProvider {
            get { return _qprov; }
            set
            {
                if (value != _qprov)
                {
                    _qprov = value;
                    OnPropertyChanged("QueryProvider");
                }
            }
        }

        private int _sourceTableColumn { get; set; }
        /*
         SalvaValori permette di tenere traccia dei dati nel medesimo campo velocizzando
         futuri inserimenti.
         */

        public Campo() {
            Id = 0;
            Nome = string.Empty;
            Valore = string.Empty;
            Posizione = -1;
            ValorePredefinito = string.Empty;
            TabellaSorgente = string.Empty;
            IndicePrimario = false;
            IndiceSecondario = false;
            TipoCampo = EnumTypeOfCampo.Normale;
            IdModello = 0;
            Riproponi = false;
            IsDisabilitato = false;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        /// <summary>
        /// Costruttore per il campo concepito per il tipo Normale
        /// senza passare il valore dell'id
        /// </summary>
        /// <param name="nome">Nome del campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        public Campo(string nome, int posizione, string valorePredefinito, bool isIndicePrimario, 
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool isDisabilitato) {
            Id = 0;
            Nome = nome;
            Valore = string.Empty;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = string.Empty;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        /// <summary>
        /// Costruttore per il campo concepito per il tipo Normale e Sqlite
        /// </summary>
        /// <param name="id">Id del campo</param>
        /// <param name="nome">Nome del campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        public Campo(int id, string nome, int posizione, string valorePredefinito, bool isIndicePrimario,
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi,
            bool isDisabilitato)
        {
            Id = id;
            Nome = nome;
            Valore = string.Empty;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = string.Empty;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        /// <summary>
        /// Costruttore per campo Normale e Sqlite con Valore
        /// </summary>
        /// /// <param name="id">Id del campo</param>
        /// <param name="nome">Nome del campo</param>
        /// <param name="val">Valore associato al campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        public Campo(int id, string nome, string val, int posizione, string valorePredefinito, bool isIndicePrimario,
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi,
            bool isDisabilitato)
        {
            Id = id;
            Nome = nome;
            Valore = val;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = string.Empty;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        /// <summary>
        /// Costruttore per campo Db Sql (senza id)
        /// </summary>
        /// <param name="nome">Nome del campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        /// <param name="sourcetab">Tabella di riferimento per il database</param>
        /// <param name="sourcetabcol">Indica la colonna della sourcetab da proporre come autocompletamento</param>
        public Campo(string nome, int posizione, string valorePredefinito, bool isIndicePrimario,
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi,
            bool isDisabilitato, string sourcetab, int sourcetabcol = 1)
        {
            Id = 0;
            Nome = nome;
            Valore = string.Empty;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = sourcetab;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            _sourceTableColumn = sourcetabcol;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }
 
        /// <summary>
        /// Costruttore per campo Db Sql
        /// </summary>
        /// <param name="id">Id del campo</param>
        /// <param name="nome">Nome del campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        /// <param name="sourcetab">Tabella di riferimento per il database</param>
        /// <param name="sourcetabcol">Indica la colonna della sourcetab da proporre come autocompletamento</param>
        public Campo(int id, string nome, int posizione, string valorePredefinito, bool isIndicePrimario,
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi,
            bool isDisabilitato, string sourcetab, int sourcetabcol = 1)
        {
            Id = id;
            Nome = nome;
            Valore = string.Empty;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = sourcetab;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            _sourceTableColumn = sourcetabcol;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        /// <summary>
        /// Costruttore per campo Db Sql con Valore
        /// </summary>
        /// <param name="id">Id del campo</param>
        /// <param name="nome">Nome del campo</param>
        /// <param name="val">Valore associato al campo</param>
        /// <param name="posizione">Posizione del campo</param>
        /// <param name="valorePredefinito">Valore predefinito del campo</param>
        /// <param name="isIndicePrimario">Imposta il campo come indice primario (unico)</param>
        /// <param name="isIndiceSecondario">Imposta il campo come indice secondario (unico)</param>
        /// <param name="campo">Imposta l'enumeratore della tipologia del campo</param>
        /// <param name="idmodello">Imposta l'id del modello a cui associare il campo</param>
        /// <param name="riproponi">Permette di riproporre l'ultimo valore inserito in questo campo per l'inserimento successivo</param>
        /// <param name="isDisabilitato">Abilita o disabilita il campo</param>
        /// <param name="sourcetab">Tabella di riferimento per il database</param>
        /// <param name="sourcetabcol">Indica la colonna della sourcetab da proporre come autocompletamento</param>
        public Campo(int id, string nome, string val, int posizione, string valorePredefinito, bool isIndicePrimario,
            bool isIndiceSecondario, EnumTypeOfCampo campo, int idmodello, bool riproponi,
            bool isDisabilitato, string sourcetab, int sourcetabcol = 1)
        {
            Id = id;
            Nome = nome;
            Valore = val;
            Posizione = posizione;
            ValorePredefinito = valorePredefinito;
            TabellaSorgente = sourcetab;
            IndicePrimario = isIndicePrimario;
            IndiceSecondario = isIndiceSecondario;
            TipoCampo = campo;
            IdModello = idmodello;
            Riproponi = riproponi;
            IsDisabilitato = isDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            _sourceTableColumn = sourcetabcol;
            QueryProvider = new List<ISuggestion>();
            SetConfigBasedOnType();
        }

        public Campo(Campo campo) {
            Id = campo.Id;
            Nome = campo.Nome;
            Valore = campo.Valore;
            Posizione = campo.Posizione;
            ValorePredefinito = campo.ValorePredefinito;
            TabellaSorgente = campo.TabellaSorgente;
            IndicePrimario = campo.IndicePrimario;
            IndiceSecondario = campo.IndiceSecondario;
            TipoCampo = campo.TipoCampo;
            IdModello = campo.IdModello;
            Riproponi = campo.Riproponi;
            IsDisabilitato = campo.IsDisabilitato;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            _sourceTableColumn = campo._sourceTableColumn;
            QueryProvider = campo.QueryProvider;
        }

        //public Campo() {
        //    Id = 0;
        //    Nome = string.Empty;
        //    Posizione = 0;
        //    SalvaValori = false;
        //    ValorePredefinito = string.Empty;
        //    IndicePrimario = false;
        //    IndiceSecondario = false;
        //    TipoCampo = 0;
        //    IdModello = -1;
        //    Riproponi = false;
        //    IsDisabled = false;
        //    SourceTableAutocomplete = string.Empty;
        //}

        //public Campo(string nome, bool sv, string vp, bool ip)
        //{
        //    this.Nome = nome;
        //    this.SalvaValori = sv;
        //    this.ValorePredefinito = vp;
        //    this.IndicePrimario = ip;
        //    this.IndiceSecondario = false;
        //    this.TipoCampo = 0;
        //    this.Riproponi = false;
        //    SourceTableAutocomplete = string.Empty;
        //}

        //public Campo(string nome, bool sv, string vp, bool ip, bool rip, bool disab)
        //{
        //    this.Nome = nome;
        //    this.SalvaValori = sv;
        //    this.ValorePredefinito = vp;
        //    this.IndicePrimario = ip;
        //    this.TipoCampo = 0;
        //    this.Riproponi = rip;
        //    this.IsDisabled = disab;
        //    SourceTableAutocomplete = string.Empty;
        //}

        //public Campo(int id, string nome, bool sv, string vp, bool ip, bool rip, bool disab)
        //{
        //    this.Id = id;
        //    this.Nome = nome;
        //    this.SalvaValori = sv;
        //    this.ValorePredefinito = vp;
        //    this.IndicePrimario = ip;
        //    this.TipoCampo = 0;
        //    this.Riproponi = rip;
        //    this.IsDisabled = disab;
        //    SourceTableAutocomplete = string.Empty;
        //}

        //public Campo(int id, string nome, int po, bool sv, string vp, bool ip, bool sc, bool disab, int fk)
        //{
        //    this.Id = id;
        //    this.Nome = nome;
        //    this.Posizione = po;
        //    this.SalvaValori = sv;
        //    this.ValorePredefinito = vp;
        //    this.IndicePrimario = ip;
        //    this.TipoCampo = 0;
        //    this.IdModello = fk;
        //    this.IsDisabled = disab;
        //    this.IndiceSecondario = sc;
        //    SourceTableAutocomplete = string.Empty;
        //}

        //public Campo(Campo _campo)
        //{
        //    if(_campo == null) return;
        //    this.Id = _campo.Id;
        //    this.Nome = _campo.Nome;
        //    this.Posizione = _campo.Posizione;
        //    this.SalvaValori = _campo.SalvaValori;
        //    this.ValorePredefinito = _campo.ValorePredefinito;
        //    this.IndicePrimario = _campo.IndicePrimario;
        //    this.IndiceSecondario = _campo.IndiceSecondario;
        //    this.TipoCampo = _campo.TipoCampo;
        //    this.IdModello = _campo.IdModello;
        //    this.Riproponi = _campo.Riproponi;
        //    this.IsDisabled = _campo.IsDisabled;
        //    this.SourceTableAutocomplete = _campo.SourceTableAutocomplete;
        //}

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
            resutl = (resutl * 7) + this.TabellaSorgente.GetHashCode();
            resutl = (resutl * 7) + this.Riproponi.GetHashCode();
            resutl = (resutl * 7) + this.IsDisabilitato.GetHashCode();
            return resutl;
        }

        public override string ToString()
        {
            return String.Format("{this.Id},{this.TipoCampo},{this.Nome},{this.Valore}),{this.SalvaValori}");
        }

        public void QueryProviderSelector()
        {
            throw new NotImplementedException();
        }
    }
}
