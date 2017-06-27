using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BatchDataEntry.Models
{
    public class Concatenation: BaseModel
    {
        public int START_POS;
        public int END_POS;

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
                    FindStartEnd();
                }
            }
        }

        public List<int> Positions { get; set; }

        public Concatenation()
        {
            this.CampiSelezionati = new Dictionary<string, object>();
            Positions = new List<int>();
        }

        public Concatenation(int id, string nome, int modello)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.CampiSelezionati = new Dictionary<string, object>();
            Positions = new List<int>();
        }

        public Concatenation(int id, string nome, int modello, Dictionary<string, object> campi)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.CampiSelezionati = new Dictionary<string, object>();
            Positions = new List<int>();
        }

        public Concatenation(Concatenation c)
        {
            this.Id = c.Id;
            this.Nome = c.Nome;
            this.Modello = c.Modello;
            this.CampiSelezionati = c.CampiSelezionati;
            this.Positions = c.Positions;
        }

        public void FindStartEnd()
        {
            if (CampiSelezionati == null || CampiSelezionati.Count == 0) return;
            List<int> posizioni = new List<int>();
            this.START_POS = 0;
            this.END_POS = 0;

            try
            {
                foreach (KeyValuePair<string, object> k in this.CampiSelezionati)
                {
                    var c = JsonConvert.DeserializeObject<Campo>(k.Value.ToString());
                    posizioni.Add(c.Posizione);
                }
                this.START_POS = posizioni.Min();
                this.END_POS = posizioni.Max();
            }
            catch (Exception)
            {
                // nope
            }
        }

        public void InitPositions()
        {
            if(CampiSelezionati.Count > 0)
            {
                foreach (KeyValuePair<string, object> k in this.CampiSelezionati)
                {
                    var c = JsonConvert.DeserializeObject<Campo>(k.Value.ToString());
                    Positions.Add(c.Posizione);
                }
                Positions.Sort();
            }
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