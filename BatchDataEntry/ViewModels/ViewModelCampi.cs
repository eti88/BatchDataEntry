using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.DBModels;
using BatchDataEntry.Helpers;
using Campo = BatchDataEntry.Models.Campo;

namespace BatchDataEntry.ViewModels
{
    class ViewModelCampi : ViewModelBase
    {
        /*
         TODO: aggiungere un oggetto ponte in cui salvare le modifiche
         dei campi altrimenti vengono aggiornate in automatico nella lista
         da riportare anche le stesse modifiche nel vmApplicazione
         */

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
            Campo c = new Campo();
            this.Campi.Add(c);
            this.SelectedCampo = c;
        }

        private void DelItem()
        {
            #if DEBUG
                Console.WriteLine("DeleteCampo: " + SelectedCampo.ToString());
            #endif
            if (SelectedCampo.Id >= 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Campo tmp = new DBModels.Campo(SelectedCampo);
                db.DeleteRecord(tmp, tmp.Id);
                Campi.Remove(SelectedCampo);
            }
        }

        private void SaveCampo()
        {
            DatabaseHelper db = new DatabaseHelper();
            #if DEBUG
                Console.WriteLine("SaveCampo: " + SelectedCampo.ToString());
            #endif

            if (SelectedCampo.Id > 0)
            {
                DBModels.Campo tc = new DBModels.Campo(SelectedCampo);
                db.UpdateRecord(tc);
                RaisePropertyChanged("Campi");
            }
            else
            {
                DBModels.Campo tc = new DBModels.Campo(SelectedCampo);
                db.InsertRecord(tc);
                RaisePropertyChanged("Campi");
            }
        }

    }
}
