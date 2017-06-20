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

        private bool CanEdit
        {
            get { return (SelectedConcat != null); }
        }

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

        private Concatenation _selectedConcat;
        public Concatenation SelectedConcat
        {
            get { return _selectedConcat; }
            set
            {
                if (_selectedConcat != value)
                {
                    _selectedConcat = value;
                    RaisePropertyChanged("SelectedConcat");
                }
            }
        }

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

        private RelayCommand _cmdmod;
        public ICommand ModifyCommand
        {
            get
            {
                if (_cmdmod == null)
                    _cmdmod = new RelayCommand(param => ModifyItem(), param => this.CanEdit);
                return _cmdmod;
            }
        }

        private RelayCommand _del;
        public ICommand DeleteCommand
        {
            get
            {
                if (_del == null)
                    _del = new RelayCommand(param => DeleteItem(), param => this.CanEdit);
                return _del;
            }
        }

        public void AddNewConcatItem()
        {
            if (Concatenazioni == null)
                Concatenazioni = new ObservableCollection<Concatenation>();

            Concatenazioni.Add(new Concatenation());
        }

        public void ModifyItem()
        {

        }

        public void DeleteItem()
        {

        }
    }
}
