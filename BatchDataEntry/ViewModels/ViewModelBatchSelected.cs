using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
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
    internal class ViewModelBatchSelected : ViewModelMain
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Members

        private Batch _currentBatch { get; }

        private DataTable _dtSource;
        public DataTable DataSource
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

        private bool isSelectedRow
        {
            get { return SelectedRow == null ? false : true; }
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

        #region Cmd

        private RelayCommand _continuaCmd;
        public ICommand ContinuaCmd
        {
            get
            {
                if (_continuaCmd == null)
                {
                    _continuaCmd = new RelayCommand(param => ContinuaInserimento());
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
                    _checkCmd = new RelayCommand(param => CheckBatch());
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
                    _daSelezCmd = new RelayCommand(param => ContinuaDaSelezione(), param => isSelectedRow);
                }
                return _daSelezCmd;
            }
        }

        private RelayCommand _deleteCmd;
        public ICommand EliminaSelezCmd
        {
            get
            {
                if (_deleteCmd == null)
                {
                    _deleteCmd = new RelayCommand(param => EliminaSelezione(), param => isSelectedRow);
                }
                return _deleteCmd;
            }
        }

        #endregion

        private void ContinuaDaSelezione()
        {
            var posizioneCol = -1;

            foreach (var campo in SelectedBatch.Applicazione.Campi)
            {
                if (campo.IndicePrimario)
                {
                    posizioneCol = campo.Posizione;
                }
            }

            if (posizioneCol == -1)
            {
                logger.Error("Errore impossibile continuare da posizione (Colonna)...");
                return;
            }

            var continua = new Documento();
            var indexFile = SelectedRow[posizioneCol].ToString();
            if (!string.IsNullOrEmpty(indexFile))
            {
                continua.DataContext = new ViewModelDocumento(_currentBatch, indexFile);
                continua.ShowDialog();
                LoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }

        private void ContinuaInserimento()
        {
            if (_currentBatch != null)
            {
                var inserimento = new Documento();
                inserimento.DataContext = new ViewModelDocumento(_currentBatch);
                try
                {
                    inserimento.ShowDialog();
                }
                catch (Exception)
                {
                }

                LoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }

        private void EliminaSelezione()
        {
            if (SelectedRow != null)
            {
                Task.Factory.StartNew(() =>
                {
                    var colPrimary = 0;
                    if (_currentBatch.Applicazione == null)
                        _currentBatch.LoadModel();
                    if (_currentBatch.Applicazione.Campi == null || _currentBatch.Applicazione.Campi.Count == 0)
                        _currentBatch.Applicazione.LoadCampi();

                    foreach (var c in _currentBatch.Applicazione.Campi)
                    {
                        if (c.IndicePrimario)
                        {
                            colPrimary = c.Posizione;
                            break;
                        }
                    }

                    var db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"],
                        _currentBatch.DirectoryOutput);
                    Console.WriteLine("Valore: " + SelectedRow[colPrimary]);

                    var dbCsvFile = Path.Combine(_currentBatch.DirectoryOutput,
                        ConfigurationManager.AppSettings["csv_file_name"]);
                    Document current = new Document(db.GetDocumento(SelectedRow[colPrimary].ToString()));
                    db.Delete("Documento", string.Format("Id= {0}", current.Id));
                    string fileName = current.Path;

                    Csv.DeleteRow(dbCsvFile, SelectedRow[colPrimary].ToString(), colPrimary);
                    // elimina il record dal file db (csv)                  
                    File.Delete(fileName); // elimina il file pdf originario
                });

                LoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }

        private void CheckBatch()
        {
            if (_currentBatch == null)
                return;

            var cacheDbPath = Path.Combine(_currentBatch.DirectoryOutput,
                ConfigurationManager.AppSettings["cache_db_name"]);

            if (!File.Exists(cacheDbPath))
            {
                MessageBox.Show(
                    "Database cache mancante! Aprire <<Modifica Batch>> e provare a risalvare le impostazioni per creare il database.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"],
                    _currentBatch.DirectoryOutput);

                var rowCount = 0;
                var cmd = string.Format("SELECT COUNT(*) FROM {0}", "Documento");
                rowCount = dbc.Count(cmd);
                var processedRow = 0;
                var cmd2 = string.Format("SELECT COUNT(*) FROM {0} WHERE isIndicizzato = 1", "Documento");
                processedRow = dbc.Count(cmd2);

                StatusBarCol1 = string.Format("File Indicizzati ({0} / {1})", processedRow, rowCount);
                if (processedRow == rowCount && rowCount > 0)
                    StatusBarCol2 = "Batch Completato!";
            });
        }

        #region Constructors

        public ViewModelBatchSelected()
        {
        }

        public ViewModelBatchSelected(Batch batch)
        {
            _currentBatch = batch;
            var bytes = Utility.GetDirectorySize(batch.DirectoryInput);
            LoadGrid();
            Dimensioni = Utility.ConvertSize(bytes, "MB").ToString("0.00");
            NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);
            _currentBatch.Applicazione.LoadCampi();
        }

        #endregion

        

        #region LoadDataIntoGridView

        private void LoadGrid()
        {
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            DataSource = db.GetDataTable("Documento");
        }

        //private async void TaskLoadGrid()
        //{
        //    await LoadDataTask();
        //}

        //private async Task LoadDataTask()
        //{
        //    Func<DataView> function =
        //        () =>
        //            LoadDataFromFile(
        //                Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]),
        //                _currentBatch.Applicazione.Campi);
        //    var res = await Task.Run(function);
        //    if (res != null)
        //        DataSource = res;
        //    else
        //        logger.Warn("[SelectedBatchView]Errore nel caricamento dei dati dal file {0} nella tabella",
        //            Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
        //}

        //private DataView LoadDataFromFile(string path, ObservableCollection<Campo> campi)
        //{
        //    if (File.Exists(path) && campi != null && campi.Count > 0)
        //        return (DataView) Helpers.DataSource.CreateDataSourceFromCsv(path, campi);
        //    return null;
        //}

        #endregion
    }
}