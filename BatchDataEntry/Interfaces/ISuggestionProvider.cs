using System.Collections.Generic;

namespace BatchDataEntry.Interfaces
{
    interface ISuggestionProvider<T>
    {
        IEnumerable<T> ListOfSuggestions { get; set; }
        IEnumerable<T> GetRecords();
        IEnumerable<T> GetSuggestions();
    }
}
