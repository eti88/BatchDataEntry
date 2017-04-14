using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //Extra
        private bool? _CloseWindowFlag;

        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raise a Propery Changed event
        /// </summary>
        /// <param name="prop"></param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => { CloseWindowFlag = CloseWindowFlag == null ? true : !CloseWindowFlag; }));
        }

        [Conditional("DEBUG")]
        protected void ConsoleErrorPrint(string message, Exception ex)
        {
            Console.WriteLine(message, ex.ToString());
        }
    }
}