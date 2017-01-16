using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using MadMilkman.Ini;
using System.Configuration;
using NLog;

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
                    dbCache.CreateCacheDb();

                if (CurrentBatch.Applicazione.Campi == null)
                    CurrentBatch.Applicazione.LoadCampi();

                if (CurrentBatch.Applicazione.Campi.Count > 0)
                    dbCache.GenerateCacheTable(CurrentBatch.Applicazione.Campi);
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
        
    }
}
