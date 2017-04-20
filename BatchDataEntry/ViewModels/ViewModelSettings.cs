using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using NLog;
using System;
using System.Windows;
using System.Windows.Input;

namespace BatchDataEntry.Views
{
    public class ViewModelSettings : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DbCredential _credential;
        public DbCredential Credential
        {
            get
            {
                return _credential;
            }
            set
            {
                if (_credential != value)
                {
                    _credential = value;
                    RaisePropertyChanged("Credential");
                }

            }
        }

        private RelayCommand _btnTestCmd;
        public ICommand BtnTestCmd
        {
            get
            {
                if (_btnTestCmd == null)
                    _btnTestCmd = new RelayCommand(param => this.TestConnection());
                return _btnTestCmd;
            }
        }

        private RelayCommand _btnSaveCmd;
        public ICommand BtnSaveCmd
        {
            get
            {
                if (_btnSaveCmd == null)
                    _btnSaveCmd = new RelayCommand(param => this.SaveCredential());
                return _btnSaveCmd;
            }
        }

        public ViewModelSettings() {
            try
            {
                Credential = DbCredential.Load();
            }
            catch (Exception)
            {
                Credential = new DbCredential();
            }

        }

        public void TestConnection() {
            bool test = false;
            if (Credential != null)
                test = Credential.TestConnection();
            MessageBox.Show(string.Format("Test {0}", (test) ? "riuscito" : "fallito"));
        }

        public void SaveCredential() {
            if (string.IsNullOrEmpty(Credential.User)) Credential.User = string.Empty;
            if (string.IsNullOrEmpty(Credential.Password)) Credential.Password = string.Empty;
            if (string.IsNullOrEmpty(Credential.Address)) Credential.Address = string.Empty;
            if (string.IsNullOrEmpty(Credential.Dbname)) Credential.Dbname = string.Empty;
            Credential.Save();
        }
    }
}
