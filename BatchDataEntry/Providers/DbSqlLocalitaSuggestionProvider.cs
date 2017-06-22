using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Interfaces;
using BatchDataEntry.Models;
using BatchDataEntry.Suggestions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WpfControls.Editors;

namespace BatchDataEntry.Providers
{
    public class DbSqlLocalitaSuggestionProvider : AbsDbSuggestions, ISuggestionProvider
    {
        public DbSqlLocalitaSuggestionProvider(int pos, string tab, int tabCol)
        {
            var suggestions = GetRecords(pos, tab, tabCol);
            ListOfSuggestions = suggestions;
        }

        public static IEnumerable<AbsSuggestion> GetRecords(int idcol, string sourceTable, int tableCol)
        {
            Batch b;
            if (Properties.Settings.Default.CurrentBatch == 0) return new List<AbsSuggestion>();
            try
            {
                AbsDbHelper db = null;
                if (Properties.Settings.Default.UseSQLServer)
                {
                    db = new DatabaseHelperSqlServer(Properties.Settings.Default.SqlUser, Properties.Settings.Default.SqlPassword,
                     Properties.Settings.Default.SqlServerAddress, Properties.Settings.Default.SqlDbName);
                }
                else
                    db = new DatabaseHelper();

                b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                if (b == null) return null;
                if (b.Applicazione == null || b.Applicazione.Id == 0) b.LoadModel(db);
                if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi(db);

                int pos = b.Applicazione.Campi[idcol].Posizione;
                IEnumerable<AbsSuggestion> task = GetList(db, sourceTable, tableCol);
                return task;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.Message);
#endif
                return new List<AbsSuggestion>();
            }
        }

        private static List<AbsSuggestion> GetList(AbsDbHelper db, string sourceTable, int column)
        {
            try
            {
                var lst = new List<AbsSuggestion>();
                if (db is DatabaseHelperSqlServer)
                {
                    var tmpdb = db as DatabaseHelperSqlServer;
                    lst = tmpdb.GetAutocompleteList(sourceTable, column);
                }
                return lst;
            }
            catch (Exception)
            {
                return new List<AbsSuggestion>();
            }
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) && ListOfSuggestions == null) return null;
            if (ListOfSuggestions.Count() == 0)
                return null;

            IEnumerable<AbsSuggestion> res = new List<AbsSuggestion>();
            res = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionLocalita)item).Valore) && ((SuggestionLocalita)item).Valore.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return res;
        }
    }
}
