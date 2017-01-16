using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDocumento : ViewModelBase
    {
        /*
         * Modificare:
         * quando viene selezionato un specifico documento -> apre quel doc e quando viene indicizzato ritorna al selected batch.
         * quando viene selezionato contina (dopo check) -> cicla i file mancanti finché non sono finiti
           */

        private Doc _doc;
        private ObservableCollection<Doc> _dc;
        private Batch _batch;

        private int currentItem { get; set; }
        private static int lastItem { get; set; }

        public Doc DocFile
        {
            get { return _doc; }
            set
            {
                if (_doc != value)
                    _doc = value;
                RaisePropertyChanged("DocFile");
            }
        }
        public ObservableCollection<Doc> DocFiles
        {
            get { return _dc; }
            set
            {
                if (_dc != value)
                    _dc = value;
                RaisePropertyChanged("DocsFile");
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

        private ObservableCollection<Voce> _voci;
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

        private string[] _ms;
        public string[] MissingRows
        {
            get { return _ms; }
            set
            {
                if (_ms != value)
                    _ms = value;
                RaisePropertyChanged("MissingRows");
            }
        }

        #region Command
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
        #endregion

        public ViewModelDocumento()
        {
            this.Batch = new Batch();
        }

        public ViewModelDocumento(Batch _currentBatch, string indexRowVal)
        {
            Batch = _currentBatch;
            Voci = AddInputsToPanel(Batch.Applicazione.Campi);
            // Caricare dbcache e precuperare dati ultimo valore indicato
            
        }

        public ViewModelDocumento(Batch _currentBatch)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            Voci = AddInputsToPanel(Batch.Applicazione.Campi);
            // Caricare dbcache e partire dall'ultimo valore inserito
        }

        protected ObservableCollection<Voce> AddInputsToPanel(ObservableCollection<Campo> campi)
        {
            ObservableCollection<Voce> voci = new ObservableCollection<Voce>();

            foreach (Campo campo in campi)
            {
                if(campo.SalvaValori)
                    Voci.Add(new Voce(campo.Posizione, campo.Nome, campo.SalvaValori));
                else
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

        private string GetFileFromIndex(Batch b, string index)
        {
            //Dictionary<string, string> record = Cache.GetKey(Path.Combine(b.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]),
            //    "Documenti", index);
            //string path = record.Values.ElementAt(0);
            
            return "";
        }

        protected void LoadFile()
        {
            if (Batch.TipoFile == TipoFileProcessato.Pdf)
            {
                //isPdfFile("");
            }
            else
            {
                isTiffFile("");
            }
        }

        private void isTiffFile(string file)
        {
            // converti la dir dei tiff in pdf in una directory temporanea

        }



        public void Indicizza()
        {
            if (!IsFileLocked(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]), 5000))
            {
                // salvare record nel dbcsv
                // cambiare da false a true nel file cache input dir
                // copiare il file nell a dir
            }
        }

        public void Salta()
        {
            currentItem++;
            // bindare elementi dall'elemento currentItem in lista
        }

        public void Interrompi()
        {
            this.CloseWindow(true);
        }
    }
}
