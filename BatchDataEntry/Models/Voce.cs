using System.Collections.Generic;
using System.Linq;
using BatchDataEntry.Providers;
using BatchDataEntry.Helpers;

namespace BatchDataEntry.Models
{
    public class Voce : BaseModel
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

        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key != value)
                    _key = value;
                OnPropertyChanged("Key");
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

        public string AUTOCOMPLETETYPE;

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
            AUTOCOMPLETETYPE = "NULL";
            IsDisabled = false;
        }

        public Voce(int id, string key, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            AUTOCOMPLETETYPE = "NULL";
            IsDisabled = !enabled;
        }

        public Voce(string key, string value, bool enabled = true)
        {
            this.Key = key;
            this.Value = (string.IsNullOrEmpty(value)) ? string.Empty : value;
            AUTOCOMPLETETYPE = "NULL";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, string value, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.Value = (string.IsNullOrEmpty(value)) ? string.Empty : value;
            AUTOCOMPLETETYPE = "NULL";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, bool autocomp, string autoType, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            this.Value = string.Empty;
            AUTOCOMPLETETYPE = autoType;
            IsDisabled = !enabled;
            QueryProviderSelector(autoType, id);
        }

        public Voce(int id, string key, string valu, bool autocomp, string autoType, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            AUTOCOMPLETETYPE = autoType;
            IsDisabled = !enabled;
            this.Value = (string.IsNullOrEmpty(valu)) ? string.Empty : valu;
            QueryProviderSelector(autoType, id);
        }

        private async void QueryProviderSelector(string tp, int id)
        {
            if (tp.Equals("CSV"))
            {
                var csv = new CsvSuggestionProvider();
                QueryProvider = csv.ListOfSuggestions.ToList();
            }
            else if (tp.Equals("DB"))
            { 
                dbQueryProvider = await DbSuggestionProvider.GetRecords(id);
            }       
        }

        public override string ToString()
        {
            return string.Format("[Key: {0}, Value: {1}, AutocompleteList: {2}]", this.Id, this.Key, this.Value);
        }

    }
}
