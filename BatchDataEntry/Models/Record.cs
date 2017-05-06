using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Providers;
using System;
using System.Collections.Generic;

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

        private List<AbsSuggestion> _qprov;
        public List<AbsSuggestion> QueryProvider
        {
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

        public Record() : base() {
            Valore = string.Empty;
            ElementoSelezionato = null;
            QueryProvider = new List<AbsSuggestion>();
            SourceTableColumn = 1;
        }

        public Record(int id, string nome, int posizione, string valorepredef, string tabella, bool primario, bool secondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool disabilitato) : base(id, nome, posizione, valorepredef, tabella, primario, secondario, campo, idmodello, riproponi, disabilitato)
        {
            // TODO: Verificare che effettivamente venga chiamato il costruttore padre
            Valore = string.Empty;
            SourceTableColumn = 1;
            QueryProvider = new List<AbsSuggestion>();
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
        }

        public Record(int id, string nome, string valore, int posizione, string valorepredef, string tabella, bool primario, bool secondario, EnumTypeOfCampo campo, int idmodello, bool riproponi, bool disabilitato) : base(id, nome, posizione, valorepredef, tabella, primario, secondario, campo, idmodello, riproponi, disabilitato)
        {
            // TODO: Verificare che effettivamente venga chiamato il costruttore padre
            Valore = valore;
            SourceTableColumn = 1;
            QueryProvider = new List<AbsSuggestion>();
            ElementoSelezionato = null;
            ElementoSelezionatoValore = string.Empty;
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
            QueryProvider = rec.QueryProvider;
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
            if (TipoCampo == EnumTypeOfCampo.Normale) return;
            if (TipoCampo == EnumTypeOfCampo.AutocompletamentoCsv)
            {
                var csv = new CsvSuggestionProvider();
                QueryProvider = (List<AbsSuggestion>)csv.ListOfSuggestions;
            }
            else if (TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSqlite)
            {
                QueryProvider = await DbSuggestionProvider.GetRecords(Posizione);
            }
            else if (TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSql)
            {
                if (string.IsNullOrWhiteSpace(TabellaSorgente) || SourceTableColumn < 1)
                    throw new Exception("QueryProviderSelector mancano argomenti");
                QueryProvider = await DbSqlSuggestionProvider.GetRecords(this.Posizione, TabellaSorgente, SourceTableColumn);
            }
        }
    }
}
