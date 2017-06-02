using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BatchDataEntry.Business;
using NLog;
using Batch = BatchDataEntry.Models.Batch;
using BatchDataEntry.Abstracts;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNewBatch : ViewModelBase
    {
#region Attributes
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private bool _alreadyExist;
        private bool _saved = false;
        private AbsDbHelper db = null;

        private bool _isVisible = false;
        public bool IsVisible
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

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RaisePropertyChanged("Progress");
                }
            }
        }

        private int _maxVal;
        public int MaxValue
        {
            get { return _maxVal; }
            set
            {
                if (_maxVal != value)
                {
                    _maxVal = value;
                    RaisePropertyChanged("MaxValue");
                }
            }
        }

        private Batch _currentBatch;
        public Batch CurrentBatch
        {
            get { return _currentBatch; }
            set
            {
                if (_currentBatch != value)
                {
                    _currentBatch = value;
                    RaisePropertyChanged("CurrentBatch");
                }
            }
        }

        private IEnumerable<Modello> _models;
        public IEnumerable<Modello> Models
        {
            get { return _models; }
            set
            {
                if(_models != value) { 
                    _models = value;
                    RaisePropertyChanged("Models");
                }
            }
        }

        private string _indexType;
        public string IndexType
        {
            get { return _indexType; }
            set
            {
                if (_indexType != value)
                {
                    _indexType = value;
                    RaisePropertyChanged("IndexType");
                }
            }
        }

        private RelayCommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(param => this.AddOrUpdateBatchItem(), param => this.CanSave);
                }
                return _applyCommand;
            }
        }

        private bool CanSave
        {
            get
            {
                if (!_saved &&
                    !string.IsNullOrEmpty(CurrentBatch.Nome) &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryInput) &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryOutput))
                    return true;
                else
                    return false;
            }
        }

#endregion

        public ViewModelNewBatch()
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
        }

        public ViewModelNewBatch(Batch batch)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
        }

        public ViewModelNewBatch(AbsDbHelper dbs)
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            db = dbs;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
        }

        public ViewModelNewBatch(Batch batch, AbsDbHelper dbs)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            db = dbs;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            DatabaseHelper dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], CurrentBatch.DirectoryOutput);
            Generate(CurrentBatch, dbc);
            //backgroundWorker.ReportProgress(i);
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            Progress = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsVisible = false;
            MessageBox.Show("Finito");
        }

        public void AddOrUpdateBatchItem()
        {
            IsVisible = true;
            Progress = 0;
            MaxValue = 100;
            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update              
                Batch batch = new Batch(CurrentBatch, db);
                db.Update(batch);          
            }
            else
            {
                Batch batch = new Batch(CurrentBatch, db);
                db.Insert(batch);     
            }
            RaisePropertyChanged("Batches");
            backgroundWorker.RunWorkerAsync();
            _saved = true;
        }

        public void PopulateComboboxModels()
        {
            if (db == null) return;
            Models = db.IEnumerableModelli();
            RaisePropertyChanged("Models");
        }

        #region CacheFilesInit

        public bool FillCacheDb(DatabaseHelper dbcache, AbsDbHelper dbdata, Batch b)
        {
            List<string> files = new List<string>();
            if (CurrentBatch.Applicazione.Id == 0 && dbdata == null) CurrentBatch.LoadModel(dbdata);
            else if (CurrentBatch.Applicazione.Id == 0) CurrentBatch.LoadModel(dbdata);
            if (!CurrentBatch.Applicazione.OrigineCsv) return false;
            if (CurrentBatch.Applicazione.Campi.Count == 0) CurrentBatch.Applicazione.LoadCampi(dbdata);

            // Genera il file di cache partendo dal file csv invece che dalla lista all'interno della cartella.
            if (IndexType.Contains("Automatica"))
            {
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Automatica ###");
                #endif
                int indexColumn = 0;

                for (int i = 0; i < CurrentBatch.Applicazione.Campi.Count; i++)
                {
                    if (CurrentBatch.Applicazione.Campi[i].IndicePrimario)
                    {
                        indexColumn = CurrentBatch.Applicazione.Campi[i].Posizione;
                        break;
                    }                       
                }
                
                List<string> lines = Csv.ReadRows(CurrentBatch.Applicazione.PathFileCsv, Convert.ToChar(_currentBatch.Applicazione.Separatore));
                MaxValue = lines.Count;
                List<Document> documents = new List<Document>();
                for (int i = 0; i < lines.Count; i++)
                {
                    Document doc = new Document();
                    doc.Id = i + 1;
                    try
                    {
                        string[] cells = lines[i].Split(b.Applicazione.Separatore[0]);

                        if (cells.Length == 1) // viene restituita la riga di partenza
                        {
                            MessageBox.Show(
                                "Non è possibile leggere correttamente le righe del file csv di origine, sicuro di aver inserito il delimitatore di campo giusto?");
                            return false;
                        }

                        // Il nome del file corrisponde all'index (primario) impostato nel modello!
                        string cleanValue = cells[indexColumn].Replace("'", "");
                        string queryCheck = string.Format("SELECT Count(Id) FROM Documenti WHERE FileName = '{0}'", Path.GetFileNameWithoutExtension(cleanValue));
                        if (dbcache.Count(queryCheck) > 0) continue;
                        doc.FileName = cleanValue;

                        doc.FileName = cells[indexColumn];
                        string absPath = Path.Combine(CurrentBatch.DirectoryInput, cleanValue.Contains(".pdf") ? cleanValue : String.Format("{0}.pdf", cleanValue));
                        doc.Path = absPath;
                        doc.IsIndexed = true;

                        for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                        {
                            string colName = b.Applicazione.Campi.ElementAt(z).Nome;
                            
                            string celValue = (z < cells.Length && !string.IsNullOrEmpty(cells[z])) ? cells[z] : "";
                            doc.Voci.Add(Record.Create(b.Applicazione.Campi.ElementAt(z), z, celValue));
                        }

                    }
                    catch (Exception er)
                    {
                        logger.Error("### Init basato su file Csv esterno ### break at line: " + (i+1));
                        logger.Error(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message));
                        ConsoleErrorPrint(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message), er);
                        throw;
                    }
                    documents.Add(doc);
                    // Copia del pdf corrispondente nella cartella di output
                    if (!string.IsNullOrEmpty(CurrentBatch.PatternNome))
                        Utility.CopiaPdf(doc.Path, CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".pdf"));
                    else
                        Utility.CopiaPdf(doc.Path, CurrentBatch.DirectoryOutput, doc.FileName + ".pdf");
                }

                for (int i = 0; i < documents.Count; i++)
                {
                    dbcache.InsertRecordDocumento(b, documents[i]);
                    backgroundWorker.ReportProgress(i);
                }
            }
            else if (IndexType.Contains("Manuale"))
            {
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Manuale ###");
                #endif

                if (b.TipoFile == TipoFileProcessato.Pdf)
                    files = Directory.GetFiles(b.DirectoryInput, "*.pdf").ToList();
                else
                    files = Directory.GetDirectories(b.DirectoryInput).ToList();

                if (files.Count == 0) return false;

                files = files.CustomSort().ToList();
                MaxValue = files.Count;
                #if DEBUG
                Console.WriteLine(@"### Inizio indicizzazione files ###");
                #endif

                // adesso per ogni file in files aggiungere un record fileName, path, false
                int lastId = 0;
                try
                {
                    lastId = dbcache.Count(@"SELECT seq FROM SQLITE_SEQUENCE WHERE name='Documenti'");                    
                }
                catch (Exception)
                {
                    logger.Warn("Query fallita per il recuper del lastId");
                    throw;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    string queryCheck = string.Format("SELECT Count(Id) FROM Documenti WHERE FileName = '{0}'", Path.GetFileNameWithoutExtension(files[i]));
                    if (dbcache.Count(queryCheck) > 0) continue;

                    Document doc = new Document();
                    doc.Id = lastId + 1;
                    doc.FileName = Path.GetFileNameWithoutExtension(files[i]);
                    doc.Path = files[i];
                    doc.IsIndexed = false;

                    if (!b.Applicazione.OrigineCsv)
                    {                     
                        for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                        {
                            string valore = (string.IsNullOrEmpty(b.Applicazione.Campi.ElementAt(z).ValorePredefinito))
                                ? ""
                                : b.Applicazione.Campi.ElementAt(z).ValorePredefinito;
                            doc.Voci.Add(Record.Create(b.Applicazione.Campi.ElementAt(z), z, valore));
                        }
                    }
                    dbcache.InsertRecordDocumento(b, doc);
                    backgroundWorker.ReportProgress(i);
                    lastId++;
                }
            }
            else
            {
                MessageBox.Show("Metodo indicizzazione non selezionato");
                return false;
            }
    
            return true;
        }

        public void Generate(Batch batch, DatabaseHelper dbc)
        {
            try
            {
                if (batch.Applicazione == null || batch.Applicazione.Id == 0) batch.LoadModel(db);
                if (batch.Applicazione.Campi == null || batch.Applicazione.Campi.Count == 0) batch.Applicazione.LoadCampi(db);

                List<string> campi = new List<string>();
                for (int i = 0; i < batch.Applicazione.Campi.Count; i++)
                    campi.Add(batch.Applicazione.Campi[i].Nome);

                string pathCacheDb = Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"]);

                if (!File.Exists(pathCacheDb))
                {                              
                    bool res = false;

                    dbc.CreateCacheDb(campi);
                    res = FillCacheDb(dbc, db, batch);
                }else if (File.Exists(pathCacheDb))
                {
                    // Appende al db cache i nuovi pdf
                    FillCacheDb(dbc, db, batch);
                }

                if (CheckOutputDirFiles(batch.DirectoryOutput))
                    CreateMissingFiles(true, batch);
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Console.WriteLine("Exception generazione cache db");
                    Console.WriteLine(ex.ToString());
                #endif
                logger.Error(ex.ToString());
            }
        }

        public void CreateMissingFiles(bool res, Batch m)
        {
            if (res)
                CreateDbCsv(m.DirectoryOutput);
        }

        public bool CheckOutputDirFiles(string output_path)
        {
            bool res = !File.Exists(Path.Combine(output_path, ConfigurationManager.AppSettings["csv_file_name"]));
            return res;
        }

        public void CreateDbCsv(string output_dir)
        {
            if (ConfigurationManager.AppSettings["csv_file_name"] == null)
                File.Create(Path.Combine(output_dir, @"db.csv"));
            else
                File.Create(Path.Combine(output_dir, ConfigurationManager.AppSettings["csv_file_name"]));
        }

        #endregion
    }
}
