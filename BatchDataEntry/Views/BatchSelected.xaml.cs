﻿using System;
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
            DatabaseHelperSqlServer dbsql;
            DatabaseHelper db;
            if (Properties.Settings.Default.CurrentBatch == 0) return;

            try
            {
                if (Properties.Settings.Default.UseSQLServer)
                {
                    dbsql = new DatabaseHelperSqlServer(
                        Properties.Settings.Default.SqlUser,
                        Properties.Settings.Default.SqlPassword,
                        Properties.Settings.Default.SqlServerAddress,
                        Properties.Settings.Default.SqlDbName);
                    b = dbsql.GetBatchById(Properties.Settings.Default.CurrentBatch);
                    if (b == null) return;
                    if (b.Applicazione == null || b.Applicazione.Id == 0) b.LoadModel(dbsql);
                    if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi(dbsql);
                }
                else
                {
                    db = new DatabaseHelper();
                    b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                    if (b == null) return;
                    if (b.Applicazione == null || b.Applicazione.Id == 0) b.LoadModel();
                    if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi();
                }
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
