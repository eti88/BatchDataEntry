using System;
using System.Collections.Generic;
using BatchDataEntry.Helpers;

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

        public Voce()
        {
            Suggestions = new List<string>();
            IsFocused = "False";
            IsDisabled = false;
        }

        public Voce(int id, string key, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            Suggestions = new List<string>();
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(string key, string value, bool enabled = true)
        {
            this.Key = key;
            this.Value = value;
            Suggestions = new List<string>();
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, string value, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
            Suggestions = new List<string>();
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, bool autocomp, DatabaseHelper db, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            if (this.IsAutocomplete)
            {
                try
                {
                    if (!string.IsNullOrEmpty(key) && db != null)
                    {
                        var lst = new List<string>();
                        lst = db.GetAutocompleteList(id);
                        if (lst != null)
                            Suggestions = lst;
                        else
                            Suggestions = new List<string>();
                    }
                    else
                        Suggestions = new List<string>();
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Console.WriteLine(@"[VOCEEXCEPTION]" + e.ToString());
                    #endif
                    this.IsAutocomplete = false;
                    Suggestions = new List<string>();
                }
            }
            IsFocused = "False";
            IsDisabled = !enabled;
        }

        public Voce(int id, string key, string valu, bool autocomp, DatabaseHelper db, bool enabled = true)
        {
            this.Id = id;
            this.Key = key;
            this.IsAutocomplete = autocomp;
            if (this.IsAutocomplete)
            {
                try
                {
                    if (!string.IsNullOrEmpty(key) && db != null)
                    {
                        var lst = new List<string>();
                        lst = db.GetAutocompleteList(id);
                        if (lst != null)
                            Suggestions = lst;
                        else
                            Suggestions = new List<string>();
                    }
                    else
                        Suggestions = new List<string>();
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Console.WriteLine(@"[VOCEEXCEPTION]" + e.ToString());
                    #endif
                    this.IsAutocomplete = false;
                    Suggestions = new List<string>();
                }
            }
            IsFocused = "False";
            IsDisabled = !enabled;
            this.Value = valu;
        }

        public override string ToString()
        {
            return string.Format("[Key: {0}, Value: {1}, AutocompleteList: {2}]", this.Id, this.Key, this.Value);
        }
    }
}
