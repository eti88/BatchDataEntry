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
                _models = value;
                RaisePropertyChanged("Modelli");
            }
        }

        private Modello _selectedModel;
        public Modello SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("SelectedModel");
            }
        }

        private RelayCommand _addNewModel;
        public ICommand AddNewModelCmd
        {
            get
            {
                if(_addNewModel == null)
                    _addNewModel = new RelayCommand(param => this.AddNewModelItem());
                return _addNewModel;
            }
        }

        private RelayCommand _deleteModel;
        public ICommand DeleteModelCmd
        {
            get
            {
                if(_deleteModel == null)
                    _deleteModel = new RelayCommand(param => this.RemoveModelItem());
                return _deleteModel;
            }
        }

        private RelayCommand _salvaModel;
        public ICommand SaveModelModifyCmd
        {
            get
            {
                if(_salvaModel == null)
                    _salvaModel = new RelayCommand(param => this.SaveModelModified(), param => this.CanUpdate);
                return _salvaModel;
            }
        }

        private RelayCommand _openCampiView;
        public ICommand OpenCampiViewCmd
        {
            get
            {
                if (_openCampiView == null)
                    _openCampiView = new RelayCommand(param => this.OpenCampiView(), param => this.CanUpdate);
                return _openCampiView;
            }
        }

        private bool CanUpdate
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
            this.SelectedModel = new Modello();
        }

        private void RemoveModelItem()
        {
            #if DEBUG
                        Console.WriteLine("DeleteModello: " + SelectedModel.ToString());
            #endif
            if (SelectedModel.Id >= 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                db.DeleteRecord(SelectedModel, SelectedModel.Id);
                Modelli.Remove(SelectedModel);
            }
            
        }

        private void SaveModelModified()
        {
            DatabaseHelper db = new DatabaseHelper();
            #if DEBUG
                        Console.WriteLine("SaveModello: " + SelectedModel.ToString());
            #endif

            if (SelectedModel.Id > 0)
            {
                //Update
                DBModels.Modello mu = new DBModels.Modello(SelectedModel);
                db.UpdateRecord(mu);
                RaisePropertyChanged("Modelli");
            }
            else
            {
                //Insert
                DBModels.Modello mi = new DBModels.Modello(SelectedModel);
                db.InsertRecord(mi);
                RaisePropertyChanged("Modelli");
            }
        }

        private void OpenCampiView()
        {
            var campiView = new CampiV();
            campiView.DataContext = new ViewModelCampi(SelectedModel.Campi);
            campiView.ShowDialog();
        }
    }
}
