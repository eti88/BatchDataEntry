using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class Suggestion
    {
        public string ColumnA { get; set; }
        public string ColumnB { get; set; }

        public string FullSuggestion
        {
            get { return string.Format("{0} - {1}", ColumnA, ColumnB); }
        }

        public Suggestion() { }

        public Suggestion(string a, string b)
        {
            this.ColumnA = a;
            this.ColumnB = b;
        }
    }
}
