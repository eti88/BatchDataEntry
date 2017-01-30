using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BatchDataEntry.Helpers;

namespace BatchDataEntry.Models
{
    public class Document: BaseModel
    {
        private int _index;
        public int Id
        {
            get { return _index; }
            set
            {
                if (_index != value)
                    _index = value;
                OnPropertyChanged("Id");
            }
        }

        private string _name;
        public string FileName
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
                OnPropertyChanged("FileName");
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

        private bool _isIndexed;
        public bool IsIndexed
        {
            get { return _isIndexed; }
            set
            {
                if (_isIndexed != value)
                    _isIndexed = value;
                OnPropertyChanged("IsIndexed");
            }
        }

        private ObservableCollection<Voce> _voci;
        public ObservableCollection<Voce> Voci
        {
            get { return _voci; }
            set
            {
                if (_voci != value)
                    _voci = value;
                OnPropertyChanged("Voci");
            }
        }

        public bool IsDirectory()
        {
            return (File.GetAttributes(this.Path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public Document()
        {
            this.Id = 0;
            this.Path = "";
            this.Voci = new ObservableCollection<Voce>();
        }

        public Document(Batch b, Dictionary<int, string> dictionary)
        {
            this.Voci = new ObservableCollection<Voce>();

            if(b.Applicazione.Id == 0)
                b.Applicazione.LoadCampi();
            if(b.Applicazione.Campi.Count == 0)
                b.Applicazione.LoadCampi();
            int h = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                KeyValuePair<int, string> row = dictionary.ElementAt(i);
                if (i == 0)
                    this.Id = Convert.ToInt32(row.Value);
                if(i == 1)
                    this.FileName = row.Value;
                if(i == 2)
                    this.Path = row.Value;
                if (i == 3)
                    this.IsIndexed = GetBool(row.Value);
                if (i > 3)
                {         
                    if(!string.IsNullOrEmpty(row.Value))
                        this.Voci.Add(new Voce(h, b.Applicazione.Campi.ElementAt(h).Nome, row.Value));
                    else
                        this.Voci.Add(new Voce(h, b.Applicazione.Campi.ElementAt(h).Nome));
                    h++;
                }       
            }
        }

        public Document(int id, string name, string path, bool indexed)
        {
            this.Id = id;
            this.FileName = name;
            this.Path = path;
            this.IsIndexed = indexed;
            this.Voci = new ObservableCollection<Voce>();
        }

        public Document(Document dc)
        {
            this.Id = dc.Id;
            this.FileName = dc.FileName;
            this.Path = dc.Path;
            this.IsIndexed = dc.IsIndexed;
            this.Voci = new ObservableCollection<Voce>();
        }

        public void AddInputsToPanel(Batch b, DatabaseHelper db)
        {
            ObservableCollection<Voce> voci = new ObservableCollection<Voce>();         
            foreach (Campo campo in b.Applicazione.Campi)
            {
                if (campo.SalvaValori)
                    voci.Add(new Voce(campo.Id, campo.Nome, campo.SalvaValori, db));
                else
                    voci.Add(new Voce(campo.Posizione, campo.Nome));
            }
            this.Voci = voci;
        }

        private bool GetBool(string val)
        {
            return (val == "1") ? true : false;
        }
    }
}
