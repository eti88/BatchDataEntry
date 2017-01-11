using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Apitron.PDF.Rasterizer;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDocumento : ViewModelBase
    {
        private readonly string FILENAME_CACHE = Properties.Settings.Default["filename_cache_output_dir"].ToString();
        private readonly string FILENAME_DBCSV = Properties.Settings.Default["filename_db_output_dir"].ToString();
        private readonly string FILENAME_VALUES = Properties.Settings.Default["filename_autocomplete"].ToString();
        private readonly string FILENAME_IN_CACHE = Properties.Settings.Default["filename_cache_input_dir"].ToString();

        private Document _document;
        private Doc _dc;
        private Batch _batch;

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
        public Doc DocFile
        {
            get { return _dc; }
            set
            {
                if (_dc != value)
                    _dc = value;
                RaisePropertyChanged("DocFile");
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
            this.Document = null;
            this.Batch = new Batch();
        }

        public ViewModelDocumento(Batch _currentBatch,string filePos, string filePath)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;

            DocFile.Index = filePos;
            DocFile.Path = filePath;
            DocFile.Voci = AddInputsToPanel(Batch.Applicazione.Campi);
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
