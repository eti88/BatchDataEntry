using System;
using System.Windows;
using BatchDataEntry.Models;
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
    }
}
