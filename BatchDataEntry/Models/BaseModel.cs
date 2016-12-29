using System.ComponentModel;

namespace BatchDataEntry.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
