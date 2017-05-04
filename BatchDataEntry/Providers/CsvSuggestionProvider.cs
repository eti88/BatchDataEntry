using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using CsvHelper;
using WpfControls.Editors;
using BatchDataEntry.Abstracts;
using BatchDataEntry.Suggestions;

namespace BatchDataEntry.Providers
{
    public class CsvSuggestionProvider: ISuggestionProvider
    {
        public IEnumerable<AbsSuggestion> ListOfSuggestions { get; set; }

        public CsvSuggestionProvider()
        {
            var suggestions = GetCsvRecords();
            ListOfSuggestions = suggestions;
        }

        public static IEnumerable<AbsSuggestion> GetCsvRecords()
        {
            Batch b;
            // Carica il batch corrente dalle impostazioni (controllare se presente)  
            if (Properties.Settings.Default.CurrentBatch == 0) return null;

            try
            {
                if (Properties.Settings.Default.UseSQLServer)
                {
                    var dbsql = new DatabaseHelperSqlServer(Properties.Settings.Default.SqlUser, Properties.Settings.Default.SqlPassword,
                     Properties.Settings.Default.SqlServerAddress, Properties.Settings.Default.SqlDbName);
                    b = dbsql.GetBatchById(Properties.Settings.Default.CurrentBatch);
                    if (b == null) return null;
                    if (b.Applicazione == null || b.Applicazione.Id == 0)
                        b.LoadModel(dbsql);
                    if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                        b.Applicazione.LoadCampi(dbsql);
                }
                else
                {
                    DatabaseHelper db = new DatabaseHelper();
                    b = db.GetBatchById(Properties.Settings.Default.CurrentBatch);
                    if (b == null) return null;
                    if (b.Applicazione == null || b.Applicazione.Id == 0)
                        b.LoadModel();
                    if (b.Applicazione.Campi == null || b.Applicazione.Campi.Count == 0)
                        b.Applicazione.LoadCampi();
                }

                int colIdx = 0;
                int colSec = 0;
                foreach (var c in b.Applicazione.Campi)
                {
                    if (c.IndicePrimario) colIdx = c.Posizione;
                    if (c.IndiceSecondario) colSec = c.Posizione;
                }
                IEnumerable<AbsSuggestion> task;
                if (colSec == 0)
                    task = GetSingleColumnList(b.Applicazione.PathFileCsv, b.Applicazione.Separatore, colIdx);
                else
                    task = GetDoubleColumnList(b.Applicazione.PathFileCsv, b.Applicazione.Separatore, colIdx, colSec);

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

        private static IEnumerable<AbsSuggestion> GetSingleColumnList(string file, string separator, int column)
        {
            var lst = new List<AbsSuggestion>();
            if (File.Exists(file))
            {
                using (var sr = new StreamReader(file))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.Delimiter = separator;
                    while (csv.Read())
                    {
                        lst.Add(new SuggestionSingleColumn(csv.GetField<string>(column)));
                    }
                }
            }
            
            return lst;
        }

        private static IEnumerable<AbsSuggestion> GetDoubleColumnList(string file, string separator, int colA, int colB)
        {
            var ret = new List<AbsSuggestion>();

            string[,] lst;
            if (File.Exists(file))
            {
                lst = Csv.ReadColumn(file, colA, colB);
                int len = lst.Length / 2;
                for (int i = 0; i < len; i++)
                {
                    ret.Add(new SuggestionDoubleColumn(lst[i, 0], lst[i, 1]));
                }
            }

            return ret;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) && ListOfSuggestions == null) return null;
            IEnumerable<AbsSuggestion> results = new List<AbsSuggestion>();
            if(this.ListOfSuggestions.GetType().GetElementType() is SuggestionDoubleColumn)
                results = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionDoubleColumn)item).ColumnA) && ((SuggestionDoubleColumn)item).ColumnA.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            else if(this.ListOfSuggestions.GetType().GetElementType() is SuggestionSingleColumn)
                results = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(((SuggestionSingleColumn)item).Valore) && ((SuggestionSingleColumn)item).Valore.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return results.ToList();
        }
    }
}
