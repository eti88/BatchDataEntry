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
            this.Close();
        }
    }
}
