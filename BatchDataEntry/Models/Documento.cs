using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{

    public class Documento 
    {

        public int Id { get; set; }
        public string Origine { get; set; } // Documento di origine es: pdf

        public void Indicizza() { }
        public void Salta() { }
        public void Interrompi() { }
        
    }
}
