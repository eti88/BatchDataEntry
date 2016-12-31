using System.Windows;
using System.Windows.Forms;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per NuovoBatch.xaml
    /// </summary>
    public partial class NuovoBatch : Window
    {

        public NuovoBatch()
        {
            InitializeComponent();          
        }

        private void ButtonCancella_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSelectPathInput_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxDirInput.Text = dialog.SelectedPath;
                }
            }
        }

        private void ButtonSelectPathOutput_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxDirOutput.Text = dialog.SelectedPath;
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
