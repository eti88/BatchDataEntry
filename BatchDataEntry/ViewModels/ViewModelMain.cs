using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using NLog;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelMain : ViewModelBase
    {
#region Attribute
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Batch _intermediate;
        private DatabaseHelperSqlServer dbsql;

        private ObservableCollection<Batch> _Batches { get; set; }
        public ObservableCollection<Batch> Batches
        {
            get
            {
                return _Batches;
            }
            set
            {
                if (_Batches != value)
                {
                    _Batches = value;
                    RaisePropertyChanged("Batches");
                }
                
            }
        }   

        private Batch _SelectedBatch;
        public Batch SelectedBatch
        {
            get { return _SelectedBatch; }
            set
            {
                if (_SelectedBatch != value)
                {
                    _SelectedBatch = value;
                    _intermediate = new Batch(_SelectedBatch);
                    RaisePropertyChanged("SelectedBatch");
                }
            }
        }

        private RelayCommand _newCommand;
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                {
                    _newCommand = new RelayCommand(param => this.NewBatchItem());
                }
                return _newCommand;
            }
        }

        private RelayCommand _modifyCommand;
        public ICommand ModifyCommand
        {
            get
            {
                if (_modifyCommand == null)
                {
                    _modifyCommand = new RelayCommand(param => this.ModifyBatchItem(), param => this.CanEdit);
                }
                return _modifyCommand;
            }
        }

        private RelayCommand _resumeCommand;
        public ICommand ResumeCommand
        {
            get
            {
                if (_resumeCommand == null)
                {
                    _resumeCommand = new RelayCommand(param => this.ResumeBatchItem(), param => this.CanEdit);
                }
                return _resumeCommand;
            }
        }

        private RelayCommand _modelAddCommand;
        public ICommand ModelAddCommand
        {
            get
            {
                if (_modelAddCommand == null)
                {
                    _modelAddCommand = new RelayCommand(param => this.ModelAddItem());
                }
                return _modelAddCommand;
            }
        }

        private RelayCommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get {
                if (_deleteCommand == null)
                {
                        _deleteCommand = new RelayCommand(param => this.DeleteBatchItem(), param => this.CanEdit);
                }
                return _deleteCommand;
            }
        }

        private RelayCommand _aboutWindowCmd;
        public ICommand ShowAboutWindowCmd
        {
            get
            {
                if (_aboutWindowCmd == null)
                {
                    _aboutWindowCmd = new RelayCommand(param => this.AboutWindowOpen());
                }
                return _aboutWindowCmd;
            }
        }

        private RelayCommand _settingsWindowCmd;
        public ICommand ShowSettingsWindowCmd
        {
            get
            {
                if(_settingsWindowCmd == null)
                {
                    _settingsWindowCmd = new RelayCommand(param => this.SettingsWindowOpen());
                }
                return _settingsWindowCmd;
            }
        }

        private bool CanEdit
        {
            get { return (SelectedBatch != null); }
        }
#endregion

        public ViewModelMain()
        {
            this.Batches = new ObservableCollection<Batch>();
            if(Properties.Settings.Default.UseSQLServer)
                dbsql = new DatabaseHelperSqlServer(
                    Properties.Settings.Default.SqlUser,
                    Properties.Settings.Default.SqlPassword,
                    Properties.Settings.Default.SqlServerAddress,
                    Properties.Settings.Default.SqlDbName);
            if(Properties.Settings.Default.UseSQLServer && dbsql != null)
                LoadBatches(dbsql);
            else
                LoadBatches();
        }

        public ViewModelMain(string user, string password, string address, string dbname)
        {
            this.Batches = new ObservableCollection<Batch>();
            Properties.Settings.Default.UseSQLServer = true;
            if (Properties.Settings.Default.UseSQLServer)
                dbsql = new DatabaseHelperSqlServer(user, password, address, dbname);
            if (Properties.Settings.Default.UseSQLServer && dbsql != null)
                LoadBatches(dbsql);
            else
                LoadBatches();
        }

        public void LoadBatches()
        {
            DatabaseHelper db = new DatabaseHelper();
            ObservableCollection<Batch> batches = new ObservableCollection<Batch>();

            try
            {
                batches = db.GetBatchRecords();
                Batches = batches;
            }
            catch (Exception e)
            {
                ConsoleErrorPrint("Exception in LoadBatches()", e);
                logger.Error("[ViewModelMain:LoadBatches]" + e.Message);
            }
            
        }

        public void LoadBatches(DatabaseHelperSqlServer db)
        {
            ObservableCollection<Batch> batches = new ObservableCollection<Batch>();

            try
            {
                batches = db.GetBatchRecords();
                Batches = batches;
            }
            catch (Exception e)
            {
                ConsoleErrorPrint("Exception in LoadBatches()", e);
                logger.Error("[ViewModelMain:LoadBatches]" + e.Message);
            }

        }

        #region ButtonsCommand

        private void NewBatchItem()
        {
            var newBatchWindow = new NuovoBatch();
            if (Properties.Settings.Default.UseSQLServer && dbsql == null)
                newBatchWindow.DataContext = new ViewModelNewBatch();
            else
                newBatchWindow.DataContext = new ViewModelNewBatch(dbsql);
            newBatchWindow.ShowDialog();
            if(!Properties.Settings.Default.UseSQLServer && dbsql == null)
                LoadBatches();
            else
                LoadBatches(dbsql);
            RaisePropertyChanged("Batches");      
        }

        private void ModifyBatchItem()
        {
            var newBatchWindow = new NuovoBatch();
            if (Properties.Settings.Default.UseSQLServer && dbsql != null)
                newBatchWindow.DataContext = new ViewModelNewBatch(_intermediate, dbsql);
            else
                newBatchWindow.DataContext = new ViewModelNewBatch(_intermediate);
            newBatchWindow.ShowDialog();
            if (!Properties.Settings.Default.UseSQLServer && dbsql == null)
                LoadBatches();
            else
                LoadBatches(dbsql);
            RaisePropertyChanged("Batches");          
        }

        private void ResumeBatchItem()
        {
            var resumeBatchWindow = new BatchSelected();
            if (Properties.Settings.Default.UseSQLServer && dbsql != null)
                resumeBatchWindow.DataContext = new ViewModelBatchSelected(_intermediate, dbsql);
            else
                resumeBatchWindow.DataContext = new ViewModelBatchSelected(_intermediate);
            resumeBatchWindow.Show();           
            CloseWindow();
        }

        private void ModelAddItem()
        {
            var models = new Applicazione();
            if (Properties.Settings.Default.UseSQLServer && dbsql != null)
                models.DataContext = new ViewModelApplicazione(dbsql);
            else
                models.DataContext = new ViewModelApplicazione();
            models.ShowDialog();
            if (!Properties.Settings.Default.UseSQLServer && dbsql == null)
                LoadBatches();
            else
                LoadBatches(dbsql);
        }

        public void DeleteBatchItem()
        {
            if (Properties.Settings.Default.UseSQLServer && dbsql != null)
            {
                dbsql.DeleteFromTable("Batch", String.Format("Id = {0}", SelectedBatch.Id));
                Batches.Remove(SelectedBatch);
                LoadBatches(dbsql);
            }
            else
            {
                DatabaseHelper db = new DatabaseHelper();
                db.Delete("Batch", String.Format("Id = {0}", SelectedBatch.Id));
                Batches.Remove(SelectedBatch);
                LoadBatches();
            }
        }

        #endregion

        private void AboutWindowOpen()
        {
            var about = new AboutWindow();
            about.Show();
        }

        private void SettingsWindowOpen()
        {
            var settingsWindow = new SettingsApp();
            settingsWindow.Show();
        }
    }
}
