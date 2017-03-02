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
using WpfControls.Editors;

namespace BatchDataEntry.Providers
{
    public class CsvSuggestionProvider: ISuggestionProvider
    {
        public IEnumerable<Suggestion> ListOfSuggestions { get; set; }

        public CsvSuggestionProvider()
        {
            var suggestions = GetCsvRecords();
            ListOfSuggestions = suggestions;
        }

        public static IEnumerable<Suggestion> GetCsvRecords()
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
                IEnumerable<Suggestion> task;
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

        private static IEnumerable<Suggestion> GetSingleColumnList(string file, string separator, int column)
        {
            var lst = new List<Suggestion>();
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
                }
            }
            
            return lst;
        }

        private static IEnumerable<Suggestion> GetDoubleColumnList(string file, string separator, int colA, int colB)
        {
            var ret = new List<Suggestion>();

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

            return ret;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) && ListOfSuggestions == null) return null;
            var results = this.ListOfSuggestions.Where(item => !string.IsNullOrEmpty(item.ColumnA) && item.ColumnA.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase));
            return results.ToList();
        }
    }
}
