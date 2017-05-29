using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BatchDataEntry.Helpers;
using BatchDataEntry.Abstracts;

namespace BatchDataEntry.Models
{
    public class Document: BaseModel
    {
        #region attr
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

        private ObservableCollection<Record> _voci;
        public ObservableCollection<Record> Voci
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

        #endregion

        public Document()
        {
            this.Id = 0;
            this.Path = "";
            this.Voci = new ObservableCollection<Record>();
        }

        public Document(AbsDbHelper db, Batch b, Dictionary<int, string> dictionary)
        {
            this.Voci = new ObservableCollection<Record>();
            if(b.Applicazione.Id == 0) b.Applicazione.LoadCampi(db);
            if(b.Applicazione.Campi.Count == 0) b.Applicazione.LoadCampi(db);

            try
            {
                int h = 0;
                for (int i = 0; i < dictionary.Count; i++)
                {
                    KeyValuePair<int, string> row = dictionary.ElementAt(i);
                    if (i == 0) this.Id = Convert.ToInt32(row.Value);
                    else if (i == 1) this.FileName = row.Value;
                    else if (i == 2) this.Path = row.Value;
                    else if (i == 3) this.IsIndexed = GetBool(row.Value);
                    else if (i > 3)
                    {
                        Record r = null;
                        if (!string.IsNullOrEmpty(row.Value))
                            r = Record.Create(b.Applicazione.Campi.ElementAt(h), h, row.Value);
                        else
                            r = Record.Create(b.Applicazione.Campi.ElementAt(h), h);

                        if (r == null) throw new Exception("Record creation error");
                        this.Voci.Add(r);
                        h++;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            
        }

        public Document(int id, string name, string path, bool indexed)
        {
            this.Id = id;
            this.FileName = name;
            this.Path = path;
            this.IsIndexed = indexed;
            this.Voci = new ObservableCollection<Record>();
        }

        public Document(Document dc)
        {
            this.Id = dc.Id;
            this.FileName = dc.FileName;
            this.Path = dc.Path;
            this.IsIndexed = dc.IsIndexed;
            this.Voci = new ObservableCollection<Record>();
        }

        public void AddInputsToPanel(Batch b, AbsDbHelper db, DatabaseHelper dcache, Dictionary<int, string> dictionary)
        {
            if (this.Voci != null && this.Voci.Count > 0) return;
            int h = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                if (i > 3)
                {
                    KeyValuePair<int, string> row = dictionary.ElementAt(i);
                    //voci.Add(Record.Create(campo, campo.Posizione));
                    Record r = null;
                    if (!string.IsNullOrEmpty(row.Value))
                        r = Record.Create(b.Applicazione.Campi.ElementAt(h), h, row.Value);
                    else
                        r = Record.Create(b.Applicazione.Campi.ElementAt(h), h);
                    this.Voci.Add(r);
                    h++;
                }
            }
        }

        private bool GetBool(string val)
        {
            return (val == "1") ? true : false;
        }
    }
}
