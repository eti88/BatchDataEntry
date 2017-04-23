using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per SettingsApp.xaml
    /// </summary>
    public partial class SettingsApp : Window
    {
        public SettingsApp()
        {
            InitializeComponent();
        }

        private void ckUseServer_Unchecked(object sender, RoutedEventArgs e)
        {
            DisableCampi();
        }

        private void DisableCampi()
        {
            if (txtUser != null)
                txtUser.IsEnabled = false;
            if (txtPass != null)
                txtPass.IsEnabled = false;
            if (txtAddress != null)
                txtAddress.IsEnabled = false;
            if (txtDbName != null)
                txtDbName.IsEnabled = false;
        }

        private void EnableCampi()
        {
            if(txtUser != null)
                txtUser.IsEnabled = true;
            if (txtPass != null)
                txtPass.IsEnabled = true;
            if (txtAddress != null)
                txtAddress.IsEnabled = true;
            if (txtDbName != null)
                txtDbName.IsEnabled = true;
        }

        private void ckUseServer_Checked(object sender, RoutedEventArgs e)
        {
            EnableCampi();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (ckUseServer.IsChecked.Value)
                EnableCampi();
            else
                DisableCampi();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
