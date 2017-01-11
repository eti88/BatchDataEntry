using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using MadMilkman.Ini;
using System.Configuration;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNewBatch : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string FILENAME_CACHE = Properties.Settings.Default["filename_cache_output_dir"].ToString();
        private readonly string FILENAME_DBCSV = Properties.Settings.Default["filename_db_output_dir"].ToString();
        private readonly string FILENAME_VALUES = Properties.Settings.Default["filename_autocomplete"].ToString();
        private readonly string FILENAME_IN_CACHE = Properties.Settings.Default["filename_cache_input_dir"].ToString();

        private bool _alreadyExist;

        private Batch _currentBatch;
        public Batch CurrentBatch
        {
            get { return _currentBatch; }
            set
            {
                if (_currentBatch != value)
                {
                    _currentBatch = value;
                    RaisePropertyChanged("CurrentBatch");
                }
            }
        }

        private IEnumerable<DBModels.Modello> _models;
        public IEnumerable<DBModels.Modello> Models
        {
            get { return _models; }
            set
            {
                if(_models != value) { 
                    _models = value;
                    RaisePropertyChanged("Models");
                }
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
                bool[] resck = CheckOutputDirFiles(CurrentBatch.DirectoryInput, CurrentBatch.DirectoryOutput);
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
            if (res[3])
                GeneraDirInput(m.DirectoryInput, m.TipoFile);
        }

        protected bool[] CheckOutputDirFiles(string input_path, string output_path)
        {
            bool[] res = new bool[4];

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
            if (!File.Exists(Path.Combine(input_path, FILENAME_IN_CACHE)))
                res[3] = true;
            else
                res[3] = false;

            return res;
        }

        protected void GeneraDirInput(string input_path, TipoFileProcessato ext)
        {
            if (Directory.Exists(input_path))
            {
                string filePath = Path.Combine(input_path, FILENAME_IN_CACHE);
                Csv.CreateCsv(filePath);
                if (File.Exists(filePath))
                {
                    if (ext == TipoFileProcessato.Pdf)
                    {
                        string[] docs = Directory.GetFiles(input_path, string.Format("*.{0}", ext));
                        for (int i = 0; i < docs.Length; i++)
                        {
                            string row = string.Format("{0};{1}", Path.GetFileNameWithoutExtension(docs[i]),false);
                            Csv.AddRow(filePath, row);
                        }
                    }
                    else if (ext == TipoFileProcessato.Tiff)
                    {
                        string[] dirs = Directory.GetDirectories(input_path);
                        for (int i = 0; i < dirs.Length; i++)
                        {

                            string row = string.Format("{0};{1}", new DirectoryInfo(dirs[i]).Name, false);
                            Csv.AddRow(filePath, row);
                        }
                    } 
                }
            }
        }

        protected void GeneraDirOutput(string input_path, string output_path, TipoFileProcessato ext, int idModello)
        {

            // Crea la directory se assente
            if (!Directory.Exists(output_path))
            {
                Directory.CreateDirectory(output_path);
            }
            GeneraDirInput(input_path, ext);
            CreateIniFile(input_path, output_path, ext);
            CreateAutocompleteIniFile(output_path, idModello);
            CreateDbCsv(output_path);
        }

        protected void CreateIniFile(string input_path, string output_path, TipoFileProcessato ext)
        {
            string fileOutputPathCache = Path.Combine(output_path, FILENAME_CACHE);
            //string fileOutputPathDb = Path.Combine(output_path, FILENAME_CACHE);

            if (Business.Cache.CreateFile(fileOutputPathCache))
            {
                // Nel file di index inserire i nomi dei file senza estensione e puliti quindi se DOC00000002 -> 00000002             
                /*
                 * Recupera i nomi dei file direttamente da un file csv (forse da inserire più avanti)
                 * string[] fileNames = Business.Csv.ReadColumn(output_path, primaryIndex).ToArray();
                 */

                Business.Cache.AddSection(fileOutputPathCache, "Documenti");

                if (ext == TipoFileProcessato.Pdf)
                {
                    string[] docs = Directory.GetFiles(input_path, string.Format("*.{0}", ext));
                    string[] fileNames = new string[docs.Length];

                    for (int i = 0; i < docs.Length; i++)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(docs[i]);
                        if (!Business.Utility.ContainsOnlyNumbers(fileName))
                        {
                            fileName = Business.Utility.RemovePatternFromString(fileName, "DOC");
                        }
                        
                        fileNames[i] = fileName;
                    }

                    Business.Cache.AddMultipleKeyToSection(fileOutputPathCache, "Documenti", fileNames, docs);
                }
                else if (ext == TipoFileProcessato.Tiff)
                {
                    string[] dirs = Directory.GetDirectories(input_path);
                    string[] fileNames = new string[dirs.Length];
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        fileNames[i] = Business.Utility.RemovePatternFromString(dirs[i], "DOC");
                    }
                    
                    Business.Cache.AddMultipleKeyToSection(fileOutputPathCache, "Documenti", fileNames, dirs);

                }
            }
        }

        protected void CreateDbCsv(string output_dir)
        {
            File.Create(Path.Combine(output_dir, FILENAME_DBCSV));
        }

        protected void CreateAutocompleteIniFile(string output_dir, int idModello)
        {
            string fileName = Path.Combine(output_dir, FILENAME_VALUES);

            Business.Cache.CreateFile(fileName);
            DatabaseHelper db = new DatabaseHelper();
            ObservableCollection<Campo> campi = db.CampoQuery("SELECT * FROM Campo WHERE IdModello = " + idModello);
            List<string> colNames = new List<string>(campi.Select((campo) => campo.Nome));

            if (campi == null || campi.Count == 0)
            {
                logger.Error("Creazione del file per l'autocompletamento non riuscito! (La query non ha restituito nessuna colonna associata al modello)");
            }
            Business.Cache.AddMultipleSection(fileName, colNames);
        }
    }
}
