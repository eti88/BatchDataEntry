﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Configuration;
using System.Windows;
using BatchDataEntry.Business;
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
                if (!string.IsNullOrEmpty(CurrentBatch.Nome) &&
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

            try
            {
                if (!File.Exists(Path.Combine(CurrentBatch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"])))
                {
                    if (CurrentBatch.Applicazione == null || CurrentBatch.Applicazione.Id == 0)
                        CurrentBatch.LoadModel();

                    if (CurrentBatch.Applicazione.Campi == null || CurrentBatch.Applicazione.Campi.Count == 0)
                        CurrentBatch.Applicazione.LoadCampi();    

                    List<string> campi = new List<string>();
                    for (int i = 0; i < CurrentBatch.Applicazione.Campi.Count; i++)
                    {
                        campi.Add(CurrentBatch.Applicazione.Campi[i].Nome);
                    }
                    
                    dbCache.CreateCacheDb(campi);
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
            IEnumerable<Modello> tmp = db.IEnumerableModelli();      
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

            if(files.Count == 0)
                return;

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
            }
            MessageBox.Show(@"Generazione File Temporanei Terminata");
        }
    }
}
