using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNuovaColonna : ViewModelBase
    {
        private DatabaseHelperSqlServer dbsql;

        public ViewModelNuovaColonna()
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovaColonna(DatabaseHelperSqlServer db)
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, DatabaseHelperSqlServer dbq)
        {
            DatabaseHelper dblite;
            Modello mod;
            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;
            if (dbq != null)
            {
                dbsql = dbq;
                mod = dbsql.GetModelloById(c.IdModello);
            }
            else
            {
                dblite = new DatabaseHelper();
                mod = dblite.GetModelloById(c.IdModello);
            } 
            this.NomeTabella = mod.Nome;
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, int colCount, DatabaseHelperSqlServer dbq)
        {
            DatabaseHelper dblite;
            Modello mod;

            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;
            if(dbq != null)
            {
                dbsql = dbq;
                mod = dbsql.GetModelloById(c.IdModello);
            }
            else
            {
                dblite = new DatabaseHelper();
                mod = dblite.GetModelloById(c.IdModello);
            }
            this.SelectedCampo.Posizione = colCount++;
            this.NomeTabella = mod.Nome;
        }

        private bool alreadyExist = false;

        private Campo _selectCampo;
        public Campo SelectedCampo
        {
            get { return _selectCampo; }
            set
            {
                if (_selectCampo != value)
                {
                    _selectCampo = value;
                    RaisePropertyChanged("SelectedCampo");
                }
            }
        }

        private string _nometab;
        public string NomeTabella
        {
            get { return _nometab; }
            set
            {
                if (_nometab != value)
                {
                    _nometab = value;
                    RaisePropertyChanged("NomeTabella");
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

        private bool CanUse
        {
            get { return this.alreadyExist; }
        }

        private bool CanSave
        {
            get
            {
                if (!string.IsNullOrEmpty(SelectedCampo.Nome) && SelectedCampo.Nome.Length > 2)
                    return true;
                else
                    return false;
            }
        }

        public void AddNewItem()
        {
            DatabaseHelper dbsqlite = new DatabaseHelper();
            Campo m = new Campo(SelectedCampo);
            int lastId = -1;

            if(dbsql == null && alreadyExist)
                dbsqlite.UpdateRecordCampo(m);
            else if(dbsql == null && !alreadyExist)
                lastId = dbsqlite.InsertRecordCampo(m);
            else if (alreadyExist)
                dbsql.Update(m);
            else
                lastId = dbsql.Insert(m);

            if (lastId != -1)
            {
                SelectedCampo.Id = lastId;
            }
            RaisePropertyChanged("SelectedCampo");
            this.CloseWindow(true);
        }
    }
}
