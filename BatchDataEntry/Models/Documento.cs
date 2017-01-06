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
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        // Documento di origine es: path pdf
        public string _origine;
        public string Origine { get { return _origine; }
            set
            {
                if (_origine != value)
                {
                    _origine = value;
                    OnPropertyChanged("Origine");
                }
            }
        }

        public string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                }
            }
        }

        public Documento() { }

        public Documento(string id, string origine, string filename)
        {
            this.Id = id;
            this.Origine = origine;
            this.FileName = filename;
        }
    }
}
