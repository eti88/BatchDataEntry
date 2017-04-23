using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelApplicazione : ViewModelBase
    {
        private DatabaseHelperSqlServer dbsql;
        private Modello _intermediate;
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

        public ViewModelApplicazione(DatabaseHelperSqlServer db)
        {
            Modelli = new ObservableCollection<Modello>();
            dbsql = db;
            LoadModels(db);
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
                    _intermediate = new Modello(_selectedModel);
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
            if(Modelli == null) Modelli = new ObservableCollection<Modello>();
            if(Modelli.Count > 0) Modelli.Clear();
            Modelli = db.GetModelloRecords();
            RaisePropertyChanged("Modelli");
        }

        public void LoadModels(DatabaseHelperSqlServer db)
        {
            if (Modelli == null) Modelli = new ObservableCollection<Modello>();
            if (Modelli.Count > 0) Modelli.Clear();
            Modelli = db.GetModelloRecords();
            RaisePropertyChanged("Modelli");
        }

        private void AddNewModelItem()
        {
            var nuovoModello = new NuovoModello();
            var m = new Modello();
            if(dbsql == null)
                nuovoModello.DataContext = new ViewModelNuovoModello(m, false);
            else
                nuovoModello.DataContext = new ViewModelNuovoModello(dbsql, m, false);
            var res = nuovoModello.ShowDialog();
            if (res == true)
            {
                Modelli.Add(m);
            }
            RaisePropertyChanged("Modelli");
        }

        public void RemoveModelItem()
        {
            if (SelectedModel != null && SelectedModel.Id >= 0)
            {
                Modello tmp = new Modello(SelectedModel);
                if (dbsql == null)
                {
                    var db = new DatabaseHelper();
                    db.Delete(@"Modello", String.Format("Id = {0}", tmp.Id));
                }
                else
                {
                    dbsql.DeleteReference(string.Format(@"DELETE FROM Campi WHERE IdModello = {0}", tmp.Id));
                    dbsql.DeleteReference(string.Format(@"DELETE FROM Batch WHERE IdModello = {0}", tmp.Id));
                    dbsql.DeleteFromTable(@"Modelli", String.Format("Id = {0}", tmp.Id));
                }
                Modelli.Remove(SelectedModel);
                RaisePropertyChanged("Modelli");
            }
        }
        
        private void ModifyItem()
        {
            var nuovoModello = new NuovoModello();
            if (dbsql == null)
                nuovoModello.DataContext = new ViewModelNuovoModello(_intermediate, true);
            else
                nuovoModello.DataContext = new ViewModelNuovoModello(dbsql, _intermediate, true);
            nuovoModello.ShowDialog();
            if (dbsql == null)
                LoadModels();
            else
                LoadModels(dbsql);
            RaisePropertyChanged("Modelli");
        }

        private void OpenCampiView()
        {
            if (SelectedModel != null)
            {
                var campiView = new CampiV();
                campiView.DataContext = new ViewModelCampi(dbsql, SelectedModel.Id);
                campiView.ShowDialog();
            }
        }

        public void CopyModel()
        {
            if(SelectedModel == null)
                return;
            
            if(SelectedModel.Id == 0)
                return;

            var copy = new Modello(SelectedModel);
            copy.Id = 0;
            var db = new DatabaseHelper();
            copy.Nome = String.Format("{0} - Copia", copy.Nome);
            int newId;
            ObservableCollection<Campo> _campi;
            if (dbsql == null)
            {
                newId = db.InsertRecordModello(copy);
                _campi = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + SelectedModel.Id);
            }
            else
            {
                newId = dbsql.Insert(copy);
                _campi = dbsql.CampoQuery("SELECT * FROM Campi WHERE IdModello = " + SelectedModel.Id);
            }

            if (_campi == null) throw new Exception("Retrive Data (_campi) for copy model failed!");
            foreach (var campo in _campi)
            {
                campo.IdModello = newId;
                campo.Id = 0;
                if (dbsql == null)
                    db.InsertRecordCampo(campo);
                else
                    dbsql.Insert(campo);
            }
            copy.Id = newId;
            Modelli.Add(copy);
            RaisePropertyChanged("Modelli");
        }
    }
}