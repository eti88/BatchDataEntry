using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using CsvHelper;
using NLog;

namespace BatchDataEntry.Providers
{
    public static class CsvSuggestionProvider
    {
        public static async Task<ObservableCollection<Suggestion>> GetCsvRecords()
        {
            Batch b;
            // Carica il batch corrente dalle impostazioni (controllare se presente)  
            if (Properties.Settings.Default.CurrentBatch == 0) return null;

            DatabaseHelper db = new DatabaseHelper();
            try
            {
                b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                if (b == null) return null;
                if (b.Applicazione == null || b.Applicazione.Id == 0)
                    b.LoadModel();
                if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                    b.Applicazione.LoadCampi();
                int colIdx = 0;
                int colSec = 0;
                foreach (var c in b.Applicazione.Campi)
                {
                    if (c.IndicePrimario) colIdx = c.Posizione;
                    if (c.IndiceSecondario) colSec = c.Posizione;
                }
                ObservableCollection<Suggestion> task;
                if (colSec == 0)
                    task = await GetSingleColumnList(b.Applicazione.PathFileCsv,
                        b.Applicazione.Separatore, colIdx);
                else
                    task = await GetDoubleColumnList(b.Applicazione.PathFileCsv,
                        b.Applicazione.Separatore, colIdx, colSec);

                return task;
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.Message);
                #endif
                return null;
            }
        }

        private static async Task<ObservableCollection<Suggestion>> GetSingleColumnList(string file, string separator, int column)
        {
            var lst = new ObservableCollection<Suggestion>();
            await Task.Factory.StartNew(() =>
            {
                if (File.Exists(file))
            {
                using (var sr = new StreamReader(file))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.Delimiter = separator;
                    while (csv.Read())
                    {
                        lst.Add(new Suggestion(csv.GetField<string>(column), string.Empty));
                    }
                }}
            });
            return lst; 
        }

        private static async Task<ObservableCollection<Suggestion>> GetDoubleColumnList(string file, string separator, int colA, int colB)
        {
            var ret = new ObservableCollection<Suggestion>();
            await Task.Factory.StartNew(() =>
            {
                string[,] lst;
                if (File.Exists(file))
                {
                    lst = Csv.ReadColumn(file, colA, colB);
                    int len = lst.Length / 2;
                    for (int i = 0; i < len; i++)
                    {
                        ret.Add(new Suggestion(lst[i, 0], lst[i, 1]));
                    }
                }
            });
            return ret;
        }
    }
}
