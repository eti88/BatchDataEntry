using BatchDataEntry.Abstracts;

namespace BatchDataEntry.Suggestions
{
    public class SuggestionSingleColumn : AbsSuggestion
    {
        public SuggestionSingleColumn() {
            NumCampi = 1;
        }

        public SuggestionSingleColumn(string val)
        {
            NumCampi = 1;
            Valore = val;
        }

        public SuggestionSingleColumn(int col, string val)
        {
            NumCampi = 1;
            Colonna = col;
            Valore = val;
        }
    }
}
