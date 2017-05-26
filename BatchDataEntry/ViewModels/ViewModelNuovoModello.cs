using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Abstracts;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNuovoModello :ViewModelBase
    {
        #region Attr

        private AbsDbHelper db;
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

        // Commands

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

        #endregion

        #region Constructors

        public ViewModelNuovoModello()
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovoModello(AbsDbHelper dbs)
        {
            this.alreadyExist = false;
            db = dbs;
        }

        public ViewModelNuovoModello(AbsDbHelper dbs ,Modello modello, bool needUpdate)
        {
            this.SelectedModel = modello;
            this.alreadyExist = needUpdate;
            db = dbs;
        }

        #endregion

        public void AddNewItem()
        {
            Modello m = new Modello(SelectedModel);
            int lastId = -1;

            if (alreadyExist)
                    db.Update(m);             
            else
            {
                lastId = db.Insert(m);
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
