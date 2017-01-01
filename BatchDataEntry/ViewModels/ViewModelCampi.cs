using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelCampi : ViewModelBase
    {
        public ViewModelCampi()
        {
            Campi = new ObservableCollection<Campo>();
        }

        public ViewModelCampi(ObservableCollection<Campo> campi)
        {
            this.Campi = campi;
            RaisePropertyChanged("Campi");
        }

        private ObservableCollection<Campo> _campi;
        public ObservableCollection<Campo> Campi
        {
            get { return _campi; }
            set
            {
                _campi = value;
                RaisePropertyChanged("Campi");
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
        public ICommand AddNewCampoCmd
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

        private RelayCommand _save;
        public ICommand SalvaCampoCmd
        {
            get
            {
                if (_save == null)
                {
                    _save = new RelayCommand(param => this.SaveCampo(), param => this.CanSave);
                }
                return _save;
            }
        }

        private bool CanDel
        {
            get { return (SelectedCampo == null) ? false : true; }
        }

        private bool CanSave
        {
            get
            {
                if (SelectedCampo == null)
                    return false;
                else if (!string.IsNullOrEmpty(SelectedCampo.Nome) && SelectedCampo.Posizione >= 0)
                    return true;
                else
                    return false;
            }
        }

        private void AddItem()
        {
        }

        private void DelItem()
        {
        }

        private void SaveCampo() { }

    }
}
