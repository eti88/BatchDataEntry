using System;
using System.Windows;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per NuovoModello.xaml
    /// </summary>
    public partial class NuovoModello : Window
    {
        public NuovoModello()
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

        private void ButtonSalvaModel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
