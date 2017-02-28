using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BatchDataEntry.Business;
using NLog;
using Batch = BatchDataEntry.Models.Batch;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelNewBatch : ViewModelBase
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private bool _alreadyExist;
        private bool _saved = false;

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

        public ViewModelNewBatch()
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            // not required for this question, but is a helpful event to handle
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        public ViewModelNewBatch(Batch batch)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            // not required for this question, but is a helpful event to handle
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
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
            isVisible = false;
            MessageBox.Show("Finito");
        }

        private void AddOrUpdateBatchItem()
        {
            isVisible = true;
            Progress = 0;
            MaxValue = 100;

            DatabaseHelper db = new DatabaseHelper();

            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update              
                Batch batch = new Batch(CurrentBatch);
                db.UpdateRecordBatch(batch);
                RaisePropertyChanged("Batches");             
            }
            else
            {
                Batch batch = new Batch(CurrentBatch);
                db.InsertRecordBatch(batch);
                RaisePropertyChanged("Batches");
            }

            backgroundWorker.RunWorkerAsync();
            _saved = true;
        }

        private void PopulateComboboxModels()
        {
            DatabaseHelper db = new DatabaseHelper();
            IEnumerable<Modello> tmp = db.IEnumerableModelli();      
            Models = tmp;
            RaisePropertyChanged("Models");
        }

        #region CacheFilesInit

        public string convertQuotes(string str)
        {
            try
            {
                if (str == null)
                    return String.Empty;

                return str.Replace("'", "''");
            }
            catch (Exception)
            {
                return String.Empty;
            }

        }

        public bool fillCacheDb(DatabaseHelper db, Batch b)
        {
            List<string> files = new List<string>();

            // Genera il file di cache partendo dal file csv invece che dalla lista all'interno della cartella.
            if (IndexType.Contains("Automatica"))
            {
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Automatica ###");
                #endif

                if(CurrentBatch.Applicazione.Id == 0)
                    CurrentBatch.LoadModel();

                if (!CurrentBatch.Applicazione.OrigineCsv)
                    return false;

                if(CurrentBatch.Applicazione.Campi.Count == 0)
                    CurrentBatch.Applicazione.LoadCampi();

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

                        doc.FileName = cells[indexColumn];
                        doc.Path = Path.Combine(CurrentBatch.DirectoryInput, cells[indexColumn].Contains(".pdf") ? cells[indexColumn] : String.Format("{0}.pdf", cells[indexColumn]));
                        doc.IsIndexed = false;

                        for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                        {
                            string colName = b.Applicazione.Campi.ElementAt(z).Nome;
                            
                            string celValue = (z < cells.Length && !string.IsNullOrEmpty(cells[z])) ? cells[z] : "";
                            doc.Voci.Add(new Voce(colName, celValue));
                        }

                    }
                    catch (Exception er)
                    {
                        logger.Error("### Init basato su file Csv esterno ### break at line: " + (i+1));
                        logger.Error(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message));
                        throw;
                    }
                    documents.Add(doc);
                }
                documents = documents.OrderBy(o => o.FileName).ToList();
                for (int i = 0; i < documents.Count; i++)
                {
                    db.InsertRecordDocumento(b, documents[i]);
                    backgroundWorker.ReportProgress(i);
                }
                //db.InsertRecordDocumento(b, doc);
            }
            else if (IndexType.Contains("Manuale"))
            {
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Manuale ###");
                #endif

                if (b.TipoFile == TipoFileProcessato.Pdf)
                {
                    files = Directory.GetFiles(b.DirectoryInput, "*.pdf").ToList();
                }
                else
                {
                    files = Directory.GetDirectories(b.DirectoryInput).ToList();
                }

                if (files.Count == 0)
                    return false;

                List<string> lines;

                if (b.Applicazione.OrigineCsv)
                {
                    lines = Csv.ReadRows(CurrentBatch.Applicazione.PathFileCsv, Convert.ToChar(_currentBatch.Applicazione.Separatore));
                }
                else
                {
                    lines = new List<string>();
                }

                MaxValue = files.Count + lines.Count;
                List<Document> documents = new List<Document>();

                #if DEBUG
                Console.WriteLine(@"### Inizio indicizzazione files ###");
                #endif

                // adesso per ogni file in files aggiungere un record fileName, path, false
                for (int i = 0; i < files.Count - 1; i++)
                {
                    Document doc = new Document
                    {
                        Id = i + 1,
                        FileName = Path.GetFileNameWithoutExtension(files[i]),
                        Path = files[i],
                        IsIndexed = false
                    };

                    if (!b.Applicazione.OrigineCsv)
                    {                     
                        for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                        {
                            string colName = b.Applicazione.Campi.ElementAt(z).Nome;
                            string valore = (string.IsNullOrEmpty(b.Applicazione.Campi.ElementAt(z).ValorePredefinito))
                                ? ""
                                : b.Applicazione.Campi.ElementAt(z).ValorePredefinito;
                            doc.Voci.Add(new Voce(colName, valore));
                        }
                    }
                    documents.Add(doc); 
                }
                documents = documents.OrderBy(o => o.FileName).ToList();
                for (int i = 0; i < documents.Count; i++)
                {
                    db.InsertRecordDocumento(b, documents[i]);
                    backgroundWorker.ReportProgress(i);
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
                if (!File.Exists(Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"])))
                {
                    if (batch.Applicazione == null || batch.Applicazione.Id == 0)
                        batch.LoadModel();

                    if (batch.Applicazione.Campi == null || batch.Applicazione.Campi.Count == 0)
                        batch.Applicazione.LoadCampi();

                    List<string> campi = new List<string>();
                    for (int i = 0; i < batch.Applicazione.Campi.Count; i++)
                    {
                        campi.Add(batch.Applicazione.Campi[i].Nome);
                    }

                    bool res = false;

                    dbc.CreateCacheDb(campi);
                    res = fillCacheDb(dbc, batch);
                    if (!res)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            if (!File.Exists(Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"])))
                                return;
                            
                            bool isLocked = true;

                            while (isLocked)
                            {
                                try
                                {
                                    File.Delete(Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"]));
                                    Thread.Sleep(5000);
                                    isLocked = false;
                                }
                                catch (System.IO.IOException) { }
                                
                            }

                            

                            
                                File.Delete(Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"]));
                        });
                        logger.Error("Creazione batch interrotta funzione FillCacheDb fallita." );
                        return;
                    }

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

        protected void CreateMissingFiles(bool res, Batch m)
        {
            if (res)
                CreateDbCsv(m.DirectoryOutput);
        }

        protected bool CheckOutputDirFiles(string output_path)
        {
            bool res = !File.Exists(Path.Combine(output_path, ConfigurationManager.AppSettings["csv_file_name"]));
            return res;
        }

        protected void CreateDbCsv(string output_dir)
        {
            File.Create(Path.Combine(output_dir, ConfigurationManager.AppSettings["csv_file_name"]));
        }

        #endregion
    }
}
