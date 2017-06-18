using BatchDataEntry.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Suggestions
{
    public class SuggestionLocalita : AbsSuggestion
    {
        public string LDisplay
        {
            get { return string.Format("{0} - {1} - {2}", this.Valore, this.Pv, this.Cap); }
        }

        private string Pv { get; set; }
        private string Cap { get; set; }

        public SuggestionLocalita(string loc, string pv, string cap)
        {
            this.Valore = loc;
            this.Pv = pv;
            this.Cap = cap;
        }
    }
}
