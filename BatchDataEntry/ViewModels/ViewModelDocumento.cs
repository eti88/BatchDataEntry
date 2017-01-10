using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Apitron.PDF.Rasterizer;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDocumento : ViewModelBase
    {
        private Document _document;
        private Batch _batch;
        private string _fileCache;
        private string _fileDb;
        private string _fileValues;
        private ObservableCollection<Voce> _voci;

        public Document Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                    _document = value;
                RaisePropertyChanged("Document");
            }
        }
        public Batch Batch
        {
            get { return _batch; }
            set
            {
                if (_batch != value)
                    _batch = value;
                RaisePropertyChanged("Batch");
            }
        }
        public string FileCache
        {
            get { return _fileCache; }
            set
            {
                if (_fileCache != value)
                    _fileCache = value;
                RaisePropertyChanged("FileCache");
            }
        }
        public string FileDb
        {
            get { return _fileDb; }
            set
            {
                if (_fileDb != value)
                    _fileDb = value;
                RaisePropertyChanged("FileDb");
            }
        }
        public string FileValues
        {
            get { return _fileValues; }
            set
            {
                if (_fileValues != value)
                    _fileValues = value;
                RaisePropertyChanged("FileValues");
            }
        }
        public ObservableCollection<Voce> Voci
        {
            get { return _voci; }
            set
            {
                if (_voci != value)
                    _voci = value;
                RaisePropertyChanged("Voci");
            }
        }

        private RelayCommand _cmdNext;
        public ICommand CmdNext
        {
            get
            {
                if (_cmdNext == null)
                {
                    _cmdNext = new RelayCommand(param => this.Indicizza());
                }
                return _cmdNext;
            }
        }

        private RelayCommand _cmdJump;
        public ICommand CmdJump
        {
            get
            {
                if (_cmdJump == null)
                {
                    _cmdJump = new RelayCommand(param => this.Salta());
                }
                return _cmdJump;
            }
        }

        private RelayCommand _cmdStop;
        public ICommand CmdStop
        {
            get
            {
                if (_cmdStop == null)
                {
                    _cmdStop = new RelayCommand(param => this.Interrompi());
                }
                return _cmdStop;
            }
        }
        
        
        public ViewModelDocumento()
        {
            
        }

        public ViewModelDocumento(Batch _currentBatch, string fileIndex, string _cache, string _db, string _vals)
        {
            this._fileCache = _cache;
            this._fileDb = _db;
            this._fileValues = _vals;
            if (_currentBatch != null)
                Batch = _currentBatch;

            string[] record = fileIndex.Split(';');

            Voci = AddInputsToPanel(Batch.Applicazione.Campi);
        }

        protected ObservableCollection<Voce> AddInputsToPanel(ObservableCollection<Campo> campi)
        {
            ObservableCollection<Voce> voci = new ObservableCollection<Voce>();

            foreach (Campo campo in campi)
            {
                voci.Add(new Voce(campo.Posizione, campo.Nome));
            }

            return voci;
        }

        private bool IsFileLocked(string filePath, int secondsToWait)
        {
            bool isLocked = true;
            int i = 0;

            while (isLocked && ((i < secondsToWait) || (secondsToWait == 0)))
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open)) { }
                    return false;
                }
                catch (IOException e)
                {
                    var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                    isLocked = errorCode == 32 || errorCode == 33;
                    i++;

                    if (secondsToWait != 0)
                        new System.Threading.ManualResetEvent(false).WaitOne(1000);
                }
            }

            return isLocked;
        }

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



        public void Indicizza()
        {

        }

        public void Salta()
        {


        }

        public void Interrompi()
        {

        }
    }
}
