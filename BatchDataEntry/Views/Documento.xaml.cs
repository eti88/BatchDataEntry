using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BatchDataEntry.Models;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Documento.xaml
    /// </summary>
    public partial class Documento : Window
    {
        public Documento()
        {
            InitializeComponent();
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
