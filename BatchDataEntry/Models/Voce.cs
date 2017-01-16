using System.Collections.Generic;

namespace BatchDataEntry.Models
{
    public class Voce : BaseModel
    {
        public Voce() { }

        public Voce(int id, string key)
        {
            this.Id = id;
            this.Key = key;
        }

        public Voce(int id, string key, string value)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
        }

        public Voce(int id, string key, bool autocomp)
        {
            this.Id = id;
            this.Key = key;
            if (autocomp)
            {
                // recupera la lista dei valori
            }
        }

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
        public string Key {
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
        public string Value {
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

        private IEnumerable<string> _suggestion;
        public IEnumerable<string> Suggestions
        {
            get { return _suggestion; }
            set
            {
                if (_suggestion != value && IsAutocomplete)
                    _suggestion = value;
                OnPropertyChanged("Suggestions");
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", this.Id, this.Key, this.Value);
        }
    }
}
