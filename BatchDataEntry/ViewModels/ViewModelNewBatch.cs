using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Configuration;
using BatchDataEntry.DBModels;
using NLog;
using Batch = BatchDataEntry.Models.Batch;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNewBatch : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private bool _alreadyExist;

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

        private IEnumerable<DBModels.Modello> _models;
        public IEnumerable<DBModels.Modello> Models
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
                if (!string.IsNullOrEmpty(CurrentBatch.Nome) && CurrentBatch.TipoFile != null &&
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
        }

        public ViewModelNewBatch(Batch batch)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            PopulateComboboxModels();
        }

        private void AddOrUpdateBatchItem()
        {
            DatabaseHelper db = new DatabaseHelper();
            DatabaseHelper dbCache = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], CurrentBatch.DirectoryOutput);

            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update              
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.UpdateRecord(batch);
                RaisePropertyChanged("Batches");             
            }
            else
            {
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.InsertRecord(batch);
                RaisePropertyChanged("Batches");
            }

            try
            {
                if (!File.Exists(dbCache.PATHDB))
                {
                    dbCache.CreateCacheDb();
                    fillCacheDb(dbCache, CurrentBatch);
                }
                    
                if (CheckOutputDirFiles(CurrentBatch.DirectoryOutput))
                    CreateMissingFiles(true, CurrentBatch);
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine("Exception generazione cache db");
                Console.WriteLine(e.ToString());
                #endif
                logger.Error(e.ToString());
            }
            this.CloseWindow(true);
        }

        private void PopulateComboboxModels()
        {
            DatabaseHelper db = new DatabaseHelper();
            IEnumerable<DBModels.Modello> tmp = db.IEnumerableModelli();      
            Models = tmp;
            RaisePropertyChanged("Models");
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

        private void fillCacheDb(DatabaseHelper db, Batch b)
        {
            List<string> files = new List<string>();
            if (b.TipoFile == TipoFileProcessato.Pdf)
            {
                files = Directory.GetFiles(b.DirectoryInput, "*.pdf").ToList();
            }
            else
            {
                files = Directory.GetDirectories(b.DirectoryInput).ToList();
            }

            if(files.Count == 0)
                return;

            // adesso per ogni file in files aggiungere un record fileName, path, false
            for (int i = 0; i < files.Count; i++)
            {               
                Documento doc = new Documento();
                doc.Id = i + 1;
                doc.FileName = Path.GetFileNameWithoutExtension(files[i]);
                doc.Path = files[i];
                doc.isIndicizzato = false;
                db.InsertRecord(doc);
            }
        }
    }
}
