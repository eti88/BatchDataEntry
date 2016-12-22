using System.Collections.Generic;
using System.ComponentModel;

namespace BatchDataEntry.Models
{
    /// <summary>
    /// Modello per la definizione dei campi per ogni "applicazione" cioè viene generato un modello personalizzato
    /// per l'utilizzo di determinati campi senza inserirli ogni volta. (I modelli sono unici)
    /// </summary>
    public class Modello : INotifyPropertyChanged
    {
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

        private int _Tipo;
        public int Tipo {
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
