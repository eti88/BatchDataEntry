using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Providers;
using System;

namespace BatchDataEntry.Models
{
    public class Record: Campo
    {
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

        private AbsSuggestion _selected;
        public AbsSuggestion ElementoSelezionato
        {
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
        public string ElementoSelezionatoValore
        {
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

        private AbsDbSuggestions _suggestionsProvider;
        public AbsDbSuggestions SuggestionsProvider
        {
            get { return _suggestionsProvider; }
            set
            {
                if (value != _suggestionsProvider)
                {
                    _suggestionsProvider = value;
                    OnPropertyChanged("SuggestionsProvider");
                }
            }
        }

        public Record() : base() {
            Valore = string.Empty;
            ElementoSelezionato = null;
            SourceTableColumn = 1;
            SuggestionsProvider = null;
        }

        public Record(int id, string nome, int posizione, string valorepredef, string tabella, bool primario, bool secondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool disabilitato) : base(id, nome, posizione, valorepredef, tabella, primario, secondario, campo, idmodello, riproponi, disabilitato)
        {
            Valore = string.Empty;
            SourceTableColumn = 1;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            SuggestionsProvider = null;
        }

        public Record(int id, string nome, string valore, int posizione, string valorepredef, string tabella, bool primario, bool secondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool disabilitato) : base(id, nome, posizione, valorepredef, tabella, primario, secondario, campo, idmodello, riproponi, disabilitato)
        {
            Valore = valore;
            SourceTableColumn = 1;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
            SuggestionsProvider = null;
        }

        public Record(Record rec)
        {
            Id = rec.Id;
            Nome = rec.Nome;
            Posizione = rec.Posizione;
            ValorePredefinito = rec.ValorePredefinito;
            TabellaSorgente = rec.TabellaSorgente;
            IndicePrimario = rec.IndicePrimario;
            IndiceSecondario = rec.IndiceSecondario;
            TipoCampo = rec.TipoCampo;
            IdModello = rec.IdModello;
            Riproponi = rec.Riproponi;
            IsDisabilitato = rec.IsDisabilitato;
            Valore = rec.Valore;
            SourceTableColumn = rec.SourceTableColumn;
            SuggestionsProvider = null;
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
        }

        /// <summary>
        /// Metodo usato per generare i record partendo dall'oggetto campo
        /// </summary>
        /// <param name="c">Campo da usare per generare il record</param>
        /// <param name="pos">posizione del record da 0 a x</param>
        /// <returns>Record precompilato</returns>
        public static Record Create(Campo c, int pos)
        {
            var rec = new Record();
            if(c == null) throw new Exception("Campo non definito nel modello");

            rec.Id = rec.Posizione = pos;
            rec.Nome = c.Nome;
            // Imposta il tipo di campo
            rec.TipoCampo = c.TipoCampo;
            rec.SetConfigBasedOnType();
            if (!string.IsNullOrEmpty(c.ValorePredefinito))
                rec.Valore = c.ValorePredefinito;
            else
                rec.Valore = string.Empty;

            rec.ValorePredefinito = c.ValorePredefinito;
            rec.IndicePrimario = c.IndicePrimario;
            rec.IndiceSecondario = c.IndiceSecondario;
            rec.IdModello = c.IdModello;
            rec.IsDisabilitato = c.IsDisabilitato;
            rec.TabellaSorgente = c.TabellaSorgente;
            if (c.SourceTableColumn < 1) c.SourceTableColumn = 1;
            rec.SourceTableColumn = c.SourceTableColumn;
            if(rec.SalvaValori)
                rec.QueryProviderSelector();
            return rec;
        }

        /// <summary>
        /// Metodo usato per generare i record partendo dall'oggetto campo
        /// </summary>
        /// <param name="c">Campo da usare per generare il record</param>
        /// <param name="pos">posizione del record da 0 a x</param>
        /// <param name="valu">Valore assegnato al record</param>
        /// <returns>Record precompilato con valore</returns>
        public static Record Create(Campo c, int pos, string valu)
        {
            var rec = new Record();
            if (c == null) throw new Exception("Campo non definito nel modello");

            rec.Id = rec.Posizione = pos;
            rec.Nome = c.Nome;
            // Imposta il tipo di campo
            rec.TipoCampo = c.TipoCampo;
            rec.SetConfigBasedOnType();
            if (!string.IsNullOrEmpty(valu))
                rec.Valore = valu;
            else if (!string.IsNullOrEmpty(c.ValorePredefinito))
                rec.Valore = c.ValorePredefinito;
            else
                rec.Valore = string.Empty;

            rec.ValorePredefinito = c.ValorePredefinito;
            rec.IndicePrimario = c.IndicePrimario;
            rec.IndiceSecondario = c.IndiceSecondario;
            rec.IdModello = c.IdModello;
            rec.IsDisabilitato = c.IsDisabilitato;
            rec.TabellaSorgente = c.TabellaSorgente;
            if (c.SourceTableColumn < 1) c.SourceTableColumn = 1;
            rec.SourceTableColumn = c.SourceTableColumn;
            rec.QueryProviderSelector();
            return rec;
        }

        public async void QueryProviderSelector()
        {
            if (TipoCampo == EnumTypeOfCampo.Normale || TipoCampo == EnumTypeOfCampo.AutocompletamentoCsv) return;
            if (TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSqlite)
            {
                var au = new DbSuggestionProvider(this.Posizione);
                SuggestionsProvider = au;
            }
            else if (TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSql)
            {
                if (string.IsNullOrWhiteSpace(TabellaSorgente) || SourceTableColumn < 1)
                    throw new Exception("SuggestionsProvider mancano argomenti");
                var au = new DbSqlSuggestionProvider(this.Posizione, TabellaSorgente, SourceTableColumn);
                SuggestionsProvider = au;
            }
        }
    }
}
