using System.Windows;
using System.Windows.Forms;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Tools.xaml
    /// </summary>
    public partial class Tools : Window
    {
        public Tools()
        {
            InitializeComponent();
        }

        private void ButtonSelectPath_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxDirInput.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
