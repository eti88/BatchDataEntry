using System;
using System.Windows;
using Gu.Wpf.NumericInput;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per DialogNumeration.xaml
    /// </summary>
    public partial class DialogNumeration : Window
    {
        public DialogNumeration()
        {
            InitializeComponent();
            txtZeri.Text = "0";
            txtNumStart.Text = "0";
            txtIdxStart.Text = "1";
            txtIdxStop.Text = "0";
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Convert.ToInt32(txtZeri.Text) > 0) && 
                    (Convert.ToInt32(txtNumStart.Text) > 0) &&
                    (Convert.ToInt32(txtIdxStart.Text) >= 1) &&
                    (Convert.ToInt32(txtIdxStop.Text) > 0))
                    this.DialogResult = true;
            }
            catch (FormatException)
            {
                return;
            }
            catch (Exception)
            {             
                // ignore
            }
            
        }

        public string GetZeri { get { return txtZeri.Text; } }
        public string GetStartNumber { get { return txtNumStart.Text; } }
        public string GetIndexStart { get { return txtIdxStart.Text; } }
        public string GetIndexStop { get { return txtIdxStop.Text; } }
    }
}
