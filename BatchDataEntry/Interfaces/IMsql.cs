using System.Collections.Generic;

namespace BatchDataEntry.Interfaces
{
    public interface IMsql
    {
        List<string> GetAutocompleteList(string tableName, int columnTable);
    }
}
