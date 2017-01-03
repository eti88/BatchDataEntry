using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Views;
using Campo = BatchDataEntry.Models.Campo;

namespace BatchDataEntry.ViewModels
{
    class ViewModelCampi : ViewModelBase
    {
        public ViewModelCampi()
        {
            this.Colonne = new ObservableCollection<Campo>();
        }

        public ViewModelCampi(int idModello)
        {
            DatabaseHelper db = new DatabaseHelper();
            this.Colonne = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + idModello);
            RaisePropertyChanged("Colonne");
            this._idModello = idModello;
        }

        private int _countCols
        {
            get { return Colonne.Count; }
        }
        private readonly int _idModello;

        private ObservableCollection<Campo> _clos;
        public ObservableCollection<Campo> Colonne
        {
            get { return _clos; }
            set
            {
                _clos = value;
                RaisePropertyChanged("Colonne");
            }
        }

        private Campo _selectedCampo;
        public Campo SelectedCampo
        {
            get { return _selectedCampo; }
            set
            {
                _selectedCampo = value;
                RaisePropertyChanged("SelectedCampo");
            }
        }

        private RelayCommand _addnew;
        public ICommand addNewItemCommand
        {
            get
            {
                if (_addnew == null)
                {
                    _addnew = new RelayCommand(param => this.AddItem());
                }
                return _addnew;
            }
        }

        private RelayCommand _delitem;
        public ICommand DelCampoCmd
        {
            get
            {
                if (_delitem == null)
                {
                    _delitem = new RelayCommand(param => this.DelItem(), param => this.CanDel);
                }
                return _delitem;
            }
        }

        private RelayCommand _update;
        public ICommand updateItemCommand
        {
            get
            {
                if (_update == null)
                {
                    _update = new RelayCommand(param => this.UpdateItem(), param => this.CanDel);
                }
                return _update;
            }
        }

        private bool CanDel
        {
            get { return (SelectedCampo == null) ? false : true; }
        }

        private void AddItem()
        {
            var colonna = new NuovaColonna();
            Campo campo = new Campo();
            campo.IdModello = this._idModello;
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
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Campo tmp = new DBModels.Campo(SelectedCampo);
                Colonne.Remove(SelectedCampo);
                db.DeleteRecord(tmp, tmp.Id);
                RaisePropertyChanged("Colonne");
            }
        }

        private void UpdateItem()
        {
            var colonna = new NuovaColonna();
            colonna.DataContext = new ViewModelNuovaColonna(SelectedCampo, true);
            colonna.ShowDialog();
            RaisePropertyChanged("Colonne");
        }
    }
}
