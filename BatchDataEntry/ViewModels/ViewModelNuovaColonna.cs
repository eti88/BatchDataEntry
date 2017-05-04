using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Collections.Generic;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNuovaColonna : ViewModelBase
    {
        private DatabaseHelperSqlServer dbsql;
        private bool alreadyExist = false;

        private List<string> _srcTables;
        public List<string> SrcTables
        {
            get { return _srcTables; }
            set
            {
                if(_srcTables != value)
                {
                    _srcTables = value;
                    RaisePropertyChanged("SrcTables");
                }
            }
        }

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

        #region Constructors

        public ViewModelNuovaColonna()
        {
            this.alreadyExist = false;
            SrcTables = new List<string>();
        }

        public ViewModelNuovaColonna(DatabaseHelperSqlServer db)
        {
            this.alreadyExist = false;
            SrcTables = db.GetTableList();
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
                SrcTables = dbsql.GetTableList();
            }
            else
            {
                dblite = new DatabaseHelper();
                mod = dblite.GetModelloById(c.IdModello);
                SrcTables = new List<string>();
            }
            this.NomeTabella = mod.Nome;                      
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, int colCount, DatabaseHelperSqlServer dbq)
        {
            DatabaseHelper dblite;
            Modello mod;

            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;
            if (dbq != null)
            {
                dbsql = dbq;
                mod = dbsql.GetModelloById(c.IdModello);
                SrcTables = dbsql.GetTableList();
            }
            else
            {
                dblite = new DatabaseHelper();
                mod = dblite.GetModelloById(c.IdModello);
                SrcTables = new List<string>();
            }
            this.SelectedCampo.Posizione = colCount++;
            this.NomeTabella = mod.Nome;            
        }

        #endregion

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
