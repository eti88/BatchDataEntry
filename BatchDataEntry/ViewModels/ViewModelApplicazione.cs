using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;

namespace BatchDataEntry.ViewModels
{
    class ViewModelApplicazione : ViewModelBase
    {
        public ViewModelApplicazione()
        {
            this.Modelli = new ObservableCollection<Modello>();
            LoadModels();
        }

        private ObservableCollection<Modello> _models;
        public ObservableCollection<Modello> Modelli
        {
            get { return _models; }
            set
            {
                if (_models != value)
                {
                    _models = value;
                    RaisePropertyChanged("Modelli");
                }
            }
        }

        private Modello _selectedModel;
        public Modello SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                if (_selectedModel != value)
                {
                    _selectedModel = value;
                    RaisePropertyChanged("SelectedModel");
                }
            }
        }

        private RelayCommand _addNewModel;
        public ICommand addNewItemCommand
        {
            get
            {
                if(_addNewModel == null)
                    _addNewModel = new RelayCommand(param => this.AddNewModelItem());
                return _addNewModel;
            }
        }

        private RelayCommand _updateModel;
        public ICommand updateItemCommand
        {
            get
            {
                if (_updateModel == null)
                    _updateModel = new RelayCommand(param => this.ModifyItem(), param => this.CanModify);
                return _updateModel;
            }
        }

        private RelayCommand _deleteModel;
        public ICommand deleteItemCommand
        {
            get
            {
                if(_deleteModel == null)
                    _deleteModel = new RelayCommand(param => this.RemoveModelItem(), param => this.CanModify);
                return _deleteModel;
            }
        }

        private RelayCommand _openCampiView;
        public ICommand ButtonColonneCommand
        {
            get
            {
                if (_openCampiView == null)
                    _openCampiView = new RelayCommand(param => this.OpenCampiView(), param => this.CanModify);
                return _openCampiView;
            }
        }

        private bool CanModify
        {
            get { return (SelectedModel == null) ? false : true; }
        }

        public void LoadModels()
        {
            DatabaseHelper db = new DatabaseHelper();
            Modelli = db.GetModelloRecords();
            RaisePropertyChanged("Modelli");
        }

        private void AddNewModelItem()
        {
            var nuovoModello = new NuovoModello();       
            Modello m = new Modello();
            nuovoModello.DataContext = new ViewModelNuovoModello(m, false);
            var res = nuovoModello.ShowDialog();
            if (res == true)
            {
                Modelli.Add(m);
            }
            RaisePropertyChanged("Modelli");         
        }

        private void RemoveModelItem()
        {
            if (SelectedModel != null && SelectedModel.Id >= 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Modello tmp = new DBModels.Modello(SelectedModel);
                Modelli.Remove(SelectedModel);
                db.DeleteRecord(tmp, tmp.Id);
                RaisePropertyChanged("Modelli");
            }
        }

        private void ModifyItem()
        {
            var nuovoModello = new NuovoModello();
            nuovoModello.DataContext = new ViewModelNuovoModello(SelectedModel, true);
            nuovoModello.ShowDialog();
            RaisePropertyChanged("Modelli");
        }

        private void OpenCampiView()
        {
            if (SelectedModel != null)
            {
                var campiView = new CampiV();
                campiView.DataContext = new ViewModelCampi(SelectedModel.Id);
                campiView.ShowDialog();
            }          
        }
    }
}
