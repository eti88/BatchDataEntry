using System;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Collections.Generic;
using BatchDataEntry.Abstracts;
using System.Linq;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNuovaColonna : ViewModelBase
    {
        private AbsDbHelper db;      
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

        private string _selectedAutoTab;
        public string SelectedAutoTable
        {
            get { return _selectedAutoTab; }
            set
            {
                if (_selectedAutoTab != value)
                {
                    _selectedAutoTab = value;
                    RaisePropertyChanged("SelectedAutoTable");
                    TableColumnsDictionary = PopulateCmbTabColonne(db, SelectedAutoTable);
                    if (!string.IsNullOrEmpty(SelectedAutoTable))
                        SelectedCampo.TabellaSorgente = SelectedAutoTable;
                    SelectedTableColumn = new KeyValuePair<string, int>();
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

        private Dictionary<string, int> _tableColumnsDictionary;
        public Dictionary<string, int> TableColumnsDictionary
        {
            get { return _tableColumnsDictionary; }
            set
            {
                if (_tableColumnsDictionary != value)
                {
                    _tableColumnsDictionary = value;
                    RaisePropertyChanged("TableColumnsDictionary");
                    SelectedTableColumn = new KeyValuePair<string, int>();
                }
            }
        }

        private KeyValuePair<string, int> _selectedTableColumn;
        public KeyValuePair<string, int> SelectedTableColumn
        {
            get { return _selectedTableColumn; }
            set
            {
                if (_selectedTableColumn.Value != value.Value)
                {
                    _selectedTableColumn = value;
                    RaisePropertyChanged("SelectedTableColumn");
                    SelectedCampo.SourceTableColumn = _selectedTableColumn.Value;
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

        public ViewModelNuovaColonna(AbsDbHelper db)
        {
            this.alreadyExist = false;
            if(db is DatabaseHelperSqlServer)
                SrcTables = ((DatabaseHelperSqlServer)db).GetTableList();
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, AbsDbHelper dbq)
        {
            Modello mod;
            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;
            db = dbq;
            mod = db.GetModelloById(c.IdModello);
            if (db is DatabaseHelperSqlServer)
                SrcTables = ((DatabaseHelperSqlServer)db).GetTableList();
            else
                SrcTables = new List<string>();

            this.NomeTabella = mod.Nome;
            SelectedAutoTable = SelectedCampo.TabellaSorgente;
            TableColumnsDictionary = PopulateCmbTabColonne(db, SelectedAutoTable);
            SelectedTableColumn = TableColumnsDictionary.FirstOrDefault(p => p.Value == SelectedCampo.SourceTableColumn);
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, int colCount, AbsDbHelper dbq)
        {
            Modello mod;

            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;
            db = dbq;
            mod = db.GetModelloById(c.IdModello);
            if (db is DatabaseHelperSqlServer)
                SrcTables = ((DatabaseHelperSqlServer)db).GetTableList();
            else
                SrcTables = new List<string>();

            this.SelectedCampo.Posizione = colCount++;
            this.NomeTabella = mod.Nome;
            SelectedAutoTable = SelectedCampo.TabellaSorgente;
            TableColumnsDictionary = PopulateCmbTabColonne(db, SelectedAutoTable);
            SelectedTableColumn = TableColumnsDictionary.FirstOrDefault(p => p.Value == SelectedCampo.SourceTableColumn);
        }

        #endregion

        private Dictionary<string, int> PopulateCmbTabColonne(AbsDbHelper dbc, string table)
        {
            var res = new Dictionary<string, int>();
            
            if (dbc != null && dbc is DatabaseHelperSqlServer && !string.IsNullOrEmpty(table))
            {
                res = ((DatabaseHelperSqlServer)dbc).GetColumns(table);
            }
            return res;
        }

        public void AddNewItem()
        {
            Campo m = new Campo(SelectedCampo);
            int lastId = -1;

            if (m.TipoCampo == EnumTypeOfCampo.AutocompletamentoLocalita)
                m.SourceTableColumn = 1;

            if(alreadyExist)
                db.Update(m);
            else if(!alreadyExist)
                lastId = db.Insert(m);
 
            if (lastId != -1)
            {
                SelectedCampo.Id = lastId;
            }
            RaisePropertyChanged("SelectedCampo");
            this.CloseWindow(true);
        }
    }
}
