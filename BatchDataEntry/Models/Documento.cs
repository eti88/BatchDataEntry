using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{

    public class Documento : BaseModel
    {
        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        // Documento di origine es: pdf
        public string _origine;
        public string Origine { get { return _origine; }
            set
            {
                _origine = value;
                OnPropertyChanged("Origine");
            }
        } 

    }
}
