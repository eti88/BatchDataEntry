using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatchDataEntry.Abstracts
{
    public abstract class AbsDbSuggestions
    {
        public static Task<List<string>> GetRecords(int idCol) {
            throw new NotImplementedException();
        }

        private static async Task<List<string>> GetList(AbsDbHelper db, int column)
        {
            throw new NotImplementedException();
        }
    }
}
