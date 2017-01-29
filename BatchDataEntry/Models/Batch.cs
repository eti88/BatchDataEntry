using System;
using BatchDataEntry.Helpers;


namespace BatchDataEntry.Models
{
    public class Batch : BaseModel
    {
        private int _id;
        private string _nome;
        private TipoFileProcessato _tipo;
        private string _in;
        private string _out;
        private int _idModello;
        private Modello _app;
        private int _numdoc;
        private int _numpages;
        private long _dim;
        private int _corrente;
        private int _utlimo;

        public int Id {
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
        public TipoFileProcessato TipoFile {
            get { return _tipo; }
            set
            {
                if (value != _tipo)
                {
                    _tipo = value;
                    OnPropertyChanged("TipoFile");
                }
            }
        }
        public string DirectoryInput {
            get { return _in; }
            set
            {
                if (value != _in)
                {
                    _in = value;
                    OnPropertyChanged("DirectoryInput");
                }
            }
        }
        public string DirectoryOutput {
            get { return _out; }
            set
            {
                if (value != _out)
                {
                    _out = value;
                    OnPropertyChanged("DirectoryOutput");
                }
            }
        }
        public int IdModello {
            get { return _idModello; }
            set
            {
                if (value != _idModello)
                {
                    _idModello = value;
                    OnPropertyChanged("IdModello");
                }
            }
        }
        public Modello Applicazione {
            get { return _app; }
            set
            {
                if (value != _app)
                {
                    _app = value;
                    OnPropertyChanged("Applicazione");
                }
            }
        }
        public int NumDoc {
            get { return _numdoc; }
            set
            {
                if (value != _numdoc)
                {
                    _numdoc = value;
                    OnPropertyChanged("NumDoc");
                }
            }
        }
        public int NumPages {
            get { return _numpages; }
            set
            {
                if (value != _numpages)
                {
                    _numpages = value;
                    OnPropertyChanged("NumPages");
                }
            }
        }
        public long Dimensioni {
            get { return _dim; }
            set
            {
                if (value != _dim)
                {
                    _dim = value;
                    OnPropertyChanged("Dimensioni");
                }
            }
        }
        public int DocCorrente {
            get { return _corrente; }
            set
            {
                if (value != _corrente)
                {
                    _corrente = value;
                    OnPropertyChanged("DocCorrente");
                }
            }
        }
        public int UltimoIndicizzato {
            get { return _utlimo; }
            set
            {
                if (value != _utlimo)
                {
                    _utlimo = value;
                    OnPropertyChanged("UltimoIndicizzato");
                }
            }
        }

        

        public Batch()
        {
            Applicazione = new Modello();
            NumDoc = 0;
            NumPages = 0;
            Dimensioni = 0L;
            DocCorrente = 0;
            UltimoIndicizzato = 0;
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = new Modello();
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = 0;
            this.NumPages = 0;
            this.Dimensioni = 0;
            this.DocCorrente = 0;
            this.UltimoIndicizzato = 0;      
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
        }

        public Batch(string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
        }

        public Batch(int id, string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui)
        {
            this.Id = id;
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
        }

        public Batch(int id, string nome, TipoFileProcessato t, string input, string output, Modello m, int nd, int np, long dim, int dc, int ui, int fk)
        {
            this.Id = id;
            this.Nome = nome;
            this.TipoFile = t;
            this.DirectoryInput = input;
            this.DirectoryOutput = output;
            this.Applicazione = m;
            this.NumDoc = nd;
            this.NumPages = np;
            this.Dimensioni = dim;
            this.DocCorrente = dc;
            this.UltimoIndicizzato = ui;
        }

        public Batch(Batch b)
        {
            this.Id = b.Id;
            this.Nome = b.Nome;
            this.TipoFile = b.TipoFile;
            this.DirectoryInput = b.DirectoryInput;
            this.DirectoryOutput = b.DirectoryOutput;
            this.IdModello = b.IdModello;
            if (this.IdModello > 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                this.Applicazione = db.GetModelloById(b.IdModello);
            }

            this.NumDoc = b.NumDoc;
            this.NumPages = b.NumPages;
            this.Dimensioni = 0;
            this.DocCorrente = b.DocCorrente;
            this.UltimoIndicizzato = b.UltimoIndicizzato;

        }

        public void LoadModel()
        {
            if (this.IdModello > 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                this.Applicazione = db.GetModelloById(this.IdModello);
            }
        }

        public override string ToString()
        {
            return String.Format("[{0},{1},{2},{3},{4},{5}]", this.Id, this.Nome, this.TipoFile, this.DirectoryInput, this.DirectoryOutput, this.IdModello);
        }
    }
}
