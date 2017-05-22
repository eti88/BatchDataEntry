using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatchDataEntry.Abstracts
{
    public abstract class AbsDbSuggestions
    {
        public IEnumerable<AbsSuggestion> ListOfSuggestions { get; set; }

        public static IEnumerable<string> GetRecords(int idCol) {
            throw new NotImplementedException();
        }

        private static IEnumerable<string> GetList(AbsDbHelper db, int column)
        {
            throw new NotImplementedException();
        }
    }
}
