using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelConcatenations : ViewModelBase
    {
        private AbsDbHelper db;

        private ObservableCollection<Concatenation> _concat;
        public ObservableCollection<Concatenation> Concatenazioni
        {
            get { return _concat; }
            set
            {
                if (_concat != value)
                {
                    _concat = value;
                    RaisePropertyChanged("Concatenazioni");
                }
            }
        }

        /*
          Tabella: Concatenazioni
          Colonne:
            - Id int pk autoc
            - Nome string
            - Modello int
            - List<Campi> // Per il salvataggio del db serializzare la lista solo di int 
         */

        public ViewModelConcatenations() {
            Concatenazioni = new ObservableCollection<Concatenation>();
            Concatenazioni.Add(new Concatenation(0, "Test 1", 0));
            Concatenazioni.Add(new Concatenation(1, "Test 2", 0));
            Concatenazioni.Add(new Concatenation(2, "Test 3", 0));
        }

        public ViewModelConcatenations(DatabaseHelperSqlServer _db, int idmodello)
        {
            db = _db;
            Concatenazioni = new ObservableCollection<Concatenation>();

        }

        private RelayCommand _addnew;
        public ICommand AddNewItemCommand
        {
            get
            {
                if (_addnew == null)
                    _addnew = new RelayCommand(param => AddNewConcatItem());
                return _addnew;
            }
        }

        public void AddNewConcatItem()
        {
            
        }

        public void DeleteItem(object elementId)
        {

        }
    }
}
