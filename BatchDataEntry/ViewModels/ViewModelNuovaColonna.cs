using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNuovaColonna : ViewModelBase
    {
        public ViewModelNuovaColonna()
        {
            this.alreadyExist = false;
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate)
        {
            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;

            DatabaseHelper db = new DatabaseHelper();
            Modello mod = db.GetModelloById(c.IdModello);
            this.NomeTabella = mod.Nome;
        }

        public ViewModelNuovaColonna(Campo c, bool needUpdate, int colCount)
        {
            this.SelectedCampo = c;
            this.alreadyExist = needUpdate;

            DatabaseHelper db = new DatabaseHelper();
            Modello mod = db.GetModelloById(c.IdModello);
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
                _selectCampo = value;
                RaisePropertyChanged("SelectedCampo");
            }
        }

        private string _nometab;
        public string NomeTabella
        {
            get { return _nometab; }
            set
            {
                _nometab = value;
                RaisePropertyChanged("NomeTabella");
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

        private void AddNewItem()
        {
            DatabaseHelper db = new DatabaseHelper();
            DBModels.Campo m = new DBModels.Campo(SelectedCampo);
            int lastId = -1;

            if (alreadyExist)
                db.UpdateRecord(m);
            else
            {
                lastId = db.InsertRecord(m);
                SelectedCampo.Id = lastId;
                RaisePropertyChanged("SelectedCampo");
            }

            this.CloseWindow(true);
        }

        private void restoreValuesObj()
        {
            SelectedCampo.Revert();
            RaisePropertyChanged("SelectedCampo");
        }
    }
}
