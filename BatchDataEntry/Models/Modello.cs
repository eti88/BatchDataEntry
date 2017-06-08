using System;
using System.Collections.ObjectModel;
using BatchDataEntry.Helpers;
using BatchDataEntry.Abstracts;
using System.Collections.Generic;

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
        private int _startFocusColumn;
        private int _csvColumn;

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
        public int StartFocusColumn
        {
            get { return _startFocusColumn; }
            set
            {
                if (value != _startFocusColumn)
                {
                    _startFocusColumn = value;
                    OnPropertyChanged("StartFocusColumn");
                }
            }
        }
        public int CsvColumn
        {
            get { return _csvColumn; }
            set
            {
                if (value != _csvColumn)
                {
                    _csvColumn = value;
                    OnPropertyChanged("CsvColumn");
                }
            }
        }

        public Modello()
        {
            Id = 0;
            Nome = string.Empty;
            OrigineCsv = false;
            Campi = new ObservableCollection<Campo>();
            PathFileCsv = string.Empty;
            Separatore = string.Empty;
            StartFocusColumn = 0;
            CsvColumn = -1;
        }

        public Modello(string nome, bool orig, ObservableCollection<Campo> campi)
        {
            Nome = nome;
            OrigineCsv = orig;
            Campi = campi;
            PathFileCsv = string.Empty;
            Separatore = string.Empty;
            StartFocusColumn = 0;
            CsvColumn = -1;
        }

        public Modello(int id, string nome, bool orig, ObservableCollection<Campo> campi)
        {
            Id = id;
            Nome = nome;
            OrigineCsv = orig;
            Campi = campi;
            PathFileCsv = string.Empty;
            Separatore = string.Empty;
            StartFocusColumn = 0;
            CsvColumn = -1;
        }

        public Modello(int id, string nome, bool orig, ObservableCollection<Campo> campi, string path, string sep)
        {
            Id = id;
            Nome = nome;
            OrigineCsv = orig;
            Campi = campi;
            PathFileCsv = path;
            Separatore = sep;
            MyMemento = new MementoModello(nome, orig, campi, path, sep, StartFocusColumn);
        }

        public Modello(Modello m)
        {
            if(m == null) return;
            Id = m.Id;
            Nome = m.Nome;
            OrigineCsv = m.OrigineCsv;
            Campi = m.Campi;
            PathFileCsv = m.PathFileCsv;
            Separatore = m.Separatore;
            StartFocusColumn = m.StartFocusColumn;
            CsvColumn = m.CsvColumn;
            MyMemento = new MementoModello(m.Nome, m.OrigineCsv, m.Campi, m.PathFileCsv, m.Separatore, m.StartFocusColumn);
        }

        public void LoadCampi(AbsDbHelper db)
        {
            if (Id > 0)
            {
                ObservableCollection<Campo> tmpc = db.CampoQuery(string.Format("SELECT * FROM Campo WHERE IdModello={0}", Id));
                Campi = tmpc;
            }
        }

        public void Revert()
        {
            Nome = MyMemento.nome;
            OrigineCsv = MyMemento.origine;
            Campi = MyMemento.campi;
            PathFileCsv = MyMemento.filecsv;
            Separatore = MyMemento.separatore;
            StartFocusColumn = MyMemento.focus;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            Modello modello = obj as Modello;
            if (Id != modello.Id)
                return false;
            if (Nome != modello.Nome)
                return false;
            if (OrigineCsv != modello.OrigineCsv)
                return false;
            if (Campi != modello.Campi)
                return false;
            if (PathFileCsv != modello.PathFileCsv)
                return false;
            if (Separatore != modello.Separatore)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int resutl = Id.GetHashCode();
            resutl += string.IsNullOrEmpty(Nome) ? 0 : Nome.GetHashCode();
            resutl += (OrigineCsv) ? 1 : 0;
            resutl += (Campi == null) ? 0 : 1;
            resutl += string.IsNullOrEmpty(PathFileCsv) ? 0 : PathFileCsv.GetHashCode();
            resutl += string.IsNullOrEmpty(Separatore) ? 0 : Separatore.GetHashCode();
            return resutl;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}, {4}, c: {5}, focus: {6}]", Id, Nome, OrigineCsv, PathFileCsv, Separatore, Campi.Count, StartFocusColumn);
        }
    }

    public class MementoModello
    {
        public readonly string nome;
        public readonly bool origine;
        public readonly ObservableCollection<Campo> campi;
        public readonly string filecsv;
        public readonly string separatore;
        public readonly int focus;

        public MementoModello(string _nome, bool _origine, ObservableCollection<Campo> _campi, string _file, string _sep, int _foc)
        {
            nome = _nome;
            origine = _origine;
            campi = _campi;
            filecsv = _file;
            separatore = _sep;
            focus = _foc;
        }
    }
}
