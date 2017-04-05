using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.Providers
{
    public static class DbSuggestionProvider
    {
        public static async Task<List<string>> GetRecords(int idCol)
        {
            Batch b;
            // Carica il batch corrente dalle impostazioni (controllare se presente)  
            if (Properties.Settings.Default.CurrentBatch == 0) return new List<string>();

            DatabaseHelper db = new DatabaseHelper();
            try
            {
                b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                if (b == null) return new List<string>();
                if (b.Applicazione == null || b.Applicazione.Id == 0)
                    b.LoadModel();
                if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                    b.Applicazione.LoadCampi();
                int z = b.Applicazione.Campi[idCol].Posizione;
                //b.Applicazione.Campi.Where(x => x.Id == idCol).Select(j => j.Posizione).Single();
                
                DatabaseHelper _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], b.DirectoryOutput);
                List<string> task = await GetList(_db, z);
                return task;
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.Message);
                #endif
                return new List<string>();
            }
        }

        private static async Task<List<string>> GetList(DatabaseHelper db, int column)
        {
            try {
                var lst = new List<string>();
                await Task.Factory.StartNew(() =>
                {
                    lst = db.GetAutocompleteList(column).ToList();
                });
                return lst;
            }
            catch(Exception)
            {
                return new List<string>();
            }           
        }
    }
}
