using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelMain : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<Batch> _Batches { get; set; }
        public ObservableCollection<Batch> Batches
        {
            get
            {
                return _Batches;
            }
            set
            {
                _Batches = value;
                RaisePropertyChanged("LstBatchesUpd");
            }
        }   

        private Batch _SelectedBatch;
        public Batch SelectedBatch
        {
            get { return _SelectedBatch; }
            set
            {
                _SelectedBatch = value;
                RaisePropertyChanged("SelectedBatch");
                // Dosomething
            }
        }

        private RelayCommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get {
                if (_deleteCommand == null)
                {
                        _deleteCommand = new RelayCommand(param => this.DeleteBatchItem(), param => this.CanDelete);
                }
                return _deleteCommand;
            }
        }

        private bool CanDelete
        {
            get { return (SelectedBatch == null) ? false : true; }
        }

        public ViewModelMain()
        {
            this.Batches = new ObservableCollection<Batch>();
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
                #if DEBUG
                Console.WriteLine(@"Exception in LoadBatches()");
                Console.WriteLine(e.ToString());
                #endif
                logger.Error("[ViewModelMain:LoadBatches]" + e.Message);
            }
            
        }

        private void DeleteBatchItem()
        {
            DatabaseHelper db = new DatabaseHelper();
            
            db.DeleteRecord(SelectedBatch, SelectedBatch.Id);
            Batches.Remove(SelectedBatch);
        }

    }
}
