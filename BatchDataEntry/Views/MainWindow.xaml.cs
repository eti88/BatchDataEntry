using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.ViewModels;
using BatchDataEntry.Views;

namespace BatchDataEntry
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void ButtonLog_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "logFile.log")))
            {
                Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "logFile.log"));
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
