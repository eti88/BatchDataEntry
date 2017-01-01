﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNewBatch : ViewModelBase
    {
        private bool _alreadyExist;

        private Batch _currentBatch;
        public Batch CurrentBatch
        {
            get { return _currentBatch; }
            set
            {
                _currentBatch = value;
                RaisePropertyChanged("CurrentBatch");
            }
        }

        private IEnumerable<DBModels.Modello> _models;
        public IEnumerable<DBModels.Modello> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                RaisePropertyChanged("Models");
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
            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.UpdateRecord(batch);
                RaisePropertyChanged("Batches");
                this.CloseWindow(true);
            }
            else
            {
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.InsertRecord(batch);
                RaisePropertyChanged("Batches");
                this.CloseWindow(true);
            }         
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
