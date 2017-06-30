using System;
using System.Windows;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per DialogText.xaml
    /// </summary>
    public partial class DialogText : Window
    {
        public DialogText()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtPathNew.SelectAll();
            txtPathNew.Focus();
        }

        public string FullPathNew
        {
            get { return txtPathNew.Text; }
        }
    }
}
