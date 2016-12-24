using System;
using System.Collections.ObjectModel;
using System.Windows;
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

        private void buttonDeleteBatch_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonAddApp_OnClick(object sender, RoutedEventArgs e)
        {
            var addModel = new Applicazione();
            addModel.Show();
        }
    }
}
