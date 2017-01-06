using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelBatchSelected : ViewModelMain
    {
        private readonly string FILENAME_CACHE = "cache.ini";
        private readonly string FILENAME_DBCSV = "db.csv";
        private readonly string FILENAME_VALUES = "autocomp.ini";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ViewModelBatchSelected() { }

        public ViewModelBatchSelected(Batch batch)
        {
            _currentBatch = batch;
            long bytes = Utility.GetDirectorySize(batch.DirectoryInput);
            TaskLoadGrid();
            Dimensioni = String.Format("{0} MB", Utility.ConvertSize((double)bytes, "MB").ToString("0.00"));
            NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);      
        }

        private Batch _currentBatch { get; set; }

        private DataView _dtSource;
        public DataView DataSource
        {
            get { return _dtSource; }
            set
            {
                if (_dtSource != value)
                {
                    _dtSource = value;
                    RaisePropertyChanged("DataSource");
                }
            }
        }

        private int _ndocs;
        public int NumeroDocumenti
        {
            get { return _ndocs; }
            set
            {
                if (_ndocs != value)
                {
                    _ndocs = value;
                    RaisePropertyChanged("NumeroDocumenti");
                }
            }
        }

        private string _dimfiles;
        public string Dimensioni
        {
            get { return _dimfiles; }
            set
            {
                if (_dimfiles != value)
                {
                    _dimfiles = value;
                    RaisePropertyChanged("Dimensioni");
                }
            }
        }

        private int _curDoc;
        public int DocumentoCorrente
        {
            get { return _curDoc; }
            set
            {
                if (_curDoc != value)
                {
                    _curDoc = value;
                    RaisePropertyChanged("DocumentoCorrente");
                }
            }
        }

        private int _ulti;
        public int UltimoIndicizzato
        {
            get { return _ulti; }
            set
            {
                if (_ulti != value)
                {
                    _ulti = value;
                    RaisePropertyChanged("UltimoIndicizzato");
                }
            }
        }

        private DataRowView _selectedRow;
        public DataRowView SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != value)
                {
                    _selectedRow = value;
                    RaisePropertyChanged("SelectedRow");
                }
            }
        }

        private RelayCommand _continuaCmd;
        public ICommand ContinuaCmd
        {
            get
            {
                if (_continuaCmd == null)
                {
                    _continuaCmd = new RelayCommand(param => this.ContinuaInserimento());
                }
                return _continuaCmd;
            }
        }

        private RelayCommand _checkCmd;
        public ICommand CheckCmd
        {
            get
            {
                if (_checkCmd == null)
                {
                    _checkCmd = new RelayCommand(param => this.CheckBatch());
                }
                return _checkCmd;
            }
        }

        private RelayCommand _daSelezCmd;
        public ICommand ContinuaDaSelezioneCmd
        {
            get
            {
                if (_daSelezCmd == null)
                {
                    _daSelezCmd = new RelayCommand(param => this.ContinuaDaSelezione(), param => this.isSelectedRow);
                }
                return _daSelezCmd;
            }
        }

        private RelayCommand _deleteCmd;
        public ICommand EliminaSelezCmd
        {
            get {
                if (_deleteCmd == null)
                {
                    _deleteCmd = new RelayCommand(param => this.EliminaSelezione(), param => this.isSelectedRow);
                }
                return _deleteCmd;
            }
        }

        private bool isSelectedRow
        {
            get { return (SelectedRow == null) ? false : true; }
        }

        private void ContinuaDaSelezione()
        {
            int posizioneCol = -1;
            #if DEBUG
            Console.Write("riga selezionata: (");
            for(int i=0; i< this.DataSource.Count;i++)
            {
                Console.Write("[" + SelectedRow[i] + "]");
            }
            Console.Write(")\n");
            #endif
            foreach (Campo campo in SelectedBatch.Applicazione.Campi)
            {
                if (campo.IndicePrimario == true)
                {
                    posizioneCol = campo.Posizione;
                }
            }

            if (posizioneCol == -1)
            {
                logger.Error("Errore impossibile continuare da posizione (Colonna)...");
                return;
            }

            Views.Documento continua = new Views.Documento();
            string indexFile = SelectedRow[posizioneCol].ToString();
            if (!string.IsNullOrEmpty(indexFile))
            {
                continua.DataContext = new ViewModelDataEntry(indexFile, _currentBatch, FILENAME_CACHE, FILENAME_DBCSV, FILENAME_VALUES);
                continua.Show();
            }
        }

        private void ContinuaInserimento()
        {
            if (_currentBatch != null)
            {
                Views.Documento inserimento = new Views.Documento();
                inserimento.DataContext = new ViewModelDataEntry(_currentBatch, FILENAME_CACHE, FILENAME_DBCSV, FILENAME_VALUES);
                inserimento.Show();
            }
        }

        private void EliminaSelezione()
        {
            if (SelectedRow != null)
            {
                int colPrimary = 0;
                if(_currentBatch.Applicazione == null)
                    _currentBatch.LoadModel();
                if(_currentBatch.Applicazione.Campi == null || _currentBatch.Applicazione.Campi.Count == 0)
                    _currentBatch.Applicazione.LoadCampi();

                foreach (Campo c in _currentBatch.Applicazione.Campi)
                {
                    if (c.IndicePrimario)
                    {
                        colPrimary = c.Posizione;
                        break;
                    }
                }
                string cacheFile = Path.Combine(_currentBatch.DirectoryOutput, FILENAME_CACHE);
                string dbFile = Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV);
                string fileName = Business.Cache.GetKey(cacheFile, "Documenti", SelectedRow[colPrimary].ToString()).ElementAt(0).Value;
                Business.Csv.DeleteRow(fileName, SelectedRow[colPrimary].ToString(), colPrimary);
                File.Delete(fileName);
                TaskLoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }


        private void CheckBatch()
        {

            MessageBox.Show("Non implementato!");
            //TODO: da verificare come implementarlo
        }

        private async void TaskLoadGrid()
        {
            await LoadDataTask();
        }

        private async Task LoadDataTask()
        {

            Func<DataView> function = new Func<DataView>(() => LoadDataFromFile(Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV), _currentBatch.Applicazione.Campi));
            DataView res = await Task.Run<DataView>(function);
            if (res != null)
                this.DataSource = res;
            else
                logger.Warn(string.Format("[SelectedBatchView]Errore nel caricamento dei dati dal file {0} nella tabella", Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV)));
        }

        private DataView LoadDataFromFile(string path, ObservableCollection<Campo> campi)
        {
            if(File.Exists(path) && campi != null && campi.Count > 0)
                return (DataView)Helpers.DataSource.CreateDataSourceFromCsv(path, campi);
            else
                return null;
        }
    }
}
