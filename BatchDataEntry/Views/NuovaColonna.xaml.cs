using System;
using System.Windows;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per NuovaColonna.xaml
    /// </summary>
    public partial class NuovaColonna : Window
    {
        public NuovaColonna()
        {
            InitializeComponent();
        }

        private void ButtonSalvaModel_OnClicknClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //this.Close();
        }

        private void checkBoxDisableColumn_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxSalvaValori.IsEnabled = false;
            checkBoxPartialSave.IsEnabled = false;
        }

        private void checkBoxDisableColumn_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxSalvaValori.IsEnabled = true;
            checkBoxPartialSave.IsEnabled = true;
        }

        private void checkBoxIsPrimary_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = false;
        }

        private void checkBoxIsPrimary_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = true;
        }

        private void checkBoxIsSecondary_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = true;
        }

        private void checkBoxIsSecondary_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = false;
            checkBoxIsSecondary.IsEnabled = true;
        }
    }
}
