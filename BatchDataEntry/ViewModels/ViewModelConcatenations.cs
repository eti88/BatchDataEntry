using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelConcatenations : ViewModelBase
    {
        private DatabaseHelperSqlServer db;

        private bool CanEdit
        {
            get { return (SelectedConcat != null); }
        }
        private int currentmodel;
        private Concatenation _intermediate;
        private Dictionary<string, object> _vanillaCampi;

        private ObservableCollection<Concatenation> _concat;
        public ObservableCollection<Concatenation> Concatenazioni
        {
            get { return _concat; }
            set
            {
                if (_concat != value)
                {
                    _concat = value;
                    RaisePropertyChanged("Concatenazioni");
                }
            }
        }

        private Concatenation _selectedConcat;
        public Concatenation SelectedConcat
        {
            get { return _intermediate; }
            set
            {
                if (_selectedConcat != value)
                {
                    _selectedConcat = value;
                    _intermediate = new Concatenation(value);
                    AllCampi = new Dictionary<string, object>(_vanillaCampi);
                    RaisePropertyChanged("SelectedConcat");
                }
            }
        }

        private Dictionary<string, object> _campi;
        public Dictionary<string, object> AllCampi
        {
            get { return _campi; }
            set
            {
                if (_campi != value)
                {
                    _campi = value;
                    RaisePropertyChanged("AllCampi");
                    RaisePropertyChanged("SelectedConcat");
                }
            }
        }

        private bool isNew = false;

        public ViewModelConcatenations() {
            Concatenazioni = new ObservableCollection<Concatenation>();

            AllCampi = new Dictionary<string, object>();
        }

        public ViewModelConcatenations(DatabaseHelperSqlServer _db, int idmodello)
        {
            db = _db;
            Concatenazioni = db.LoadConcatenations(idmodello);
            GetAllCampi(idmodello);
            _vanillaCampi = new Dictionary<string, object>(AllCampi);
            currentmodel = idmodello;
        }

        /// <summary>
        /// Recuperat tutti i campi del modello contrassegnati con il tipo dbsql
        /// </summary>
        public void GetAllCampi(int idmod)
        {
            if (db == null) return;
            string query = string.Format("SELECT * FROM Campo WHERE IdModello = {0} AND TipoCampo = {1}", idmod, (int)EnumTypeOfCampo.AutocompletamentoDbSql);
            var tmplist = db.CampoQuery(query);
            if (tmplist == null) return;

            if (AllCampi == null) AllCampi = new Dictionary<string, object>();

            foreach(Campo c in tmplist)
            {
                AllCampi.Add(c.Nome, new Campo(c));
            }
        }

        private RelayCommand _addnew;
        public ICommand AddNewItemCommand
        {
            get
            {
                if (_addnew == null)
                    _addnew = new RelayCommand(param => AddNewConcatItem());
                return _addnew;
            }
        }

        private RelayCommand _cmdmod;
        public ICommand ModifyCommand
        {
            get
            {
                if (_cmdmod == null)
                    _cmdmod = new RelayCommand(param => ModifyItem(), param => this.CanEdit);
                return _cmdmod;
            }
        }

        private RelayCommand _del;
        public ICommand DeleteCommand
        {
            get
            {
                if (_del == null)
                    _del = new RelayCommand(param => DeleteItem(), param => this.CanEdit);
                return _del;
            }
        }

        public void AddNewConcatItem()
        {
            if (Concatenazioni == null)
                Concatenazioni = new ObservableCollection<Concatenation>();

            Concatenazioni.Add(new Concatenation());
            isNew = true;
        }

        public void ModifyItem()
        {
            if(CanEdit)
            {
                _intermediate.Modello = currentmodel;
                int tmpid = -1;
                if(isNew)
                {
                    tmpid = db.Insert(_intermediate);
                    if (tmpid == -1) return;
                    SelectedConcat.Id = tmpid;
                    RaisePropertyChanged("SelectedConcat");
                    isNew = false;
                }
                else
                {
                    db.Update(_intermediate);
                }
            }
        }

        public void DeleteItem()
        {
            if (CanEdit)
            {
                db.DeleteFromTable(@"Concatenazioni", String.Format("Id = {0}", _intermediate.Id));
                Concatenazioni.Remove(SelectedConcat);
                RaisePropertyChanged("Concatenazioni");
            }
        }
    }
}
