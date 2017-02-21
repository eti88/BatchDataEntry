using System.Collections.ObjectModel;
using BatchDataEntry.Providers;

namespace BatchDataEntry.Models
{
    public class Voce : BaseModel
    {
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

        private string _focus;
        public string IsFocused
        {
            get
            {
                return _focus;
            }
            set
            {
                if (_focus != value)
                    _focus = value;
                OnPropertyChanged("IsFocused");
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

        public string AUTOCOMPLETETYPE;

        private object _selectedItem;
        public object AutoSelectedItem { get { return _selectedItem; } set { if (value != _selectedItem) { _selectedItem = value;
                    OnPropertyChanged("AutoSelectedItem");
                } } }

        private string _selectedValue;
        public string AutoSelectedValue { get { return _selectedValue; } set { if (value != _selectedValue) { _selectedValue = value; OnPropertyChanged("AutoSelectedValue"); } } }

        private ObservableCollection<Suggestion> _queryProvider;
        public ObservableCollection<Suggestion> QueryProvider
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

        public Voce()
        {
            AUTOCOMPLETETYPE = "NULL";
            IsFocused = "False";
            IsDisabled = false;
        }

        public Voce(int id, string key, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            AUTOCOMPLETETYPE = "NULL";
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(string key, string value, bool enabled = true)
        {
            this.Key = key;
            this.Value = value;
            AUTOCOMPLETETYPE = "NULL";
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, string value, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
            AUTOCOMPLETETYPE = "NULL";
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, bool autocomp, string autoType, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            AUTOCOMPLETETYPE = autoType;
            IsFocused = "False";
            IsDisabled = !enabled;
            QueryProviderSelector(autoType, id);
        }

        public Voce(int id, string key, string valu, bool autocomp, string autoType, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            AUTOCOMPLETETYPE = autoType;
            IsFocused = "False";
            IsDisabled = !enabled;
            this.Value = valu;
            QueryProviderSelector(autoType, id);
        }

        private async void QueryProviderSelector(string tp, int id)
        {
            if (tp.Equals("CSV"))
            {
                QueryProvider = await CsvSuggestionProvider.GetCsvRecords();
            }
            else if (tp.Equals("DB"))
            {
                QueryProvider = await DbSuggestionProvider.GetRecords(id);
            }       
        }

        public override string ToString()
        {
            return string.Format("[Key: {0}, Value: {1}, AutocompleteList: {2}]", this.Id, this.Key, this.Value);
        }

    }
}
