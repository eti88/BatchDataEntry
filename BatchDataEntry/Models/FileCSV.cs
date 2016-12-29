using System.ComponentModel;
using SQLite;

namespace BatchDataEntry.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("FileCSV")]
    public class FileCSV : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        private string _Path { get; set; }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                if (_Path != null)
                {
                    _Path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        private string _Separatore { get; set; }
        [MaxLength(1)]
        public string Separatore { get { return _Separatore; }
            set
            {
                if (_Separatore != null)
                {
                    _Separatore = value;
                    RaisePropertyChanged("Separatore");
                }
            } }

        public FileCSV()
        {
        }

        public FileCSV(string p, string s)
        {
            this.Path = p;
            this.Separatore = s;
        }

        public FileCSV(long id, string p, string s)
        {
            this.Id = id;
            this.Path = p;
            this.Separatore = s;
        }
    }
}
