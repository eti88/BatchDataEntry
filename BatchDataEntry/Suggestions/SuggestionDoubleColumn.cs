using BatchDataEntry.Abstracts;

namespace BatchDataEntry.Suggestions
{
    public class SuggestionDoubleColumn: AbsSuggestion
    {
        public string ColumnA { get; set; }
        public string ColumnB { get; set; }

        public SuggestionDoubleColumn() {
            this.NumCampi = 2;
        }

        public SuggestionDoubleColumn(string a, string b)
        {
            this.NumCampi = 2;
            this.ColumnA = a;
            this.ColumnB = b;
        }
    }
}
