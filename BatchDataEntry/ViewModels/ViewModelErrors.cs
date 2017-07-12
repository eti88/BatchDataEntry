using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelErrors : ViewModelMain
    {
        private ObservableCollection<string> _errors;
        public ObservableCollection<string> Errors
        {
            get { return _errors; }
            set
            {
                if (_errors != value)
                    _errors = value;
                RaisePropertyChanged("Errors");
            }
        }

        public ViewModelErrors() { }

        public ViewModelErrors(List<string> files)
        {
            Errors = new ObservableCollection<string>(files);
        }

    }
}
