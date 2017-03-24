using System;
using System.Windows;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per BatchSelected.xaml
    /// </summary>
    public partial class BatchSelected : Window
    {
        public BatchSelected()
        {
            InitializeComponent();
        }

        private void dataGridRecords_Loaded(object sender, RoutedEventArgs e)
        {
            Batch b;
            if (Properties.Settings.Default.CurrentBatch == 0) return;

            DatabaseHelper db = new DatabaseHelper();
            try
            {
                b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                if (b == null) return;
                if (b.Applicazione == null || b.Applicazione.Id == 0) b.LoadModel();
                if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi();

                dataGridRecords.SelectedIndex = (b.UltimoIndicizzato > 1) ? b.UltimoIndicizzato - 1 : b.UltimoIndicizzato;
            }
            catch (Exception ex)
            {
                #if DEBUG
                Console.WriteLine(ex.ToString());
                #endif
                return;
            }
        }
    }
}
