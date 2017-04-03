using System;
using System.Reflection;
using System.Windows;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            // Imposto la versione del programma
            Assembly app = Assembly.GetExecutingAssembly();
            Version version = app.GetName().Version;
            VersionTextBlock.Text = string.Format("Versione:  {0}", version.ToString());
        }
    }
}
