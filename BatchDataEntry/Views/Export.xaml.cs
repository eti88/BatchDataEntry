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
    /// Logica di interazione per Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        public Export()
        {
            InitializeComponent();
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
