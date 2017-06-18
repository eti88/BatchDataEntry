using BatchDataEntry.Abstracts;

namespace BatchDataEntry.Suggestions
{
    public class SuggestionLocalita : AbsSuggestion
    {
        public string Pv { get; set; }
        public string Cap { get; set; }


        public SuggestionLocalita(string loc, string pv, string cap)
        {
            this.Valore = loc;
            this.Pv = pv;
            this.Cap = cap;
        }
    }
}
