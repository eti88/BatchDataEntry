using BatchDataEntry.Helpers;
using System;
using System.Data.SqlClient;
using System.Security;
using System.Windows;

namespace BatchDataEntry.Models
{
    public class DbCredential : BaseModel
    {
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("adTd1UcQj2Bxx");
        private bool _use;
        private string _user;
        private string _pass;
        private string _address;
        private string _dbname;

        public bool Use
        {
            get { return _use; }
            set
            {
                if (value != _use)
                {
                    _use = value;
                    OnPropertyChanged("Use");
                }
            }
        }
        public string User
        {
            get { return _user; }
            set
            {
                if (value != _user)
                {
                    _user = value;
                    OnPropertyChanged("User");
                }
            }
        }
        public string Password
        {
            get { return _pass; }
            set
            {
                if (value != _pass)
                {
                    _pass = value;
                    OnPropertyChanged("Password");
                }
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }
        public string Dbname
        {
            get { return _dbname; }
            set
            {
                if (value != _dbname)
                {
                    _dbname = value;
                    OnPropertyChanged("Dbname");
                }
            }
        }

        public DbCredential()
        {
            Use = false;
            User = string.Empty;
            Password = string.Empty;
            Address = string.Empty;
            Dbname = string.Empty;
        }

        public DbCredential(bool use, string user, string pass, string addr, string dbname)
        {
            Use = use;
            User = user;
            Password = pass;
            Address = addr;
            Dbname = dbname;
        }

        public override string ToString()
        {
            return string.Format("[{0}], User:{1}, Pass:{2}, Addr:{3}, Db:{4}", Use, User, Password, Address, Dbname);
        }

        public void Save() {
            var settings = Properties.Settings.Default;
            if (settings.UseSQLServer != Use)
                settings.UseSQLServer = Use;
            if (settings.SqlUser != User)
                settings.SqlUser = User;
            if (settings.SqlPassword != Password)
                settings.SqlPassword = Password;
            if (settings.SqlServerAddress != Address)
                settings.SqlServerAddress = Address;
            if (settings.SqlDbName != Dbname)
                settings.SqlDbName = Dbname;
            settings.Save();
            MessageBox.Show("Riavvia l'applicazione per rendere effettive le modifiche.");
        }

        public static DbCredential Load() {
            DbCredential crd = new DbCredential(Properties.Settings.Default.UseSQLServer,
                Properties.Settings.Default.SqlUser,
                Properties.Settings.Default.SqlPassword,
                Properties.Settings.Default.SqlServerAddress,
                Properties.Settings.Default.SqlDbName);
            return crd;
        }

        public bool TestConnection()
        {
            if (string.IsNullOrWhiteSpace(User) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Address) ||
                string.IsNullOrWhiteSpace(Dbname)) return false;

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = Address;
            builder.UserID = User;
            builder.Password = Password;
            builder.InitialCatalog = Dbname;
            builder.ConnectTimeout = 30;
            builder.MultipleActiveResultSets = true;
            return DatabaseHelperSqlServer.IsServerConnected(builder);
        }
    }
}
