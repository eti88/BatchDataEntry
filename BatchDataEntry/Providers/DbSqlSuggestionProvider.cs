using BatchDataEntry.Abstracts;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Suggestions;
using System;
using System.Collections.Generic;
using WpfControls.Editors;
using System.Collections;
using System.Linq;

namespace BatchDataEntry.Providers
{
    /// <summary>
    /// Recupera le occorrenze per l'autocompletamento dal database mssql
    /// </summary>
    public class DbSqlSuggestionProvider : AbsDbSuggestions, ISuggestionProvider
    {

        public DbSqlSuggestionProvider(int pos, string tab, int tabCol, EnumTypeOfCampo type)
        {
            var suggestions = GetRecords(pos, tab, tabCol, type);
            ListOfSuggestions = suggestions;
        }

        public static IEnumerable<AbsSuggestion> GetRecords(int idcol, string sourceTable, int tableCol, EnumTypeOfCampo type)
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
                IEnumerable<AbsSuggestion> task = GetList(db, sourceTable, tableCol, type);
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

        private static List<AbsSuggestion> GetList(AbsDbHelper db, string sourceTable, int column, EnumTypeOfCampo type)
        {
            try
            {
                var lst = new List<AbsSuggestion>();
                if (db is DatabaseHelperSqlServer)
                {
                    var tmpdb = db as DatabaseHelperSqlServer;
                    lst = tmpdb.GetAutocompleteList(sourceTable, column, type);
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
            
            if (this.ListOfSuggestions.ElementAt(0) is SuggestionSingleColumn)
            {
                res = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionSingleColumn)item).Valore) && ((SuggestionSingleColumn)item).Valore.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else if (this.ListOfSuggestions.ElementAt(0) is SuggestionLocalita)
            {
                res = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionLocalita)item).Valore) && ((SuggestionLocalita)item).Valore.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            else if (this.ListOfSuggestions.ElementAt(0) is SuggestionDoubleColumn)
            {
                res = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionDoubleColumn)item).Valore) && ((SuggestionDoubleColumn)item).Valore.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            return res;
        }
    }
}
