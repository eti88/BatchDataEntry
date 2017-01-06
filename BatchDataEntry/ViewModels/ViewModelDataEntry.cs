using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDataEntry : ViewModelBase
    {
        private readonly string FILENAME_CACHE;
        private readonly string FILENAME_DBCSV;
        private readonly string FILENAME_VALUES;

        public ViewModelDataEntry()
        {
            FILENAME_CACHE = "";
            FILENAME_DBCSV = "";
            FILENAME_VALUES = "";
        }

        public ViewModelDataEntry(Batch b, string cache, string db, string auto)
        {
            FILENAME_CACHE = cache;
            FILENAME_DBCSV = db;
            FILENAME_VALUES = auto;
        }

        public ViewModelDataEntry(string fileIndex, Batch b, string cache, string db, string auto)
        {
            this.Index = fileIndex;
            this.Batch = b;
            this.FILENAME_CACHE = cache;
            this.FILENAME_DBCSV = db;
            this.FILENAME_VALUES = auto;
        }

        private string _index;
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChanged("Index");
                }
                
            }
        }

        private Batch _batch;
        public Batch Batch
        {
            get { return _batch; }
            set {
                if (_batch != value)
                {
                    _batch = value;
                    RaisePropertyChanged("Batch");
                }
            }
        }

        private RelayCommand _applyCmd;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCmd == null)
                {
                    _applyCmd = new RelayCommand(param => this.Indicizza());
                }
                return _applyCmd;
            }
        }

        private RelayCommand _saltaCmd;
        public ICommand SaltaCmd
        {
            get
            {
                if (_saltaCmd == null)
                {
                    _saltaCmd = new RelayCommand(param => this.Salta());
                }
                return _saltaCmd;
            }
        }

        private RelayCommand _stopCmd;
        public ICommand StopCmd
        {
            get
            {
                if (_stopCmd == null)
                {
                    _stopCmd = new RelayCommand(param => this.Interrompi());
                }
                return _stopCmd;
            }
        }

        /*
         - Nel caso della cancellazione di un documento oltre all'eliminazione del record bisogna eliminare anche il documento di origine (file)
         - Output copia dei file pdf
         */


        protected void LoadFile()
        {
            if (Batch.TipoFile == TipoFileProcessato.Pdf)
            {
                isPdfFile("");
            }
            else
            {
                isTiffFile("");
            }
        }

        private void isPdfFile(string file)
        {
            
        } 

        private void isTiffFile(string file)
        {
            
        }

        protected void LoadFields() { } // carica la lista dei campi



        public void Indicizza() { }
        public void Salta() { }
        public void Interrompi() { }
    }
}
