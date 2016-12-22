using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class Documento 
    {
        public string Origine { get; set; } // Documento di origine es: pdf

        public void Indicizza() { }
        public void Salta() { }
        public void Interrompi() { }
        
    }
}
