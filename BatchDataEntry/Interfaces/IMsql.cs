using BatchDataEntry.Abstracts;
using System.Collections.Generic;

namespace BatchDataEntry.Interfaces
{
    public interface IMsql
    {
        List<AbsSuggestion> GetAutocompleteList(string tableName, int columnTable);
    }
}
