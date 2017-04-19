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
    public class ViewModelBatchSelected : ViewModelMain
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private DatabaseHelperSqlServer dbsql;

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

        #region commands

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

        private RelayCommand _genLstCmd;
        public ICommand GeneraListCmd
        {
            get
            {
                if (_genLstCmd == null)
                {
                    _genLstCmd = new RelayCommand(param => GenerateListTxt());
                }
                return _genLstCmd;
            }
        }

        private RelayCommand _changePathsCmd;
        public ICommand ChangePathsCmd
        {
            get
            {
                if (_changePathsCmd == null)
                {
                    _changePathsCmd = new RelayCommand(param => ChangePhatsIntoCacheDb());
                }
                return _changePathsCmd;
            }
        }

        private RelayCommand _changeNumerationCmd;
        public ICommand ChangeNumerationCmd
        {
            get
            {
                if (_changeNumerationCmd == null)
                {
                    _changeNumerationCmd = new RelayCommand(param => ChangeNumerationPdf());
                }
                return _changeNumerationCmd;
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

        public void UpdateValues()
        {
            DatabaseHelper db;
            Batch tmp;

            if (dbsql == null)
            {
                db = new DatabaseHelper();
                tmp = db.GetBatchById(CurrentBatch.Id);
            }
            else
            {
                tmp = dbsql.GetBatchById(CurrentBatch.Id);
            }
                
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
                var indexFile = (SelectedRowIndex > 0) ? SelectedRowIndex - 1 : SelectedRowIndex;
                if(dbsql == null)
                    continua.DataContext = new ViewModelDocumento(_currentBatch, indexFile, null);
                else
                    continua.DataContext = new ViewModelDocumento(_currentBatch, indexFile, dbsql);
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
                if(dbsql == null)
                    inserimento.DataContext = new ViewModelDocumento(_currentBatch, null);
                else
                    inserimento.DataContext = new ViewModelDocumento(_currentBatch, dbsql);
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

        public void EliminaSelezione()
        {
            if (SelectedRowIndex > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    if (_currentBatch.Applicazione == null || _currentBatch.Applicazione.Id == 0)
                    {
                        if (dbsql == null)
                            _currentBatch.LoadModel();
                        else
                            _currentBatch.LoadModel(dbsql);
                    }   
                    if (_currentBatch.Applicazione.Campi == null || _currentBatch.Applicazione.Campi.Count == 0)
                    {
                        if (dbsql == null)
                            _currentBatch.Applicazione.LoadCampi();
                        else
                            _currentBatch.Applicazione.LoadCampi(dbsql);
                    }
                    // Recupera il database di cache (differente da quello relativo ai batch e modelli    
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

        public async void CheckBatch()
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

        public void GeneratePdf()
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
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        public ViewModelBatchSelected(Batch batch)
        {
            try
            {
                CurrentBatch = batch;
                CurrentBatch.LoadModel();
                var bytes = Utility.GetDirectorySize(batch.DirectoryInput);
                Dimensioni = Utility.ConvertSize(bytes, "MB").ToString("0.00");
                LoadGrid();
                NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);
                CurrentBatch.Applicazione.LoadCampi();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.ProgressChanged += ProgressChanged;
                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                Properties.Settings.Default.StartFocusCol = GetInitialTextBoxPosition(CurrentBatch.Applicazione);
                Properties.Settings.Default.CurrentBatch = CurrentBatch.Id;
                Properties.Settings.Default.Save();
                MaxProgressBarValue = 100;
                ValueProgressBar = 0;
                //SelectedRowIndex = 12; // per selezionare la riga x (es: index: 5, (rappresenta la row 6), se selezioni la riga 6, si posiziona alla 7
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }          
        }

        public ViewModelBatchSelected(Batch batch, DatabaseHelperSqlServer dbp)
        {
            try
            {
                dbsql = dbp;
                CurrentBatch = batch;
                CurrentBatch.LoadModel(dbsql);
                var bytes = Utility.GetDirectorySize(batch.DirectoryInput);
                Dimensioni = Utility.ConvertSize(bytes, "MB").ToString("0.00");
                LoadGrid();
                NumeroDocumenti = Utility.CountFiles(batch.DirectoryInput, batch.TipoFile);
                CurrentBatch.Applicazione.LoadCampi(dbsql);
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.ProgressChanged += ProgressChanged;
                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                Properties.Settings.Default.StartFocusCol = GetInitialTextBoxPosition(CurrentBatch.Applicazione);
                Properties.Settings.Default.CurrentBatch = CurrentBatch.Id;
                Properties.Settings.Default.Save();
                MaxProgressBarValue = 100;
                ValueProgressBar = 0;
                //SelectedRowIndex = 12; // per selezionare la riga x (es: index: 5, (rappresenta la row 6), se selezioni la riga 6, si posiziona alla 7
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
        }

        #endregion

        private int GetInitialTextBoxPosition(Modello mod)
        {
            return mod.StartFocusColumn;
        }

        public void GenerateCsv()
        {
            if (DataSource.Rows.Count == 0)
                return;

            if (!File.Exists(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"])))
            {
                File.Create(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
            }
            
            var export = new Views.Export();
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            DataTable insertedDocs = db.GetDataTableWithQuery("SELECT * FROM Documenti WHERE isIndicizzato = 1");
            export.DataContext = new ViewModelExport(insertedDocs, Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
            if (export.ShowDialog() == true)
            {
                int idx = (Properties.Settings.Default.LastExportIndex > 0)
                    ? Properties.Settings.Default.LastExportIndex
                    : 0;

                Dictionary<int, string> doc = new Dictionary<int, string>();

                if (idx > 0)
                {
                    try
                    {
                        doc = db.GetDocumento(idx);
                        foreach (var item in doc)
                        {
                            if (item.Key == 1)
                            {
                                CurrentBatch.UltimoDocumentoEsportato = item.Value;
                                DatabaseHelper dbmain = new DatabaseHelper();
                                dbmain.UpdateRecordBatch(CurrentBatch);
                                break;
                            }                              
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("[GenerateCsv] " + ex.ToString());
                    }

                }
                    
            }
            StatusBarCol1 = String.Format("Csv generato...");
        }

        private void LoadGrid()
        {
            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            DataSource = db.GetDataTableDocumenti();
        }
        
        public async void ConvertTiffRecord(Batch b)
        {
            if (b.TipoFile != TipoFileProcessato.Tiff)
                return;

            if (b.Applicazione == null || b.Applicazione.Id == 0)
            {
                if(dbsql == null)
                    b.LoadModel();
                else
                    b.LoadModel(dbsql);
            }         
            if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
            {
                if (dbsql == null)
                    b.Applicazione.LoadCampi();
                else
                    b.Applicazione.LoadCampi(dbsql);
            }

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

        public void GenerateListTxt()
        {
            List<string> files =
                Directory.GetFiles(CurrentBatch.DirectoryOutput, "*.*")
                    .Where(x => x.ToLower().EndsWith(".pdf"))
                    .ToList();

            if (files == null || files.Count == 0)
            {
                MessageBox.Show(string.Format("{0} non contiene pdf per generare LISTA.txt", CurrentBatch.DirectoryOutput));
                return;
            }
            string outpuFile = Path.Combine(CurrentBatch.DirectoryOutput, @"LISTA.txt");
            Task.Factory.StartNew(() =>
            {                            
                for (int i = 0; i < files.Count; i++)
                    files[i] = new FileInfo(files[i]).Name;

                files = files.CustomSort().ToList();

                File.WriteAllLines(outpuFile, files);
                MessageBox.Show("Scrittura terminata LISTA.txt");
            });         
        }

        public void ChangePhatsIntoCacheDb()
        {
            string newPdfDirctory = "";
            DialogText dlgTxt = new DialogText();
            if (dlgTxt.ShowDialog() == true)
            {
                newPdfDirctory = dlgTxt.FullPath;
                if (!Directory.Exists(newPdfDirctory))
                {
                    MessageBox.Show("Path non valido!");
                    return;
                }
            }
            else
            {
                return;
            }

            if (string.IsNullOrEmpty(newPdfDirctory)) return;

            DatabaseHelper db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
            List<Document> documents = new List<Document>();
            try
            {
                documents = db.GetDocumentsListPartial();
                string originalPath = Path.GetDirectoryName(documents.ElementAt(0).Path);
               
                foreach (var doc in documents)
                {
                    doc.Path = doc.Path.Replace(originalPath, newPdfDirctory);
                    db.UpdateRecordDocumento(doc);
                }
                MessageBox.Show("Percorsi aggiornati");
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
                logger.Warn("Modifica del percoso dei pdf nel cache db fallita");
            }
        }

        public void ChangeNumerationPdf()
        {
            DialogNumeration dlgNums = new DialogNumeration();
            if (dlgNums.ShowDialog() == true)
            {
                #if DEBUG
                Console.WriteLine("GetZeri: " + dlgNums.GetZeri);
                Console.WriteLine("GetStartNum: " + dlgNums.GetStartNumber);
                Console.WriteLine("GetIndexSart: " + dlgNums.GetIndexStart);
                Console.WriteLine("GetIndexEnd: " + dlgNums.GetIndexStop);
                #endif

                int idxStart = Convert.ToInt32(dlgNums.GetIndexStart);
                int idxStop = Convert.ToInt32(dlgNums.GetIndexStop) - 1;
                int zeroNum = Convert.ToInt32(dlgNums.GetZeri);
                int startNum = Convert.ToInt32(dlgNums.GetStartNumber);

                if(idxStop < idxStart) return;

                List<string> files =
                Directory.GetFiles(CurrentBatch.DirectoryOutput, "*.*")
                    .Where(x => x.ToLower().EndsWith(".pdf"))
                    .ToList();

                if (files.Count == 0) return;
                files = files.CustomSort().ToList();
                Dictionary<int, string> modifiedRecords = new Dictionary<int, string>();

                try
                {
                    var dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"],
                        _currentBatch.DirectoryOutput);
                    var docs = dbc.GetDocumentsListPartial("SELECT * FROM Documenti WHERE isIndicizzato = 1");
                    for (int i = 0; i < docs.Count; i++)
                    {
                        string fileName = "";
                        if (string.IsNullOrEmpty(CurrentBatch.PatternNome))
                        {
                            if (docs[i].FileName.Equals(Path.GetFileNameWithoutExtension(files[i])))
                            {
                                fileName = string.Format("{0}.pdf", startNum.ToString("D" + zeroNum));
                                File.Move(files[i],
                                    Path.Combine(Path.GetDirectoryName(files[i]),
                                        string.Format("{0}.pdf", startNum.ToString("D" + zeroNum))));
                            }
                        }
                        else
                        {
                            string vanillaFileName = Path.GetFileNameWithoutExtension(files[i]);
                            vanillaFileName.Replace(CurrentBatch.PatternNome, "");
                            if (docs[i].FileName.Replace(CurrentBatch.PatternNome, "") ==
                                (Path.GetFileNameWithoutExtension(files[i]).Replace(CurrentBatch.PatternNome, "")))
                            {
                                fileName = string.Format("{0}{1}.pdf",
                                            (string.IsNullOrEmpty(CurrentBatch.PatternNome))
                                                ? ""
                                                : CurrentBatch.PatternNome, startNum.ToString("D" + zeroNum));
                                File.Move(files[i],
                                    Path.Combine(Path.GetDirectoryName(files[i]),
                                        string.Format("{0}{1}.pdf",
                                            (string.IsNullOrEmpty(CurrentBatch.PatternNome))
                                                ? ""
                                                : CurrentBatch.PatternNome, startNum.ToString("D" + zeroNum))));
                            }
                            else
                            {
                                #if DEBUG
                                Console.WriteLine("{0} == {1}", docs[i].FileName.Replace(CurrentBatch.PatternNome, ""),
                                    vanillaFileName);
                                #endif
                                logger.Warn(
                                    string.Format(
                                        "File {0} non rinominato che dovrebbe corrispondere al documento {1}",
                                        Path.GetFileNameWithoutExtension(files[i]), vanillaFileName));
                                continue;
                            }
                        }
                        modifiedRecords.Add(i, Path.GetFileNameWithoutExtension(fileName));
                        startNum++;
                    }
                    // Genera csv
                    DataTable dt = new DataTable();
                    string sqlQuery = String.Format("SELECT * FROM Documenti WHERE (Id BETWEEN {0} AND {1}) AND (isIndicizzato = 1)", idxStart, idxStop + 1);
                    dt = dbc.GetDataTableWithQuery(sqlQuery);
                    if (dt.Rows.Count == 0) return;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if(modifiedRecords.ElementAt(i).Key == i)
                            dt.Rows[i]["FileName"] = modifiedRecords.ElementAt(i).Value;
                    }

                    if (!File.Exists(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"])))
                        File.Create(Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));

                    var export = new Views.Export();
                    export.DataContext = new ViewModelExport(dt, Path.Combine(_currentBatch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]));
                    if (export.ShowDialog() == true)
                    {
                        int idx = (Properties.Settings.Default.LastExportIndex > 0)
                            ? Properties.Settings.Default.LastExportIndex
                            : 0;

                        Dictionary<int, string> doc = new Dictionary<int, string>();

                        if (idx > 0)
                        {
                            try
                            {
                                DatabaseHelper db = new DatabaseHelper(
                                    ConfigurationManager.AppSettings["cache_db_name"], _currentBatch.DirectoryOutput);
                                doc = db.GetDocumento(idx);
                                foreach (var item in doc)
                                {
                                    if (item.Key == 1)
                                    {
                                        CurrentBatch.UltimoDocumentoEsportato = item.Value;
                                        DatabaseHelper dbmain = new DatabaseHelper();
                                        dbmain.UpdateRecordBatch(CurrentBatch);
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("[ChangeNumerationPdf] " + ex.ToString());
                            }

                        }
                    }
                    StatusBarCol1 = String.Format("Csv generato...");
                    //General lista
                    GenerateListTxt();
                    StatusBarCol1 = String.Format("LIST.txt generato...");
                }
                catch (ArgumentOutOfRangeException)
                {
                    logger.Warn("Rinumerazione pdf ha generato un Exception in quanto sono stati richiesti di elaborare più elementi di quelli effettivamente disponibili");
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
            }
        }
    }
}