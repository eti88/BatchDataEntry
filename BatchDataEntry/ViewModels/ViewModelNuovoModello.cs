using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNuovoModello :ViewModelBase
    {
        #region Attr

        private DatabaseHelperSqlServer dbsql;
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

        #endregion

        #region Constructors

        public ViewModelNuovoModello()
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovoModello(DatabaseHelperSqlServer dbs)
        {
            this.alreadyExist = false;
            dbsql = dbs;
        }

        public ViewModelNuovoModello(Modello modello, bool needUpdate)
        {
            this.SelectedModel = modello;
            this.alreadyExist = needUpdate;
        }

        public ViewModelNuovoModello(DatabaseHelperSqlServer dbs ,Modello modello, bool needUpdate)
        {
            this.SelectedModel = modello;
            this.alreadyExist = needUpdate;
            dbsql = dbs;
        }

        #endregion

        public void AddNewItem()
        {
            DatabaseHelper db = null;
            if(dbsql == null) 
                db = new DatabaseHelper();

            Modello m = new Modello(SelectedModel);
            int lastId = -1;

            if (alreadyExist)
            {
                if (dbsql == null)
                    db.UpdateRecordModello(m);
                else
                    dbsql.Update(m);
            }               
            else
            {
                if (dbsql == null)
                    lastId = db.InsertRecordModello(m);
                else
                    lastId = dbsql.Insert(m);

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
