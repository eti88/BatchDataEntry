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
using Microsoft.Win32;
using FileDialog = System.Windows.Forms.FileDialog;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Applicazione.xaml
    /// </summary>
    public partial class Applicazione : Window
    {
        public Applicazione()
        {
            InitializeComponent();
            textBoxFileCsv.IsEnabled = false;
            buttonChooseFile.IsEnabled = false;
            textBoxSeparator.IsEnabled = false;
        }

        private void ButtonChooseFile_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = ".csv";
            fileDialog.Filter = "CSV files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            Nullable<bool> result = fileDialog.ShowDialog();
            if (result == true)
            {
                textBoxFileCsv.Text = fileDialog.FileName;
            }
        }

        private void checkBoxCsv_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBoxCsv.IsChecked == true)
            {
                textBoxFileCsv.IsEnabled = true;
                buttonChooseFile.IsEnabled = true;
                textBoxSeparator.IsEnabled = true;
            }
            else
            {
                textBoxFileCsv.IsEnabled = false;
                buttonChooseFile.IsEnabled = false;
                textBoxSeparator.IsEnabled = false;
            }
        }

        private void CheckBoxCsv_OnUnchecked(object sender, RoutedEventArgs e)
        {
            textBoxFileCsv.IsEnabled = false;
            buttonChooseFile.IsEnabled = false;
            textBoxSeparator.IsEnabled = false;
        }
    }
}
