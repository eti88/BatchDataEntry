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

        public void fillCacheDb(DatabaseHelper db, Batch b)
        {
            List<string> sourceCsv = new List<string>();
            if (b.Applicazione.OrigineCsv)
            {
                sourceCsv = Csv.ReadRows(b.Applicazione.PathFileCsv);
            }

            List<string> files = new List<string>();
            if (b.TipoFile == TipoFileProcessato.Pdf)
            {
                files = Directory.GetFiles(b.DirectoryInput, "*.pdf").ToList();
            }
            else
            {
                files = Directory.GetDirectories(b.DirectoryInput).ToList();
            }

            if (files.Count == 0)
                return;

            MaxValue = files.Count;

            // adesso per ogni file in files aggiungere un record fileName, path, false
            for (int i = 0; i < files.Count; i++)
            {
                Document doc = new Document();
                doc.Id = i + 1;
                doc.FileName = Path.GetFileNameWithoutExtension(files[i]);
                doc.Path = files[i];
                doc.IsIndexed = false;

                if (b.Applicazione.OrigineCsv)
                {
                    string[] cells = sourceCsv.ElementAt(i).Split(b.Applicazione.Separatore[0]);
                    for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                    {
                        string colName = b.Applicazione.Campi.ElementAt(z).Nome;
                        string celValue = (!string.IsNullOrEmpty(cells[z])) ? cells[z] : "";
                        doc.Voci.Add(new Voce(colName, celValue));
                    }
                }
                else
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
                db.InsertRecordDocumento(b, doc);
                backgroundWorker.ReportProgress(i);
            }
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

                    dbc.CreateCacheDb(campi);
                    fillCacheDb(dbc, batch);
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
