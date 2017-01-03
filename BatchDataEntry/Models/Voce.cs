using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class Voce : BaseModel
    {
        public Voce() { }

        public Voce(int id, string key, string value)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _k;
        public string Key
        {
            get { return _k; }
            set
            {
                _k = value;
                OnPropertyChanged("Key");
            }
        }

        private string _val;
        public string Value
        {
            get { return _val; }
            set { _val = value; OnPropertyChanged("Value"); }
        }

        private ObservableCollection<string> _suggestions;
        public ObservableCollection<string> Suggestions
        {
            get { return _suggestions; }
            set { _suggestions = value; OnPropertyChanged("Suggestions"); }
        }
        
        // Aggiungere sorgente per l'autocompletamento

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", this.Id, this.Key, this.Value);
        }
    }
}
