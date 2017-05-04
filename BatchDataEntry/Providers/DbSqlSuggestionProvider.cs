using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatchDataEntry.Providers
{
    /// <summary>
    /// Recupera le occorrenze per l'autocompletamento dal database mssql
    /// </summary>
    public class DbSqlSuggestionProvider : AbsDbSuggestions
    {
        public static async Task<List<string>> GetRecords(int idcol, string sourceTable, int tableCol)
        {
            Batch b;
            if (Properties.Settings.Default.CurrentBatch == 0) return new List<string>();
            try
            {
                if (!Properties.Settings.Default.UseSQLServer) return new List<string>();

                    var dbsql = new DatabaseHelperSqlServer(Properties.Settings.Default.SqlUser, Properties.Settings.Default.SqlPassword,
                     Properties.Settings.Default.SqlServerAddress, Properties.Settings.Default.SqlDbName);
                    b = dbsql.GetBatchById(Properties.Settings.Default.CurrentBatch);
                    if (b == null) return new List<string>();
                    if (b.Applicazione == null || b.Applicazione.Id == 0)
                        b.LoadModel(dbsql);
                    if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                        b.Applicazione.LoadCampi(dbsql);          
                int pos = b.Applicazione.Campi[idcol].Posizione;
                List<string> task = await GetList(dbsql, sourceTable, tableCol);
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

        private static async Task<List<string>> GetList(DatabaseHelperSqlServer db, string sourceTable, int column)
        {
            try
            {
                var lst = new List<string>();
                await Task.Factory.StartNew(() =>
                {
                    lst = db.GetAutocompleteList(sourceTable, column);
                });
                return lst;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
