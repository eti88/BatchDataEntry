using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class Doc: BaseModel
    {
        private string _index;
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                    _index = value;
                OnPropertyChanged("Index");
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                    _path = value;
                OnPropertyChanged("Path");
            }
        }

        public Doc()
        {
            this.Index = "";
            this.Path = "";
        }

        public Doc(string index, string path)
        {
            this.Index = index;
            this.Path = path;
        }

        public string FileName { get { return System.IO.Path.GetFileNameWithoutExtension(Path); } }
    }
}
