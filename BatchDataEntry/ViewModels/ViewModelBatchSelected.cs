using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using iTextSharp.text.pdf;
using NLog;
using Document = BatchDataEntry.Models.Document;
using MessageBox = System.Windows.MessageBox;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelBatchSelected : ViewModelMain
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        #region Members

        private Batch _currentBatch;
        public Batch CurrentBatch
        {
            get { return _currentBatch; }
            set
            {
                if (_currentBatch != value)
                    _currentBatch = value;
                RaisePropertyChanged("CurrentBatch");
            }
        }

        private bool _isVisible = false;
        public bool isVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("isVisible");
                }
            }
        }

        private int _MaxProgressBarValue;
        public int MaxProgressBarValue
        {
            get { return _MaxProgressBarValue; }
            set
            {
                if (_MaxProgressBarValue != value)
                {
                    _MaxProgressBarValue = value;
                    RaisePropertyChanged("MaxProgressBarValue");
                }
            }
        }

        private int _ValueProgressBar;
        public int ValueProgressBar
        {
            get { return _ValueProgressBar; }
            set
            {
                if (_ValueProgressBar != value)
                {
                    _ValueProgressBar = value;
                    RaisePropertyChanged("ValueProgressBar");
                }
            }
        }

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
                if (SelectedRowIndex <= 0)
                    return false;           
                return true;
            }
        }

        private bool CanConvertTiff
        {
            get { return (_currentBatch.TipoFile == TipoFileProcessato.Tiff) ? true : false; }
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

        private RelayCommand _genTiffCmd;
        public ICommand GeneraTiffCmd
        {
            get
            {
                if (_genTiffCmd == null)
                {
                    _genTiffCmd = new RelayCommand(param => GeneratePdf(), param => CanConvertTiff);
                }
                return _genTiffCmd;
            }
        }

        #endregion

        #region BackGroundWorker

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ConvertTiffRecord(CurrentBatch);
                LoadGrid();
                RaisePropertyChanged("DataSource");
            }
            catch (Exception ex)
            {
                logger.Error("[ERROR_CONVERSION]" + ex.ToString());
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            ValueProgressBar = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isVisible = false;
            MessageBox.Show("Conversione Tiff Terminata");
        }

        #endregion

        private void UpdateValues()
        {
            DatabaseHelper db = new DatabaseHelper();
            Batch tmp = db.GetBatchById(CurrentBatch.Id);
            if (tmp != null)
            {
                CurrentBatch.UltimoIndicizzato = tmp.UltimoIndicizzato;
                CurrentBatch.DocCorrente = tmp.DocCorrente;
                RaisePropertyChanged("CurrentBatch");
            }

        }

        private void ContinuaDaSelezione()
        {
            if (SelectedRowIndex > 0 && _currentBatch != null)
            {
                var continua = new Documento();
                var indexFile = SelectedRowIndex;
                continua.DataContext = new ViewModelDocumento(_currentBatch, indexFile);
                continua.ShowDialog();
                LoadGrid();
                RaisePropertyChanged("DataSource");
                UpdateValues();
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
                UpdateValues();
            }
        }

        private void EliminaSelezione()
        {
            if (SelectedRowIndex > 0)
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

                    Document current = new Document(_currentBatch, db.GetDocumento(SelectedRowIndex));
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

        private async void CheckBatch()
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
            
            await Task.Factory.StartNew(() =>
            {
                var dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"],
                    _currentBatch.DirectoryOutput);
                try
                {
                    var rowCount = 0;
                    var cmd = string.Format("SELECT COUNT(*) FROM {0}", "Documenti");
                    rowCount = dbc.Count(cmd);
                    var processedRow = 0;
                    var cmd2 = $"SELECT COUNT(*) FROM {"Documenti"} WHERE isIndicizzato=1";
                    processedRow = dbc.Count(cmd2);

                    StatusBarCol1 = $"File Indicizzati ({processedRow} / {rowCount})";
                    if (processedRow == rowCount && rowCount > 0)
                        StatusBarCol2 = "Batch Completato!";
                }
                catch (Exception e)
                {
                    logger.Error("[ERROR_CHECK]" + e.ToString());                
                }

            });
            LoadGrid();
            RaisePropertyChanged("DataSource");
        }

        private void GeneratePdf()
        {
            try
            {
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error("[BGWORKER][GENPDF]" + ex);
            }
            

        }


        #region Constructors

        public ViewModelBatchSelected()
        {
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            // not required for this question, but is a helpful event to handle
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        public ViewModelBatchSelected(Batch batch)
        {
            CurrentBatch = batch;

            var bytes = Utility.GetDirectorySize(batch.DirectoryInput);
            Dimensioni = Utility.ConvertSize(bytes, "MB").ToString("0.00");
            LoadGrid();           
            NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);
            CurrentBatch.Applicazione.LoadCampi();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            // not required for this question, but is a helpful event to handle
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            MaxProgressBarValue = 100;
            ValueProgressBar = 0;
        }

        #endregion

        public void GenerateCsv()
        {
            if (DataSource.Rows.Count == 0)
                return;

            if (!File.Exists(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"])))
            {
                File.Create(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
            }
            
            var export = new Views.Export();          
            export.DataContext = new ViewModelExport(DataSource, Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
            bool dr = (bool)export.ShowDialog();
            if (dr)
            {
                StatusBarCol1 = String.Format("Generazione del file Csv completato!");
            }
            else
            {
                StatusBarCol1 = String.Format("Generazione del file Csv Fallita! Controlla il file di log");
            }
        }

        private void LoadGrid()
        {
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            DataSource = db.GetDataTableDocumenti();
        }
        
        private async void ConvertTiffRecord(Batch b)
        {
            if (b.TipoFile != TipoFileProcessato.Tiff)
                return;

            if (b.Applicazione == null || b.Applicazione.Id == 0)
                b.LoadModel();
            if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                b.Applicazione.LoadCampi();

            var db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], b.DirectoryOutput);
            var docs = db.GetDocuments();
            MaxProgressBarValue = docs.Count;
            ValueProgressBar = 0;

            foreach (var doc in docs)
            {
                Document dc = new Document(_currentBatch, doc);
                string new_path = String.Empty;
                var fileName = Path.GetFileName(dc.Path);
                if (!Utility.ContainsOnlyNumbers(fileName))
                    fileName = Regex.Replace(fileName, "[A-Za-z]", "");

                new_path = Path.Combine(b.DirectoryOutput, fileName + ".pdf");
                await Task.Factory.StartNew(() =>
                {
                    ConvertTiffToPdf(dc.Path, new_path);
                });
                
                dc.Path = new_path;
                db.UpdateRecordDocumento(dc);
                ValueProgressBar++;
            }
        }

        public void ConvertTiffToPdf(string source_dir, string path_output_file)
        {
            List<string> files = Directory.GetFiles(source_dir, "*.*").Where(s => s.ToLower().EndsWith(".tif") || s.ToLower().EndsWith(".tiff")).ToList();
            Logger logger = LogManager.GetCurrentClassLogger();

            try
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                using (PdfWriter pdfwriter = PdfWriter.GetInstance(document, new FileStream(path_output_file, FileMode.Create)))
                {
                    document.Open();
                    document.SetMargins(0, 0, 0, 0);

                    foreach (var file in files)
                    {
                        iTextSharp.text.Image tiff = iTextSharp.text.Image.GetInstance(file);
                        
                        iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(tiff.Width, tiff.Height);
                        document.SetPageSize(pageSize);
                        document.NewPage();

                        PdfContentByte cb = pdfwriter.DirectContent;
                        tiff.SetAbsolutePosition(0, 0);
                        cb.AddImage(tiff);
                    }
                    document.Close();
                }
                
            }
            catch (Exception e)
            {
                logger.Error(string.Format("[{0}] {1}", path_output_file, e.ToString()));
            }
        }
    }
}