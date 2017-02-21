using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelApplicazione : ViewModelBase
    {
        private RelayCommand _addNewModel;

        private RelayCommand _deleteModel;

        private ObservableCollection<Modello> _models;

        private RelayCommand _openCampiView;

        private Modello _selectedModel;

        private RelayCommand _updateModel;

        private RelayCommand _buttonCopyCommand;

        public ViewModelApplicazione()
        {
            Modelli = new ObservableCollection<Modello>();
            LoadModels();
        }

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

        public ICommand addNewItemCommand
        {
            get
            {
                if (_addNewModel == null)
                    _addNewModel = new RelayCommand(param => AddNewModelItem());
                return _addNewModel;
            }
        }

        public ICommand ButtonCopyCommand
        {
            get
            {
                if(_buttonCopyCommand == null)
                    _buttonCopyCommand = new RelayCommand(param => CopyModel(), param => CanModify);
                return _buttonCopyCommand;
            }
        }

        public ICommand updateItemCommand
        {
            get
            {
                if (_updateModel == null)
                    _updateModel = new RelayCommand(param => ModifyItem(), param => CanModify);
                return _updateModel;
            }
        }

        public ICommand deleteItemCommand
        {
            get
            {
                if (_deleteModel == null)
                    _deleteModel = new RelayCommand(param => RemoveModelItem(), param => CanModify);
                return _deleteModel;
            }
        }

        public ICommand ButtonColonneCommand
        {
            get
            {
                if (_openCampiView == null)
                    _openCampiView = new RelayCommand(param => OpenCampiView(), param => CanModify);
                return _openCampiView;
            }
        }

        private bool CanModify
        {
            get { return SelectedModel != null; }
        }

        public void LoadModels()
        {
            var db = new DatabaseHelper();
            Modelli = db.GetModelloRecords();
            RaisePropertyChanged("Modelli");
        }

        private void AddNewModelItem()
        {
            var nuovoModello = new NuovoModello();
            var m = new Modello();
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
                var db = new DatabaseHelper();
                Modello tmp = new Modello(SelectedModel);
                Modelli.Remove(SelectedModel);
                db.Delete(@"Modello", String.Format("Id = {0}", tmp.Id));
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

        private void CopyModel()
        {
            if(SelectedModel == null)
                return;
            
            if(SelectedModel.Id == 0)
                return;

            var copy = new Modello(SelectedModel);
            copy.Id = 0;
            var db = new DatabaseHelper();
            copy.Nome = String.Format("{0} - Copia", copy.Nome);
            int newId = db.InsertRecordModello(copy);

            ObservableCollection<Campo>  _campi = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + SelectedModel.Id);

            foreach (var campo in _campi)
            {
                campo.IdModello = newId;
                campo.Id = 0;
                db.InsertRecordCampo(campo);
            }
            copy.Id = newId;
            Modelli.Add(copy);
            RaisePropertyChanged("Modelli");
        }
    }
}