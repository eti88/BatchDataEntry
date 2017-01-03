using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDataEntry : ViewModelBase
    {
        public ViewModelDataEntry()
        {
        }

        public ViewModelDataEntry(Batch b)
        {

        }

        /*
         - Nel caso della cancellazione di un documento oltre all'eliminazione del record bisogna eliminare anche il documento di origine (file)
         - Output copia dei file pdf
         */

        //protected void ConvertiTiffInPdf() { }
        //copy pdf() { }
        //protected void RimuoviDocumento(int indice) { }
    }
}
