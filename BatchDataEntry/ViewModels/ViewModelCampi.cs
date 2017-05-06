using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using BatchDataEntry.Abstracts;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelCampi : ViewModelBase
    {
#region Attr
        private readonly int _idModello;
        private Campo _intermediate;
        protected AbsDbHelper db;
      
        private ObservableCollection<Campo> _clos;
        private Campo _selectedCampo;
        private int _countCols
        {
            get { return Colonne.Count; }
        }
        public ObservableCollection<Campo> Colonne
        {
            get { return _clos; }
            set
            {
                if (_clos != value)
                {
                    _clos = value;
                    RaisePropertyChanged("Colonne");
                }
            }
        }
        public Campo SelectedCampo
        {
            get { return _selectedCampo; }
            set
            {
                _selectedCampo = value;
                _intermediate = new Campo(SelectedCampo);
                RaisePropertyChanged("SelectedCampo");
            }
        }

        private RelayCommand _update;
        private RelayCommand _delitem;
        private RelayCommand _addnew;
        public ICommand addNewItemCommand
        {
            get
            {
                if (_addnew == null)
                {
                    _addnew = new RelayCommand(param => AddItem());
                }
                return _addnew;
            }
        }
        public ICommand DelCampoCmd
        {
            get
            {
                if (_delitem == null)
                {
                    _delitem = new RelayCommand(param => DelItem(), param => CanDel);
                }
                return _delitem;
            }
        }
        public ICommand updateItemCommand
        {
            get
            {
                if (_update == null)
                {
                    _update = new RelayCommand(param => UpdateItem(), param => CanDel);
                }
                return _update;
            }
        }
        private bool CanDel
        {
            get { return SelectedCampo == null ? false : true; }
        }

        #endregion

        public ViewModelCampi()
        {
            Colonne = new ObservableCollection<Campo>();
        }

        public ViewModelCampi(AbsDbHelper dbc)
        {
            Colonne = new ObservableCollection<Campo>();
            db = dbc;
        }

        public ViewModelCampi(AbsDbHelper dbc, int idModello)
        {
            db = dbc;
            GetColonneFromDb(db, idModello);
            _idModello = idModello;
        }

        public void GetColonneFromDb(AbsDbHelper db, int idModello)
        {
            if (this.Colonne == null) this.Colonne = new ObservableCollection<Campo>();
            if (this.Colonne.Count > 0) this.Colonne.Clear();
            this.Colonne = db.CampoQuery("SELECT * FROM Campi WHERE IdModello = " + idModello);
            RaisePropertyChanged("Colonne");
        }

        private void AddItem()
        {
            var colonna = new NuovaColonna();
            var campo = new Campo();
            campo.IdModello = _idModello;
            colonna.DataContext = new ViewModelNuovaColonna(campo, false, _countCols, db);
            var result = colonna.ShowDialog();
            if (result == true)
                Colonne.Add(campo);
            RaisePropertyChanged("Colonne");
        }

        public void DelItem()
        {
            if (SelectedCampo != null && SelectedCampo.Id >= 0)
            {
                Campo tmp = new Campo(SelectedCampo);
                db.DeleteFromTable(@"Campo", String.Format("Id = {0}", tmp.Id));

                Colonne.Remove(SelectedCampo); 
                RaisePropertyChanged("Colonne");
            }
        }

        private void UpdateItem()
        {
            var colonna = new NuovaColonna();
            colonna.DataContext = new ViewModelNuovaColonna(_intermediate, true, db);
            colonna.ShowDialog();
            GetColonneFromDb(db ,_idModello);
            RaisePropertyChanged("Colonne");
        }
    }
}