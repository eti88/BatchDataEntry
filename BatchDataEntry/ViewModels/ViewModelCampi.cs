using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelCampi : ViewModelBase
    {
        private readonly int _idModello;

        private RelayCommand _addnew;

        private ObservableCollection<Campo> _clos;

        private RelayCommand _delitem;

        private Campo _selectedCampo;

        private RelayCommand _update;

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
                RaisePropertyChanged("SelectedCampo");
            }
        }

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

        public ViewModelCampi()
        {
            Colonne = new ObservableCollection<Campo>();
        }

        public ViewModelCampi(int idModello)
        {           
            GetColonneFromDb(idModello);
            _idModello = idModello;
        }

        private void GetColonneFromDb(int idModello)
        {
            var db = new DatabaseHelper();
            this.Colonne = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + idModello);
            RaisePropertyChanged("Colonne");
        }

        private void AddItem()
        {
            var colonna = new NuovaColonna();
            var campo = new Campo();
            campo.IdModello = _idModello;
            colonna.DataContext = new ViewModelNuovaColonna(campo, false, _countCols);
            var result = colonna.ShowDialog();
            if (result == true)
            {
                Colonne.Add(campo);
            }
            RaisePropertyChanged("Colonne");
        }

        private void DelItem()
        {
            if (SelectedCampo != null && SelectedCampo.Id >= 0)
            {
                var db = new DatabaseHelper();
                Campo tmp = new Campo(SelectedCampo);
                Colonne.Remove(SelectedCampo);
                db.Delete(@"Campo", String.Format("Id = {0}", tmp.Id));
                RaisePropertyChanged("Colonne");
            }
        }

        private void UpdateItem()
        {
            var colonna = new NuovaColonna();
            colonna.DataContext = new ViewModelNuovaColonna(SelectedCampo, true);
            colonna.ShowDialog();
            SelectedCampo = null;
            GetColonneFromDb(_idModello);
            RaisePropertyChanged("Colonne");
        }
    }
}