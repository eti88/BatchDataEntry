using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
            get
            {
                if (SelectedRowIndex == null || SelectedRowIndex <= 0)
                    return false;           
                return true;
            }
        }

        private int _selectedRowIndex;
        public int SelectedRowIndex
        {
            get { return _selectedRowIndex; }
            set
            {
                if (_selectedRowIndex != value)
                {
                    _selectedRowIndex = (value + 1);
                    RaisePropertyChanged("SelectedRowIndex");
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

        private RelayCommand _genCmd;
        public ICommand GeneraCsvCmd
        {
            get
            {
                if (_genCmd == null)
                {
                    _genCmd = new RelayCommand(param => GenerateCsv());
                }
                return _genCmd;
            }
        }
        

        #endregion

        private void ContinuaDaSelezione()
        {
            if (SelectedRowIndex != null && SelectedRowIndex > 0 && _currentBatch != null)
            {
                var continua = new Documento();
                var indexFile = SelectedRowIndex;
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
                catch (Exception e)
                {
                    logger.Error("[ERRORE CONTINUA INSERIMENTO]" + e.ToString());
                }

                LoadGrid();
                RaisePropertyChanged("DataSource");
            }
        }

        private void EliminaSelezione()
        {
            if (SelectedRowIndex != null && SelectedRowIndex > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    if (_currentBatch.Applicazione == null || _currentBatch.Applicazione.Id == 0)
                        _currentBatch.LoadModel();
                    if (_currentBatch.Applicazione.Campi == null || _currentBatch.Applicazione.Campi.Count == 0)
                        _currentBatch.Applicazione.LoadCampi();

                    var db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"],
                        _currentBatch.DirectoryOutput);

                    #if DEBUG
                    Console.WriteLine("SelectedRowIndex: " + SelectedRowIndex);
                    #endif

                    Document current = new Document(_currentBatch, db, db.GetDocumento(SelectedRowIndex));
                    db.Delete("Documenti", string.Format("Id= {0}", current.Id));

                    try
                    {
                        // elimina il record dal file db (csv)
                        if (File.Exists(current.Path))
                            File.Delete(current.Path); // elimina il file pdf originario
                    }
                    catch (Exception e)
                    {
                        logger.Warn("[DELETE_RECORD]" + e.ToString());
                    }
                    try
                    {
                        var files = Directory.GetFiles(_currentBatch.DirectoryOutput, string.Format("{0}.pdf", current.FileName));
                        if (files.Length > 0)
                            File.Delete(files[0]);
                    }
                    catch (Exception e)
                    {
                        logger.Warn("[DELETE_RECORD]" + e.ToString());
                    }
                    LoadGrid();
                    RaisePropertyChanged("DataSource");
                });
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
                var cmd = string.Format("SELECT COUNT(*) FROM {0}", "Documenti");
                rowCount = dbc.Count(cmd);
                var processedRow = 0;
                var cmd2 = string.Format("SELECT COUNT(*) FROM {0} WHERE isIndicizzato=1", "Documenti");
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

        public async void GenerateCsv()
        {
            if (DataSource.Rows.Count == 0)
                return;

            if (!File.Exists(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"])))
            {
                File.Create(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
            }

            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);

            DataTable tmpDt = db.GetDataTableDocumenti();
            List<string> tmpRecords = new List<string>();
            int colNum = tmpDt.Columns.Count;

            //TODO: da modificare
            /* Attualmente genera il file csv con tutte le colonne della tabella
             * invece bisogna creare una view che permette di selezionare le colonne dalla dt
             * da usare per la generazione
             */

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    for (int i = 0; i < tmpDt.Rows.Count; i++)
                    {
                        StringBuilder record = new StringBuilder();
                        for (int z = 0; z < colNum; z++)
                        {
                            record.Append(tmpDt.Rows[i][z]);
                            if (z != colNum - 1)
                                record.Append(';');
                        }
                        tmpRecords.Add(record.ToString());
                    }
                    Csv.AddRows(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]), tmpRecords);
                    StatusBarCol1 = String.Format("Generazione del file Csv completato!");
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    StatusBarCol1 = String.Format("Generazione del file Csv Fallita! Controlla il file di log");
                }
            });
        }

        private void LoadGrid()
        {
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            DataSource = db.GetDataTableDocumenti();
        }
    }
}