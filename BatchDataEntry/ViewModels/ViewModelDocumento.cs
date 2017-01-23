﻿using System;
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

        public ViewModelDocumento(Batch _currentBatch, Records rec, string indexRowVal)
        {        
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            LoadDocsList();
            //LoadRecords(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));
            Records = rec;
            DocFiles.CurrentIndex = getId(indexRowVal);
            DocFile = DocFiles.Current;
            DocFile.AddInputsToPanel(Batch, _db);
            RaisePropertyChanged("DocFile");
            CheckFile();
        }

        public ViewModelDocumento(Batch _currentBatch, Records rec)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione.Campi == null)
                Batch.Applicazione.LoadCampi();          
            LoadDocsList();
            Records = rec;
            DocFiles.CurrentIndex = getId();
            DocFile = DocFiles.Current;
            DocFile.AddInputsToPanel(Batch, _db);
            RaisePropertyChanged("DocFile");
            CheckFile();
        }

        private void LoadDocsList()
        {
            DatabaseHelper dbCache = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            DocFiles = dbCache.GetDocuments();
            RaisePropertyChanged("DocFiles");
        }

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
                     _db.UpdateRecord(DocFile); // Aggiorna il Path nel cachedb
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
                DocFile.IsIndexed = true;
                

                _db.UpdateRecord(DocFile);
                
                // controllare se bisogna salvare il valore inserito per l'autocomletamento
                int id = Records.isRecordAlreadyInserted(DocFile);
                if (!(id >= 0))
                    Records.AddRecord(DocFile);
                else
                    Records.UpdateRecord(DocFile, id);

                foreach (var col in DocFile.Voci)
                {
                    if (col.IsAutocomplete)
                    {
                        var auto = new Autocompletamento();
                        auto.Colonna = col.Id;
                        auto.Valore = col.Value;
                        _db.InsertRecord(auto);
                    }
                }
                if(!File.Exists(Path.Combine(Batch.DirectoryOutput, DocFile.FileName)))
                    Utility.CopiaPdf(DocFile.Path, Batch.DirectoryOutput, (DocFile.FileName + ".pdf"));
                SaveFile();
                MoveNextItem();
            }
        }

        public void MovePreviousItem()
        {
            if (DocFiles.hasPrevious)
            {
                DocFile = DocFiles.MovePrevious;
                if(DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, _db);
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
                    DocFile.AddInputsToPanel(Batch, _db);
            }
            CheckFile();
            RaisePropertyChanged("DocFile");
        }

        public void Interrompi()
        {
            SaveFile();
            this.CloseWindow(true);
        }

        public void SaveFile()
        {
            if (Records != null && Records.Rows.Count > 0)
                Records.Save(Path.Combine(Batch.DirectoryOutput, ConfigurationManager.AppSettings["bin_file_name"]));           
        }
    }
}
