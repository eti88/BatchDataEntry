using BatchDataEntry.Interfaces;

namespace BatchDataEntry.Abstracts
{
    public abstract class AbsSuggestion: ISuggestion
    {
        public int NumCampi { get; set; }
        public int Colonna { get; set; }
        public string Valore { get; set; }
    }
}
