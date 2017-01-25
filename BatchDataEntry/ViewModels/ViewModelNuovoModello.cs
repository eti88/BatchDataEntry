using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

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
                if (_selectedModel != value)
                {
                    _selectedModel = value;
                    RaisePropertyChanged("SelectedModel");
                }
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

        private bool CanUse => this.alreadyExist;

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
            Modello m = new Modello(SelectedModel);
            int lastId = -1;

            if (alreadyExist)
                db.UpdateRecordModello(m);
            else
            {
                lastId = db.InsertRecordModello(m);
                if(lastId == -1)
                    return;
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
