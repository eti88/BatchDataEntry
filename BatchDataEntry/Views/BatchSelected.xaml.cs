using System;
using System.Windows;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Abstracts;
using System.Threading;

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
            AbsDbHelper db = null;
            if (Properties.Settings.Default.CurrentBatch == 0) return;

            try
            {
                if (Properties.Settings.Default.UseSQLServer)
                {
                    db = new DatabaseHelperSqlServer(
                        Properties.Settings.Default.SqlUser,
                        Properties.Settings.Default.SqlPassword,
                        Properties.Settings.Default.SqlServerAddress,
                        Properties.Settings.Default.SqlDbName);
                }
                else
                    db = new DatabaseHelper();

                b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                if (b == null) return;
                if (b.Applicazione == null || b.Applicazione.Id == 0) b.LoadModel(db);
                if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi(db);
                Thread td = new Thread(() =>
                {
                    Thread.Sleep(500);
                    SetSelectedGridRow((b.UltimoIndicizzato == 0) ? 0 : (b.UltimoIndicizzato - 1));
                });
                td.SetApartmentState(ApartmentState.STA);
                td.Start();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.ToString());
#endif
                return;
            }
        }

        public void SetSelectedGridRow(int index)
        {
            Dispatcher.Invoke(new Action(() => { dataGridRecords.SelectedIndex = index; }));
        }
    }
}
