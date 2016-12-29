using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;


namespace BatchDataEntry.Models
{
    /// <summary>
    /// Modello per la definizione dei campi per ogni "applicazione" cioè viene generato un modello personalizzato
    /// per l'utilizzo di determinati campi senza inserirli ogni volta. (I modelli sono unici)
    /// </summary>

    [System.ComponentModel.DataAnnotations.Schema.Table("Modello")]
    public class Modello : INotifyPropertyChanged
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _Nome;
        public string Nome {
            get { return _Nome; }
            set
            {
                if (_Nome != null)
                {
                    _Nome = value;
                    RaisePropertyChanged("Nome");
                }
            }
        }

        private Batch.TipoFileProcessato _Tipo;
        public Batch.TipoFileProcessato Tipo {
            get { return _Tipo; }
            set
            {
                if (_Tipo != null)
                {
                    _Tipo = value;
                    RaisePropertyChanged("Tipo");
                }
            }
        }

        private bool _OrigineCsv;
        public bool OrigineCsv {
            get { return _OrigineCsv; }
            set
            {
                if (_OrigineCsv != null)
                {
                    _OrigineCsv = value;
                    RaisePropertyChanged("OrigineCsv");
                }
            }
        }

        private List<Campo> _Campi;
        [Ignore]
        public List<Campo> Campi {
            get { return _Campi; }
            set
            {
                if (_Campi != null)
                {
                    _Campi = value;
                    RaisePropertyChanged("Campi");
                }
            }
        }

        [Indexed]
        public int IdFileCSV { get; set; }

        private FileCSV _OrigineDatiCsv;
        [Ignore]
        public FileCSV OrigineDatiCSV {
            get { return _OrigineDatiCsv; }
            set
            {
                if (_OrigineDatiCsv != null)
                {
                    _OrigineDatiCsv = value;
                    RaisePropertyChanged("OrigineDatiCsv");
                }
            }
        }

        private int IdFileCsv { get; set; }

        public Modello()
        {
            this.OrigineCsv = false;
            this.Campi = new List<Campo>();
            this.OrigineDatiCSV = new FileCSV();
        }

        public Modello(string nome, Batch.TipoFileProcessato tipo, bool orig, List<Campo> campi, FileCSV file)
        {
            this.Nome = nome;
            this.Tipo = tipo;
            this.OrigineCsv = orig;
            this.Campi = campi;
            this.OrigineDatiCSV = file;
        }

        public Modello(int id, string nome, Batch.TipoFileProcessato tipo, bool orig, List<Campo> campi, FileCSV file)
        {
            this.Id = id;
            this.Nome = nome;
            this.Tipo = tipo;
            this.OrigineCsv = orig;
            this.Campi = campi;
            this.OrigineDatiCSV = file;
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
