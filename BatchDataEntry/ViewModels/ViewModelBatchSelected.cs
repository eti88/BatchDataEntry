using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Constructors
        public ViewModelBatchSelected() { }

        public ViewModelBatchSelected(Batch batch)
        {
            _currentBatch = batch;
            long bytes = Utility.GetDirectorySize(batch.DirectoryInput);
            TaskLoadGrid();
            Dimensioni = Utility.ConvertSize((double)bytes, "MB").ToString("0.00");
            NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);
            _currentBatch.Applicazione.LoadCampi();
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            db.GetBatchCachedTable(_currentBatch.Applicazione.Campi);

        }
        #endregion

        #region Members
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

        private string _statusBar1;
        public string StatusBarCol1
        {
            get { return _statusBar1; }
            set
            {
                if (_statusBar1 != value)
                {
                    _statusBar1 = value;
                    RaisePropertyChanged("StatusBarCol1");
                }
            }
        }

        private string _statusBar2;
        public string StatusBarCol2
        {
            get { return _statusBar2; }
            set
            {
                if (_statusBar2 != value)
                {
                    _statusBar2 = value;
                    RaisePropertyChanged("StatusBarCol2");
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

        private string[] _missingRow;
        public string[] MissingRow
        {
            get { return _missingRow; }
            set
            {
                if (_missingRow != value)
                    _missingRow = value;
                RaisePropertyChanged("MissingRow");
            }
        }

        private bool isSelectedRow
        {
            get { return (SelectedRow == null) ? false : true; }
        }

        private bool notEmptyMissingRow
        {
            get
            {
                if (MissingRow == null)
                    return false;
                if (MissingRow.Length > 0)
                    return true;
                else
                    return false;
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
        #endregion

        private RelayCommand _continuaCmd;
        public ICommand ContinuaCmd
        {
            get
            {
                if (_continuaCmd == null)
                {
                    _continuaCmd = new RelayCommand(param => this.ContinuaInserimento(), param => this.notEmptyMissingRow);
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

        private void ContinuaDaSelezione()
        {
            int posizioneCol = -1;
            
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
                //continua.DataContext = new ViewModelDataEntry(indexFile, _currentBatch, FILENAME_CACHE, FILENAME_DBCSV, FILENAME_VALUES);
                continua.Show();
            }
        }

        private void ContinuaInserimento()
        {
            if (_currentBatch != null)
            {
                Views.Documento inserimento = new Views.Documento();
                //inserimento.DataContext = new ViewModelDataEntry(_currentBatch, FILENAME_CACHE, FILENAME_DBCSV, FILENAME_VALUES);
                inserimento.Show();
            }
        }

        private void EliminaSelezione()
        {
            if (SelectedRow != null)
            {
                Task.Factory.StartNew(() =>
                {
                    int colPrimary = 0;
                    if (_currentBatch.Applicazione == null)
                        _currentBatch.LoadModel();
                    if (_currentBatch.Applicazione.Campi == null || _currentBatch.Applicazione.Campi.Count == 0)
                        _currentBatch.Applicazione.LoadCampi();

                    foreach (Campo c in _currentBatch.Applicazione.Campi)
                    {
                        if (c.IndicePrimario)
                        {
                            colPrimary = c.Posizione;
                            break;
                        }
                    }

                    /*
                     *
                     *
                     */

                    //string cacheInFile = Path.Combine(_currentBatch.DirectoryInput, FILENAME_IN_CACHE);
                    //string cacheFile = Path.Combine(_currentBatch.DirectoryOutput, FILENAME_CACHE);
                    //string dbFile = Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV);
                    //string fileName = Business.Cache.GetKey(cacheFile, "Documenti", SelectedRow[colPrimary].ToString()).ElementAt(0).Value;


                    //Business.Csv.DeleteRow(dbFile, SelectedRow[colPrimary].ToString(), colPrimary); // elimina il record dal file db (csv)
                    //Business.Csv.DeleteRow(cacheInFile, SelectedRow[colPrimary].ToString(), colPrimary); // elimina il record dal file cache (input dir)
                    //                                                                                     // Probabilmente non fa quello che deve fare
                    //Business.Csv.DeleteRow(fileName, SelectedRow[colPrimary].ToString(), colPrimary); // elimina il record dal file cache ini (output)

                    //File.Delete(fileName); // elimina il file pdf originario
                });
            
                TaskLoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }

        private void CheckBatch()
        {
            if (_currentBatch == null)
                return;

            string cacheDbPath = Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"]);

            if (!File.Exists(cacheDbPath))
            {
                MessageBox.Show(
                    "Database cache mancante! Aprire <<Modifica Batch>> e provare a risalvare le impostazioni per creare il database.");
                return;
            }
                
            /*
             Controlla quali file devono ancora essere indicizzati
             */
            Task.Factory.StartNew(() =>
            {
                //List<string> file = Csv.ReadRows(Path.Combine(_currentBatch.DirectoryInput, FILENAME_IN_CACHE));
                DatabaseHelper dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
                
                List<string> missing = new List<string>();

                int rowCount = 0;
                string cmd = string.Format("SELECT COUNT(*) FROM {0}", "Documento");
                rowCount = dbc.CountRecords(cmd);
                int processedRow = 0;
                string cmd2 = string.Format("SELECT COUNT(*) FROM {0} WHERE isIndicizzato = 1", "Documento");
                processedRow = dbc.CountRecords(cmd2);

                MissingRow = missing.ToArray();
                StatusBarCol1 = String.Format("File Indicizzati ({0} / {1})", processedRow, rowCount);
                if (processedRow == rowCount)
                    StatusBarCol2 = "Batch Completato!";
            });
        }

        private async void TaskLoadGrid()
        {
            await LoadDataTask();
        }

        private async Task LoadDataTask()
        {

            //Func<DataView> function = new Func<DataView>(() => LoadDataFromFile(Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV), _currentBatch.Applicazione.Campi));
            //DataView res = await Task.Run<DataView>(function);
            //if (res != null)
            //    this.DataSource = res;
            //else
            //    logger.Warn(string.Format("[SelectedBatchView]Errore nel caricamento dei dati dal file {0} nella tabella", Path.Combine(_currentBatch.DirectoryOutput, FILENAME_DBCSV)));
        }

        private DataView LoadDataFromFile(string path, ObservableCollection<Campo> campi)
        {
            


            if (File.Exists(path) && campi != null && campi.Count > 0)
                return (DataView)Helpers.DataSource.CreateDataSourceFromCsv(path, campi);
            else
                return null;
        }

        /*
         * -> cache.ini -> tiene traccia dell'associazione numero,file
         * -> db salva i record di output
         * -> nella cartella di input creare un altro csv che traccia i file mancanti
         *    quando viene effettuato il check controlla quest'ultimo file per vedere i record che han bisogno dell'inserimento
         */
    }
}
