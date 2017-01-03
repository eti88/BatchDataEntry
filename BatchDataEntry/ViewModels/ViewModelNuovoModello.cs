using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNuovoModello :ViewModelBase
    {
        public ViewModelNuovoModello()
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovoModello(Models.Modello modello, bool needUpdate)
        {
            this.SelectedModel = modello;
            this.alreadyExist = needUpdate;
        }

        private bool alreadyExist = false;

        private Modello _selectedModel;
        public Modello SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("SelectedModel");
            }
        }

        private RelayCommand _addNew;
        public ICommand SaveModelCmd
        {
            get
            {
                if (_addNew == null)
                    _addNew = new RelayCommand(param => this.AddNewItem(), param => this.CanSave);
                return _addNew;
            }
        }
     
        private RelayCommand _restorevalues;
        public ICommand RestoreValuesCmd
        {
            get
            {
                if (_restorevalues == null)
                    _restorevalues = new RelayCommand(param => this.restoreValuesObj(), param => this.CanUse);
                return _restorevalues;
            }
        }

        private bool CanUse
        {
            get { return this.alreadyExist; }     
        }

        private bool CanSave
        {
            get
            {
                if (!string.IsNullOrEmpty(SelectedModel.Nome) && SelectedModel.Nome.Length > 2)
                    return true;
                else
                    return false;
            }
        }

        private void AddNewItem()
        {
            DatabaseHelper db = new DatabaseHelper();
            DBModels.Modello m = new DBModels.Modello(SelectedModel);
            int lastId = -1;

            if (alreadyExist)
                db.UpdateRecord(m);
            else
            {
                lastId = db.InsertRecord(m);
                SelectedModel.Id = lastId;
                RaisePropertyChanged("SelectedModel");
            }

            this.CloseWindow(true);
        }

        private void restoreValuesObj()
        {
            SelectedModel.Revert();
            RaisePropertyChanged("SelectedModel");
        }
    }
}
