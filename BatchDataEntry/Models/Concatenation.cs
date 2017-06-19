﻿using BatchDataEntry.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Non salvare nel db
        private Dictionary<string, object> _campi;
        public Dictionary<string, object> Campi
        {
            get { return _campi; }
            set
            {
                if (_campi != value)
                {
                    _campi = value;
                    OnPropertyChanged("Campi");
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
            Campi = new Dictionary<string, object>();
        }

        public Concatenation(int id, string nome, int modello)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.Campi = new Dictionary<string, object>();
        }

        public Concatenation(int id, string nome, int modello, Dictionary<string, object> campi)
        {
            this.Id = id;
            this.Nome = nome;
            this.Modello = modello;
            this.Campi = campi;
        }

        public Concatenation(Concatenation c)
        {
            this.Id = c.Id;
            this.Nome = c.Nome;
            this.Modello = c.Modello;
            this.Campi = c.Campi;
        }


        public void LoadCampi(DatabaseHelperSqlServer db)
        {
            //Campi = db.LoadConcatenations(Modello);
        }
    }
}
