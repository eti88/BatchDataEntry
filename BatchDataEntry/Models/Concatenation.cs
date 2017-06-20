using Newtonsoft.Json;
using System.Collections.Generic;

namespace BatchDataEntry.Models
{
    public class Concatenation: BaseModel
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
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
                if (_nome != value)
                {
                    _nome = value;
                    OnPropertyChanged("Nome");
                }
            }
        }

        private int _modello;
        public int Modello
        {
            get { return _modello; }
            set
            {
                if (_modello != value)
                {
                    _modello = value;
                    OnPropertyChanged("Modello");
                }
            }
        }

        private Dictionary<string, object> _campisel;
        public Dictionary<string, object> CampiSelezionati
        {
            get { return _campisel; }
            set
            {
                if (_campisel != value)
                {
                    _campisel = value;
                    OnPropertyChanged("CampiSelezionati");
                }
            }
        }

        public Concatenation()
        {
            this.CampiSelezionati = new Dictionary<string, object>();
        }

        public Concatenation(int id, string nome, int modello)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.CampiSelezionati = new Dictionary<string, object>();
        }

        public Concatenation(int id, string nome, int modello, Dictionary<string, object> campi)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.CampiSelezionati = new Dictionary<string, object>();
        }

        public Concatenation(Concatenation c)
        {
            this.Id = c.Id;
            this.Nome = c.Nome;
            this.Modello = c.Modello;
            this.CampiSelezionati = c.CampiSelezionati;
        }

        public string SerializeDictionary()
        {
            return JsonConvert.SerializeObject(this.CampiSelezionati);
        }

        public void DeserializeDictionary(string obj)
        {
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);
            if (tmp != null) CampiSelezionati = tmp;
        }
    }
}
