using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class Autocompletamento
    {
        public int Id { get; set; }
        public int Colonna { get; set; }
        public string Valore { get; set; }

        public Autocompletamento() { }

        public Autocompletamento(int Col, string val)
        {
            this.Colonna = Col;
            this.Valore = val;
        }
    }
}
