using BatchDataEntry.Abstracts;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelTools : ViewModelBase
    {
        private AbsDbHelper db;

        public ViewModelTools() { }

        
    }

    class ColumnCheck : ExportColumn
    {
        private Check _tck;
        public Check Type
        {
            get { return _tck; }
            set
            {
                if (_tck == value) return;
                _tck = value;
                OnPropertyChanged("Type");
            }
        }

        public ColumnCheck(string label, bool voidCheck) : base(label, voidCheck)
        {
            this.Type = SetCheckType(label);
        }

        private Check SetCheckType(string lab)
        {
            Check res = Check.None;
            if(lab.ToLower().Contains("data"))
            {
                res = Check.Data;
            }else if(lab.ToLower().Contains("email"))
            {
                res = Check.Email;
            }
            else if (lab.ToLower().Contains("telefono") || lab.ToLower().Contains("cellulare"))
            {
                res = Check.Telefono;
            }
            return res;
        }

        public bool CheckisNotVoid(string text) {
            bool res;
            if (string.IsNullOrEmpty(text))
                res = false;
            else
                res = true;
            return res;
        }

        public bool CheckTel(string n)
        {
            return false;
        }

        public bool CheckEmail(string email)
        {
            return false;
        }

        public bool CheckDate(string date,  string dateFormat) {

            return false;
        }
    }

    enum Check
    {
        None = 0,
        Data = 1,
        Email = 2,
        Telefono = 3
    }
}
