using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Components;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelDocumento : ViewModelBase
    {
        private static DatabaseHelper _db;
        private Batch _batch;
        private NavigationList<Dictionary<int, string>> _dc;
        private Document _doc;

        public Document DocFile
        {
            get { return _doc; }
            set
            {
                if (_doc != value)
                    _doc = value;
                RaisePropertyChanged("DocFile");
            }
        }

        public NavigationList<Dictionary<int, string>> DocFiles
        {
            get { return _dc; }
            set
            {
                if (_dc != value)
                    _dc = value;
                RaisePropertyChanged("DocFiles");
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

        private bool CanMoveNext
        {
            get { return DocFiles != null && DocFiles.Count > 0 && DocFiles.hasNext; }
        }

        private bool CanMovePrevious
        {
            get { return DocFiles != null && DocFiles.Count > 0 && DocFiles.hasPrevious; }
        }

        public ViewModelDocumento()
        {
            Batch = new Batch();
        }

        public ViewModelDocumento(Batch _currentBatch, int indexRowVal)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0)
                Batch.LoadModel();
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0)
                Batch.Applicazione.LoadCampi();
            LoadDocsList();
            DocFiles.CurrentIndex = indexRowVal;
            DocFile = new Document(Batch, DocFiles.Current);
        }

        public ViewModelDocumento(Batch _currentBatch)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0)
                Batch.LoadModel();
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0)
                Batch.Applicazione.LoadCampi();

            LoadDocsList();
            DocFiles.CurrentIndex = GetId();
            Document tmp = new Document(Batch, DocFiles.Current);
            DocFile = new Document(Batch, DocFiles.Current);
            
        }

        

        private void LoadDocsList()
        {
            var dbCache = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            DocFiles = dbCache.GetDocuments();
            RaisePropertyChanged("DocFiles");
        }

        private int GetId()
        {
            return Batch.UltimoIndicizzato;
        }

        private bool IsFileLocked(string filePath, int secondsToWait)
        {
            var isLocked = true;
            var i = 0;

            while (isLocked && ((i < secondsToWait) || (secondsToWait == 0)))
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open))
                    {
                    }
                    return false;
                }
                catch (IOException e)
                {
                    var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                    isLocked = errorCode == 32 || errorCode == 33;
                    i++;

                    if (secondsToWait != 0)
                        new ManualResetEvent(false).WaitOne(1000);
                }
            }

            return isLocked;
        }

        public void Indexes()
        {
            if (
                !IsFileLocked(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["csv_file_name"]),
                    5000))
            {
                DocFile.IsIndexed = true;
                _db.UpdateRecordDocumento(DocFile);

                // controllare se bisogna salvare il valore inserito per l'autocomletamento
                foreach (var col in DocFile.Voci)
                {
                    if (col.IsAutocomplete)
                    {
                        var auto = new Autocompletamento();
                        auto.Colonna = col.Id;
                        auto.Valore = col.Value;
                        _db.InsertRecordAutocompletamento(auto);
                    }
                }
                Task.Factory.StartNew(() =>
                {
                    DatabaseHelper maindb = new DatabaseHelper();
                    Batch.UltimoIndicizzato = DocFiles.CurrentIndex;
                    maindb.UpdateRecordBatch(Batch);
                });

                if (!File.Exists(Path.Combine(Batch.DirectoryOutput, DocFile.FileName)))
                    Utility.CopiaPdf(DocFile.Path, Batch.DirectoryOutput, DocFile.FileName + ".pdf");
                MoveNextItem();
            }
        }

        public void MovePreviousItem()
        {
            if (DocFiles.hasPrevious)
            {
                DocFile = new Document(Batch, DocFiles.MovePrevious);
                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, _db);
            }

            RaisePropertyChanged("DocFile");
        }

        public void MoveNextItem()
        {
            if (DocFiles.hasNext)
            {
                DocFile = new Document(Batch, DocFiles.MoveNext);
                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, _db);
            }
            
            RaisePropertyChanged("DocFile");
        }

        public void Interrompi()
        {
            Batch.DocCorrente = DocFiles.CurrentIndex;

            #if DEBUG
            Console.WriteLine("Documento corrente: " + Batch.DocCorrente);
            #endif

            DatabaseHelper maindb = new DatabaseHelper();
            maindb.UpdateRecordBatch(Batch);
            CloseWindow(true);
        }

        #region Command

        private RelayCommand _cmdPrev;
        public ICommand CmdPrev
        {
            get
            {
                if (_cmdPrev == null)
                {
                    _cmdPrev = new RelayCommand(param => MovePreviousItem(), param => CanMovePrevious);
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
                    _cmdNext = new RelayCommand(param => MoveNextItem(), param => CanMoveNext);
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
                    _cmdIndex = new RelayCommand(param => Indexes());
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
                    _cmdStop = new RelayCommand(param => Interrompi());
                }
                return _cmdStop;
            }
        }

        #endregion
    }
}