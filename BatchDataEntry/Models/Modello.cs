using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchDataEntry.Models
{
    /// <summary>
    /// Modello per la definizione dei campi per ogni "applicazione" cioè viene generato un modello personalizzato
    /// per l'utilizzo di determinati campi senza inserirli ogni volta. (I modelli sono unici)
    /// </summary>
    
    public class Modello : INotifyPropertyChanged
    {
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

        public long Id { get; set; }

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

        private FileCSV _OrigineDatiCsv;
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

        public virtual Batch Batch { get; set; }
        public virtual Campo Campo { get; set; }

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
