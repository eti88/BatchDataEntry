using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using MadMilkman.Ini;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNewBatch : ViewModelBase
    {
        private readonly string FILENAME_CACHE = "cache.ini";
        private readonly string FILENAME_DBCSV = "db.csv";
        private readonly string FILENAME_VALUES = "autocomp.ini";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _alreadyExist;

        private Batch _currentBatch;
        public Batch CurrentBatch
        {
            get { return _currentBatch; }
            set
            {
                _currentBatch = value;
                RaisePropertyChanged("CurrentBatch");
            }
        }

        private IEnumerable<DBModels.Modello> _models;
        public IEnumerable<DBModels.Modello> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                RaisePropertyChanged("Models");
            }
        }

        private RelayCommand _applyCommand;
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(param => this.AddOrUpdateBatchItem(), param => this.CanSave);
                }
                return _applyCommand;
            }
        }

        private bool CanSave
        {
            get
            {
                if (!string.IsNullOrEmpty(CurrentBatch.Nome) && CurrentBatch.TipoFile != null &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryInput) &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryOutput))
                    return true;
                else
                    return false;
            }
        }

        public ViewModelNewBatch()
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            PopulateComboboxModels();
        }

        public ViewModelNewBatch(Batch batch)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            PopulateComboboxModels();
        }

        private void AddOrUpdateBatchItem()
        {
            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.UpdateRecord(batch);
                RaisePropertyChanged("Batches");
                bool[] resck = CheckOutputDirFiles(CurrentBatch.DirectoryOutput);
                CreateMissingFiles(resck, CurrentBatch);
            }
            else
            {
                DatabaseHelper db = new DatabaseHelper();
                DBModels.Batch batch = new DBModels.Batch(CurrentBatch);
                db.InsertRecord(batch);
                RaisePropertyChanged("Batches");
                GeneraDirOutput(CurrentBatch.DirectoryInput, CurrentBatch.DirectoryOutput, CurrentBatch.TipoFile, CurrentBatch.IdModello);
            }

            this.CloseWindow(true);
        }

        private void PopulateComboboxModels()
        {
            DatabaseHelper db = new DatabaseHelper();
            IEnumerable<DBModels.Modello> tmp = db.IEnumerableModelli();      
            Models = tmp;
            RaisePropertyChanged("Models");
        }

        protected void CreateMissingFiles(bool[] res, Batch m)
        {
            if(res.Length != 2)
                logger.Error("Lunghezza dell'array di controllo errato per il batch " + m.Nome);

            if (res[0])
            {
                CreateIniFile(m.DirectoryInput, m.DirectoryOutput, m.TipoFile);
            }

            if (res[1])
            {
                CreateDbCsv(m.DirectoryOutput);
            }

            if (res[2])
            {
                CreateAutocompleteIniFile(m.DirectoryOutput, m.IdModello);
            }
        }

        protected bool[] CheckOutputDirFiles(string output_path)
        {
            bool[] res = new bool[3];

            if (!File.Exists(Path.Combine(output_path, FILENAME_CACHE)))
                res[0] = true;
            else
                res[0] = false;

            if (!File.Exists(Path.Combine(output_path, FILENAME_DBCSV)))
                res[1] = true;
            else
                res[1] = false;

            if (!File.Exists(Path.Combine(output_path, FILENAME_VALUES)))
                res[2] = true;
            else
                res[2] = false;

            return res;
        }
       
        protected void GeneraDirOutput(string input_path, string output_path, TipoFileProcessato ext, int idModello)
        {

            // Crea la directory se assente
            if (!Directory.Exists(output_path))
            {
                Directory.CreateDirectory(output_path);
            }

            CreateIniFile(input_path, output_path, ext);
            CreateAutocompleteIniFile(output_path, idModello);
            CreateDbCsv(output_path);
        }

        protected void CreateIniFile(string input_path, string output_path, TipoFileProcessato ext)
        {
            IniFile file = new IniFile();
            IniSection docsSection = file.Sections.Add("Documenti");

            if (ext == TipoFileProcessato.Pdf)
            {
                string[] docs = Directory.GetFiles(input_path, string.Format("*.{0}", ext));

                for (int i = 0; i < docs.Length; i++)
                {
                    docsSection.Keys.Add(i.ToString(), docs[i]);
                }
            }
            else if(ext == TipoFileProcessato.Tiff)
            {
                // i tiff sono suddivisi per subdirs contenenti i tiff sotto forma di pagine
                string[] dirs = Directory.GetDirectories(input_path);
                for (int i = 0; i < dirs.Length; i++)
                {
                    docsSection.Keys.Add(i.ToString(), dirs[i]);
                }
            }

            file.Save(Path.Combine(output_path, FILENAME_CACHE));
        }

        protected void CreateDbCsv(string output_dir)
        {
            File.Create(Path.Combine(output_dir, FILENAME_DBCSV));
        }

        protected void CreateAutocompleteIniFile(string output_dir, int idModello)
        {
            IniFile file = new IniFile();
            
            DatabaseHelper db = new DatabaseHelper();
            ObservableCollection<Campo> campi = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + idModello);

            if (campi == null || campi.Count == 0)
            {
                logger.Error("Creazione del file per l'autocompletamento non riuscito! (La query non ha restituito nessuna colonna associata al modello)");
            }

            foreach (Campo campo in campi)
            {
                if(campo.SalvaValori)
                    file.Sections.Add(campo.Nome);
            }

            if(file.Sections.Count > 0)
                file.Save(Path.Combine(output_dir, FILENAME_VALUES));
        }
    }
}
