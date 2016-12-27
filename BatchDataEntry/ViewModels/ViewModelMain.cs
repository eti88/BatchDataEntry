using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelMain : ViewModelBase
    {
        public ViewModelMain()
        {
            this.Batches = new ObservableCollection<Batch>();
            //FillBatchesListbox();
        }

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

        private Batch _SelectedBatch = null;
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

        /*
        private void FillBatchesListbox()
        {
            Batches = new ObservableCollection<Batch>();
            using (disposable)
            {
                
            }
        }
        */
    }
}
