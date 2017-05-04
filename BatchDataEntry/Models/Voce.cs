using System.Collections.Generic;
using System.Linq;
using BatchDataEntry.Providers;
using BatchDataEntry.Helpers;
using BatchDataEntry.Interfaces;
using System;

namespace BatchDataEntry.Models
{
    public class Voce : BaseModel, ICampo
    {
#region Attr
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                    _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _nome;
        public string Nome
        {
            get
            {
                return _nome;
            }
            set
            {
                if (_nome != value)
                    _nome = value;
                OnPropertyChanged("Nome");
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                    _value = value;
                OnPropertyChanged("Value");
            }
        }

        private bool _isAutocomplete;
        public bool IsAutocomplete
        {
            get { return _isAutocomplete; }
            set
            {
                if (_isAutocomplete != value)
                    _isAutocomplete = value;
                OnPropertyChanged("IsAutocomplete");
            }
        }

        private bool _isDisabled;
        public bool IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                if (value != IsDisabled)
                {
                    _isDisabled = value;
                    OnPropertyChanged("IsDisabled");
                }
            }
        }

        private EnumTypeOfCampo _voiceType;
        public EnumTypeOfCampo VoiceType
        {
            get { return _voiceType; }
            set
            {
                if (value != VoiceType)
                {
                    _voiceType = value;
                    OnPropertyChanged("VoiceType");
                }
            }
        }

        private Suggestion _selectedItem;
        public Suggestion AutoSelectedItem { get { return _selectedItem; } set { if (value != _selectedItem) { _selectedItem = value;
                    OnPropertyChanged("AutoSelectedItem");
                } } }

        private string _selectedValue;
        public string AutoSelectedValue { get { return _selectedValue; } set { if (value != _selectedValue) { _selectedValue = value; OnPropertyChanged("AutoSelectedValue"); } } }

        private List<string> _dbqueryProvider;
        public List<string> dbQueryProvider
        {
            get { return _dbqueryProvider; }
            private set
            {
                if (value != _dbqueryProvider)
                {
                    _dbqueryProvider = value;
                    OnPropertyChanged("dbQueryProvider");
                }
            }
        }

        private List<Suggestion> _queryProvider;
        public List<Suggestion> QueryProvider
        {
            get { return _queryProvider; }
            private set
            {
                if (value != _queryProvider)
                {
                    _queryProvider = value;
                    OnPropertyChanged("QueryProvider");
                }
            }
        }

        
        
        

        #endregion

        public Voce()
        {
            VoiceType = EnumTypeOfCampo.Normale;
            IsDisabled = false;
        }

        public Voce(int id, string key, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            VoiceType = EnumTypeOfCampo.Normale;
            IsDisabled = !enabled;
        }

        public Voce(string key, string value, bool enabled = true)
        {
            this.Key = key;
            this.Value = (string.IsNullOrEmpty(value)) ? string.Empty : value;
            VoiceType = EnumTypeOfCampo.Normale;
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, string value, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.Value = (string.IsNullOrEmpty(value)) ? string.Empty : value;
            VoiceType = EnumTypeOfCampo.Normale;
            IsDisabled = !enabled;
        }

        // Usato per l'inizializzazione del csv 
        public Voce(int id, string key, bool autocomp, EnumTypeOfCampo autoType, bool enabled)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            this.Value = string.Empty;
            VoiceType = autoType;
            IsDisabled = !enabled;
            QueryProviderSelector(autoType);
        }

        // Usato per l'inizializzazione del csv versione con Valore
        public Voce(int id, string key, string value, bool autocomp, EnumTypeOfCampo autoType, bool enabled)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
            this.IsAutocomplete = autocomp;
            this.Value = string.Empty;
            VoiceType = autoType;
            IsDisabled = !enabled;
            QueryProviderSelector(autoType);
        }

        public Voce(int id, string key, bool autocomp, EnumTypeOfCampo autoType, bool enabled, string table, int tabcol)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            this.Value = string.Empty;
            VoiceType = autoType;
            IsDisabled = !enabled;
            QueryProviderSelector(autoType, id, table, tabcol);
        }

        public Voce(int id, string key, string valu, bool autocomp, EnumTypeOfCampo autoType, bool enabled, string table, int tabcol)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            VoiceType = autoType;
            IsDisabled = !enabled;
            this.Value = (string.IsNullOrEmpty(valu)) ? string.Empty : valu;
            QueryProviderSelector(autoType, id, table, tabcol);
        }

        private async void QueryProviderSelector(EnumTypeOfCampo tp)
        {
            if (tp == EnumTypeOfCampo.AutocompletamentoCsv)
            {
                var csv = new CsvSuggestionProvider();
                QueryProvider = csv.ListOfSuggestions.ToList();
            }
        }

        private async void QueryProviderSelector(EnumTypeOfCampo tp, int id)
        {
            if (tp == EnumTypeOfCampo.AutocompletamentoDbSqlite)
            {
                dbQueryProvider = await DbSuggestionProvider.GetRecords(id);
            }
        }

        // Per il momento utilizziamo sempre la prima colonna Nel caso basta aggiungere un campo nella definizione della nuova colonna per la selezione specifica
        private async void QueryProviderSelector(EnumTypeOfCampo tp, int id, string table, int tableColumn = 1)
        {
            if (tp == EnumTypeOfCampo.AutocompletamentoDbSql)
            {
                if (string.IsNullOrWhiteSpace(table) || tableColumn < 1) return;
                dbQueryProvider = await DbSqlSuggestionProvider.GetRecords(id, table, tableColumn);
            }
        }

        public override string ToString()
        {
            return string.Format("[Key: {0}, Value: {1}, AutocompleteList: {2}]", this.Id, this.Key, this.Value);
        }

        public void QueryProviderSelector()
        {
            throw new NotImplementedException();
        }
    }
}
