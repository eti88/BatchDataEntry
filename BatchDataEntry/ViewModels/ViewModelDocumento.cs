using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Components;
using BatchDataEntry.DBModels;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using Batch = BatchDataEntry.Models.Batch;
using Campo = BatchDataEntry.Models.Campo;

namespace BatchDataEntry.ViewModels
{
    class ViewModelDocumento : ViewModelBase
    {
        private Doc _doc;
        private NavigationList<Doc> _dc;
        private Batch _batch;
        private static DatabaseHelper _db;

        public Doc DocFile
        {
            get { return _doc; }
            set
            {
                if (_doc != value)
                    _doc = value;
                RaisePropertyChanged("DocFile");
            }
        }
        public NavigationList<Doc> DocFiles
        {
            get { return _dc; }
            set
            {
                if (_dc != value)
                    _dc = value;
                RaisePropertyChanged("DocsFile");
            }
        }
        public Batch Batch
        {
            get { return _batch; }
            set
            {
                if (_batch != value)
                    _batch = value;
                RaisePropertyChanged("Batch");
            }
        }  

        /// <summary>
        /// Contiene tutti i Records che verranno salvati in un file bin finché non viene generato il file csv dalla schermata precedente
        /// </summary>
        private Records _records;
        public Records Records
        {
            get { return _records; }
            set
            {
                if (_records != value)
                    _records = value;
                RaisePropertyChanged("Records");
            }
        }

        private bool CanMoveNext {
            get { return (DocFiles != null && DocFiles.Count > 0 && DocFiles.hasNext); }
        }

        private bool CanMovePrevious
        {
            get { return (DocFiles != null && DocFiles.Count > 0 && DocFiles.hasPrevious); }
        }

        #region Command
        private RelayCommand _cmdPrev;
        public ICommand CmdPrev
        {
            get
            {
                if (_cmdPrev == null)
                {
                    _cmdPrev = new RelayCommand(param => this.MovePreviousItem(), param => this.CanMovePrevious);
                }
                return _cmdPrev;
            }
        }

        private RelayCommand _cmdNext;
        public ICommand CmdNext
        {
            get
            {
                if (_cmdNext == null)
                {
                    _cmdNext = new RelayCommand(param => this.MoveNextItem(), param => this.CanMoveNext);
                }
                return _cmdNext;
            }
        }

        private RelayCommand _cmdIndex;
        public ICommand CmdIndex
        {
            get
            {
                if (_cmdIndex == null)
                {
                    _cmdIndex = new RelayCommand(param => this.Indexes());
                }
                return _cmdIndex;
            }
        }

        private RelayCommand _cmdStop;
        public ICommand CmdStop
        {
            get
            {
                if (_cmdStop == null)
                {
                    _cmdStop = new RelayCommand(param => this.Interrompi());
                }
                return _cmdStop;
            }
        }
        #endregion

        public ViewModelDocumento()
        {
            this.Batch = new Batch();
        }

        public ViewModelDocumento(Batch _currentBatch, string indexRowVal)
        {        
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            LoadDocsList();
            LoadRecords(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));          
            //_voci = AddInputsToPanel(Batch.Applicazione.Campi);
            DocFiles.CurrentIndex = getId(indexRowVal);
            DocFile = DocFiles.Current;
            DocFile.AddInputsToPanel(Batch, Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            RaisePropertyChanged("DocFile");
            CheckFile();
        }

        public ViewModelDocumento(Batch _currentBatch)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione.Campi == null)
                Batch.Applicazione.LoadCampi();
            
            LoadDocsList();
            LoadRecords(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            DocFiles.CurrentIndex = getId();
            DocFile = DocFiles.Current;
            DocFile.AddInputsToPanel(Batch, Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            RaisePropertyChanged("DocFile");
            CheckFile();
        }

        private async Task LoadRecords(string path)
        {
            Records result = new Records();
            Task task = new Task(() =>
            {
                result.Load(path);
            });
            task.Start();
            if(result != null)
                this.Records = result;
        }

        private void LoadDocsList()
        {
            DatabaseHelper dbCache = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            DocFiles = dbCache.GetDocuments();
            RaisePropertyChanged("DocFiles");
        }

        // TODO: inserire controllo se campi sono modificati

        private int getId()
        {
            return Batch.UltimoIndicizzato;
        }

        private int getId(string idx)
        {
            int res = 0;
            res = DocFiles.IndexOf(DocFiles.Where(x => x.Id == idx).Single());
            return res;
        }

        

        private bool IsFileLocked(string filePath, int secondsToWait)
        {
            bool isLocked = true;
            int i = 0;

            while (isLocked && ((i < secondsToWait) || (secondsToWait == 0)))
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open)) { }
                    return false;
                }
                catch (IOException e)
                {
                    var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                    isLocked = errorCode == 32 || errorCode == 33;
                    i++;

                    if (secondsToWait != 0)
                        new System.Threading.ManualResetEvent(false).WaitOne(1000);
                }
            }

            return isLocked;
        }

        private async void CheckFile()
        {
            if (Batch != null)
            {
                if (Batch.TipoFile == TipoFileProcessato.Tiff && DocFile.IsDirectory())
                {
                    await Task.Run(() =>
                 {
                     DatabaseHelper db = new DatabaseHelper();
                     string fileName = Path.GetDirectoryName(DocFile.Path);
                     if (!Utility.ContainsOnlyNumbers(fileName))
                         fileName = Regex.Replace(fileName, "[A-Za-z]", "");

                     string newFilePath = Path.Combine(Batch.DirectoryOutput, fileName + ".pdf");
                     Utility.ConvertTiffToPdf(DocFile.Path, Batch.DirectoryOutput, fileName);
                     _db.UpdateRecord<Documento>(new Documento(DocFile)); // Aggiorna il Path nel cachedb
                     DocFile.Path = newFilePath;
                     RaisePropertyChanged("DocFile");
                 });
                }
            }
        }

        public void Indexes()
        {
            if (!IsFileLocked(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]), 5000))
            {
                // salvare record nel dbcsv
                // cambiare da false a true nel file cache input dir
                // copiare il file nell a dir
            }
        }

        public void MovePreviousItem()
        {
            if (DocFiles.hasPrevious)
            {
                DocFile = DocFiles.MovePrevious;
                if(DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            }          
            CheckFile();
            RaisePropertyChanged("DocFile");
        }

        public void MoveNextItem()
        {
            if (DocFiles.hasNext)
            {
                DocFile = DocFiles.MoveNext;
                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            }
            CheckFile();
            RaisePropertyChanged("DocFile");
        }

        public void Interrompi()
        {
            this.CloseWindow(true);
        }
    }
}
