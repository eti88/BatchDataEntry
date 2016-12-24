using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{

    public class Campo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool SalvaValori { get; set; }
        public string ValorePredefinito { get; set; }
        public bool IndicePrimario { get; set; }
        public int TipoCampo { get; set; }

        /*
         SalvaValori permette di tenere traccia dei dati nel medesimo campo velocizzando
         futuri inserimenti.
         */
    }
}
