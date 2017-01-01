using System;
using System.Collections.ObjectModel;
using BatchDataEntry.Helpers;



namespace BatchDataEntry.Models
{
    /// <summary>
    /// Modello per la definizione dei campi per ogni "applicazione" cioè viene generato un modello personalizzato
    /// per l'utilizzo di determinati campi senza inserirli ogni volta. (I modelli sono unici)
    /// </summary>

    public class Modello : BaseModel
    {
        private int _id;
        private string _nome;
        private bool _origine;
        private ObservableCollection<Campo> _campi;
        private string _filecsv;
        private string _separatore;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        public string Nome {
            get { return _nome; }
            set
            {
                if (value != _nome)
                {
                    _nome = value;
                    OnPropertyChanged("Nome");
                }
            }
        }
        public bool OrigineCsv
        {
            get { return _origine; }
            set
            {
                if (value != _origine)
                {
                    _origine = value;
                    OnPropertyChanged("OrigineCsv");
                }
            }
        }
        public ObservableCollection<Campo> Campi
        {
            get { return _campi; }
            set
            {
                if (value != _campi)
                {
                    _campi = value;
                    OnPropertyChanged("Campi");
                }
            }
        }
        public string PathFileCsv
        {
            get { return _filecsv; }
            set
            {
                if (value != _filecsv)
                {
                    _filecsv = value;
                    OnPropertyChanged("PathFileCsv");
                }
            }
        }
        public string Separatore
        {
            get { return _separatore; }
            set
            {
                if (value != _separatore)
                {
                    _separatore = value;
                    OnPropertyChanged("Separatore");
                }
            }
        }


        public Modello()
        {
            this.OrigineCsv = false;
            this.Campi = new ObservableCollection<Campo>();
        }

        public Modello(string nome, bool orig, ObservableCollection<Campo> campi)
        {
            this.Nome = nome;
            this.OrigineCsv = orig;
            this.Campi = campi;
        }

        public Modello(int id, string nome, bool orig, ObservableCollection<Campo> campi)
        {
            this.Id = id;
            this.Nome = nome;
            this.OrigineCsv = orig;
            this.Campi = campi;
        }

        public Modello(int id, string nome, bool orig, ObservableCollection<Campo> campi, string path, string sep)
        {
            this.Id = id;
            this.Nome = nome;
            this.OrigineCsv = orig;
            this.Campi = campi;
            this.PathFileCsv = path;
            this.Separatore = sep;
        }

        public Modello(DBModels.Modello m)
        {
            Id = m.Id;
            Nome = m.Nome;
            OrigineCsv = m.OrigineCsv;
            DatabaseHelper db = new DatabaseHelper();
            Campi = db.CampoQuery(string.Format("SELECT * FROM Campo WHERE IdModello={0}", m.Id));
            PathFileCsv = m.PathFileCsv;
            Separatore = m.Separatore;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}, {4}, c: {5}]", this.Id, this.Nome, this.OrigineCsv, this.PathFileCsv, this.Separatore, Campi.Count);
        }
    }
}
