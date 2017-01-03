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
        private MementoModello MyMemento;

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

        public string contaColonne
        {
            get { return string.Format("Colonne: {0}", Campi.Count); }
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
            this.MyMemento = new MementoModello(nome, orig, campi, path, sep);
        }

        public Modello(Modello m)
        {
            this.Id = m.Id;
            this.Nome = m.Nome;
            this.OrigineCsv = m.OrigineCsv;
            this.Campi = m.Campi;
            this.PathFileCsv = m.PathFileCsv;
            this.Separatore = m.Separatore;
            this.MyMemento = new MementoModello(m.Nome, m.OrigineCsv, m.Campi, m.PathFileCsv, m.Separatore);
        }

        public Modello(DBModels.Modello m)
        {
            Id = m.Id;
            Nome = m.Nome;
            OrigineCsv = m.OrigineCsv;
            DatabaseHelper db = new DatabaseHelper();
            ObservableCollection<Campo> tmpc = db.CampoQuery(string.Format("SELECT * FROM Campo WHERE IdModello={0}", m.Id));
            Campi = tmpc;
            PathFileCsv = m.PathFileCsv;
            Separatore = m.Separatore;
            this.MyMemento = new MementoModello(m.Nome, m.OrigineCsv, tmpc, m.PathFileCsv, m.Separatore);
        }

        public void Revert()
        {
            this.Nome = this.MyMemento.nome;
            this.OrigineCsv = this.MyMemento.origine;
            this.Campi = this.MyMemento.campi;
            this.PathFileCsv = this.MyMemento.filecsv;
            this.Separatore = this.MyMemento.separatore;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            Modello modello = obj as Modello;
            if (this.Id != modello.Id)
                return false;
            if (this.Nome != modello.Nome)
                return false;
            if (this.OrigineCsv != modello.OrigineCsv)
                return false;
            if (this.Campi != modello.Campi)
                return false;
            if (this.PathFileCsv != modello.PathFileCsv)
                return false;
            if (this.Separatore != modello.Separatore)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int resutl = this.Id.GetHashCode();
            resutl += string.IsNullOrEmpty(this.Nome) ? 0 : this.Nome.GetHashCode();
            resutl += (this.OrigineCsv) ? 1 : 0;
            resutl += (Campi == null) ? 0 : 1;
            resutl += string.IsNullOrEmpty(this.PathFileCsv) ? 0 : this.PathFileCsv.GetHashCode();
            resutl += string.IsNullOrEmpty(this.Separatore) ? 0 : this.Separatore.GetHashCode();
            return resutl;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}, {4}, c: {5}]", this.Id, this.Nome, this.OrigineCsv, this.PathFileCsv, this.Separatore, Campi.Count);
        }
    }

    public class MementoModello
    {
        public readonly string nome;
        public readonly bool origine;
        public readonly ObservableCollection<Campo> campi;
        public readonly string filecsv;
        public readonly string separatore;

        public MementoModello(string _nome, bool _origine, ObservableCollection<Campo> _campi, string _file, string _sep)
        {
            this.nome = _nome;
            this.origine = _origine;
            this.campi = _campi;
            this.filecsv = _file;
            this.separatore = _sep;
        }
    }
}
