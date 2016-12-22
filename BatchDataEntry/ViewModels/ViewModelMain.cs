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
        public ObservableCollection<Batch> Batches { get; set; }

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

        
    }
}
