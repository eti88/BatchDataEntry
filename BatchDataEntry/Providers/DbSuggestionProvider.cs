using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.Providers
{
    public static class DbSuggestionProvider
    {
        public static async Task<ObservableCollection<Suggestion>> GetRecords(int idCol)
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
                int z = b.Applicazione.Campi[idCol].Posizione;
                //b.Applicazione.Campi.Where(x => x.Id == idCol).Select(j => j.Posizione).Single();
                

                DatabaseHelper _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], b.DirectoryOutput);
                ObservableCollection<Suggestion> task = await GetList(_db, z);
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

        private static async Task<ObservableCollection<Suggestion>> GetList(DatabaseHelper db, int column)
        {
            var lst = new ObservableCollection<Suggestion>();
            await Task.Factory.StartNew(() =>
            {
                lst = db.GetAutocompleteListOb(column);
            });
            return lst;
        }
    }
}
