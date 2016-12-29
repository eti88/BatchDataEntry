using System;
using System.Collections.ObjectModel;
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

        private void buttonNewBatch_Click(object sender, RoutedEventArgs e)
        {
            var newBatchWindow = new NuovoBatch();
            newBatchWindow.Show();
            this.Close();
        }

        private void buttonResumeBatch_Click(object sender, RoutedEventArgs e)
        {
            var resumeBatchWindow = new BatchSelected {DataContext = new ViewModelBatchSelected(listBoxBatches.SelectedItem) };
            resumeBatchWindow.Show();
            this.Close();
        }

        private void ButtonAddApp_OnClick(object sender, RoutedEventArgs e)
        {
            var addModel = new Applicazione();
            addModel.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
