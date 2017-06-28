using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BatchDataEntry.Business;
using NLog;
using Batch = BatchDataEntry.Models.Batch;
using BatchDataEntry.Abstracts;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelNewBatch : ViewModelBase
    {
#region Attributes
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private bool _alreadyExist;
        private bool _saved = false;
        private AbsDbHelper db = null;

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("isVisible");
                }
            }
        }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RaisePropertyChanged("Progress");
                }
            }
        }

        private int _maxVal;
        public int MaxValue
        {
            get { return _maxVal; }
            set
            {
                if (_maxVal != value)
                {
                    _maxVal = value;
                    RaisePropertyChanged("MaxValue");
                }
            }
        }

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

        private bool _isTiffSubdirs;
        public bool IsTiffSubdirs
        {
            get { return _isTiffSubdirs; }
            set
            {
                if (_isTiffSubdirs != value)
                {
                    _isTiffSubdirs = value;
                    RaisePropertyChanged("IsTiffSubdirs");
                }
            }
        }

        private IEnumerable<Modello> _models;
        public IEnumerable<Modello> Models
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

        private string _indexType;
        public string IndexType
        {
            get { return _indexType; }
            set
            {
                if (_indexType != value)
                {
                    _indexType = value;
                    RaisePropertyChanged("IndexType");
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
                if (!_saved &&
                    !string.IsNullOrEmpty(CurrentBatch.Nome) &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryInput) &&
                    !string.IsNullOrEmpty(CurrentBatch.DirectoryOutput))
                    return true;
                else
                    return false;
            }
        }

#endregion

        public ViewModelNewBatch()
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
            IsTiffSubdirs = false;
        }

        public ViewModelNewBatch(Batch batch)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
            IsTiffSubdirs = false;
        }

        public ViewModelNewBatch(AbsDbHelper dbs)
        {
            CurrentBatch = new Batch();
            _alreadyExist = false;
            db = dbs;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
            IsTiffSubdirs = false;
        }

        public ViewModelNewBatch(Batch batch, AbsDbHelper dbs)
        {
            CurrentBatch = batch;
            _alreadyExist = true;
            db = dbs;
            PopulateComboboxModels();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            IsVisible = false;
            IsTiffSubdirs = false;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            DatabaseHelper dbc = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], CurrentBatch.DirectoryOutput);
            Generate(CurrentBatch, dbc);
            //backgroundWorker.ReportProgress(i);
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            Progress = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsVisible = false;
            MessageBox.Show("Finito");
        }

        public void AddOrUpdateBatchItem()
        {
            IsVisible = true;
            Progress = 0;
            MaxValue = 100;
            if (_alreadyExist)
            {
                // Il batch è già esistente e quindi si effettua un update              
                Batch batch = new Batch(CurrentBatch, db);
                batch.UltimoIndicizzato = 0;
                db.Update(batch);          
            }
            else
            {
                Batch batch = new Batch(CurrentBatch, db);
                db.Insert(batch);     
            }
            RaisePropertyChanged("Batches");
            backgroundWorker.RunWorkerAsync();
            _saved = true;
        }

        public void PopulateComboboxModels()
        {
            if (db == null) return;
            Models = db.IEnumerableModelli();
            RaisePropertyChanged("Models");
        }

        #region CacheFilesInit

        public bool FillCacheDb(DatabaseHelper dbcache, AbsDbHelper dbdata, Batch b)
        {
            List<string> files = new List<string>();
            if (CurrentBatch.Applicazione.Id == 0 && dbdata == null) CurrentBatch.LoadModel(dbdata);
            else if (CurrentBatch.Applicazione.Id == 0) CurrentBatch.LoadModel(dbdata);
            if (CurrentBatch.Applicazione.Campi.Count == 0) CurrentBatch.Applicazione.LoadCampi(dbdata);

            // Genera il file di cache partendo dal file csv invece che dalla lista all'interno della cartella.
            if (IndexType.Contains("Automatica"))
            {
                /*
                 Per funzionare richiede un Csv in input e popola il database in base al contenuto del csv
                 */
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Automatica ###");
                #endif
                if (!CurrentBatch.Applicazione.OrigineCsv)
                {
                    logger.Error("Richiesto file CSV per la modalità Automatica");
                    return false;
                }
                    
                int indexColumn = 0;

                for (int i = 0; i < CurrentBatch.Applicazione.Campi.Count; i++)
                {
                    if (CurrentBatch.Applicazione.Campi[i].IndicePrimario)
                    {
                        indexColumn = CurrentBatch.Applicazione.Campi[i].Posizione;
                        break;
                    }                       
                }
                
                List<string> lines = Csv.ReadRows(CurrentBatch.Applicazione.PathFileCsv, Convert.ToChar(_currentBatch.Applicazione.Separatore));
                MaxValue = lines.Count;

                for (int i = 0; i < lines.Count; i++)
                {
                    // Creo il documento
                    var doc = GenerateDocument(lines[i], i, b, dbcache, indexColumn);
                    if (doc == null)
                    {
                        logger.Error("Documento " + i + " errore generazione [null]");
                        continue;
                    }
                    else if (doc.Equals(new Document()))
                    {
                        logger.Warn("Il documento " + i + " risulta un documento vuoto");
                    }
                                    
                    // Copia del pdf corrispondente nella cartella di output
                    if (!string.IsNullOrEmpty(CurrentBatch.PatternNome))
                    {
                        if(b.TipoFile == TipoFileProcessato.Pdf)
                            Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".pdf"));
                        else
                            Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".tif"));
                    }
                    else
                    {
                        if(b.TipoFile == TipoFileProcessato.Pdf)
                            Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, doc.FileName + ".pdf");
                        else
                            Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, doc.FileName + ".tif");
                    }
                        
                    dbcache.InsertRecordDocumento(b, doc);
                    backgroundWorker.ReportProgress(i);
                }
            }
            else if (IndexType.Contains("Manuale"))
            {
                /*
                 La modalità manuale: Controlla la directory sorgente popola in database di cache in base ai file
                 TIFF o PDF (in base alla selezione).
                 */

                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Manuale ###");
                #endif

                if (b.TipoFile == TipoFileProcessato.Pdf)
                    files = Directory.GetFiles(b.DirectoryInput, "*.pdf").ToList();
                else
                {
                    if(IsTiffSubdirs)
                        files = Directory.GetDirectories(b.DirectoryInput).ToList();
                    else
                        files = Directory.GetFiles(b.DirectoryInput, "*.tif").ToList();
                }
                    
                // ATTUALMENTE file divisi per subdir solo in modalità manuale
                if (files.Count == 0) return false;

                files = files.CustomSort().ToList();
                MaxValue = files.Count;

                // adesso per ogni file in files aggiungere un record fileName, path, false
                int lastId = 0;
                try
                {
                    lastId = dbcache.Count(@"SELECT seq FROM SQLITE_SEQUENCE WHERE name='Documenti'");                    
                }
                catch (Exception)
                {
                    logger.Warn("Query fallita per il recuper del lastId");
                    throw;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    string queryCheck = string.Format("SELECT Count(Id) FROM Documenti WHERE FileName = '{0}'", Path.GetFileNameWithoutExtension(files[i]));
                    if (dbcache.Count(queryCheck) > 0) continue;    // il file esiste già nel db
                    string dirpages = files[i];

                    if(this.IsTiffSubdirs)
                    {
                        List<string> pages = Directory.GetFiles(dirpages, "*.tif").ToList(); // Percorsi completi di path
                        if (pages.Count == 0) continue;
                        string newfile = Path.Combine(b.DirectoryOutput, string.Format("{0}.tif", Path.GetFileNameWithoutExtension(dirpages)));
                        mergeTiffPages(newfile, pages.ToArray());
                        dirpages = newfile;
                    }   
                    // Genera documento
                    Document doc = GenerateDocument(dirpages, lastId, b);

                    dbcache.InsertRecordDocumento(b, doc);
                    backgroundWorker.ReportProgress(i);
                    lastId++;
                }
            }
            else if (IndexType.Contains("Eurobrico")) {
                #if DEBUG
                Console.WriteLine(@"### USO GENERAZIONE Eurobrico ###");
                #endif
                if (!CurrentBatch.Applicazione.OrigineCsv)
                {
                    MessageBox.Show("Il file Csv deve essere composto da codice ean;filename");
                    return false;
                }
                List<string> lines = Csv.ReadRows(CurrentBatch.Applicazione.PathFileCsv, Convert.ToChar(_currentBatch.Applicazione.Separatore));
                files = Directory.GetFiles(b.DirectoryInput, "*.tif").ToList();

                // Recuper l'ultimo indice inserto se il batch è già esistente
                int lastId = 0;
                try
                {
                    lastId = dbcache.Count(@"SELECT seq FROM SQLITE_SEQUENCE WHERE name='Documenti'");
                }
                catch (Exception)
                {
                    logger.Warn("Query fallita per il recuper del lastId");
                    throw;
                }

                if (files.Count() != lines.Count()) MessageBox.Show(string.Format("Attenzione! File csv contine {0} records, Mentre la directory contiene {1} tif", lines.Count(), files.Count()));
                
                for (int x = 0; x < lines.Count(); x++)
                {
                    var doc = GenerateDocumentEurob(lines[x], lastId, b, dbcache);
                    if (doc == null)
                    {
                        logger.Error("Documento " + x + " errore generazione [null]");
                        continue;
                    }
                    else if(doc.Equals(new Document()))
                    {
                        logger.Warn("Il documento " + x + " risulta un documento vuoto");
                    }

                    // Copia il file nella directory di output con il nome del file == codice ean
                    if(b.DirectoryInput.Equals(b.DirectoryOutput))
                    {
                        if (!string.IsNullOrEmpty(CurrentBatch.PatternNome))
                        {
                            if (b.TipoFile == TipoFileProcessato.Pdf)
                            {
                                string newname = string.Format("{0}{1}.pdf", b.PatternNome, doc.FileName);
                                File.Move(doc.Path, Path.Combine(CurrentBatch.DirectoryOutput, newname));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, newname);
                            }
                            else
                            {
                                string newname = string.Format("{0}{1}.tif", b.PatternNome, doc.FileName);
                                File.Move(doc.Path, Path.Combine(CurrentBatch.DirectoryOutput, newname));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, newname);
                            }
                        }
                        else
                        {
                            if (b.TipoFile == TipoFileProcessato.Pdf)
                            {
                                File.Move(doc.Path, Path.Combine(CurrentBatch.DirectoryOutput, Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".pdf")));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".pdf");
                            }  
                            else
                            {
                                File.Move(doc.Path, Path.Combine(CurrentBatch.DirectoryOutput, Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".tif")));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".tif");
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CurrentBatch.PatternNome))
                        {
                            if (b.TipoFile == TipoFileProcessato.Pdf)
                            {
                                Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".pdf"));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".pdf"));
                            }                         
                            else{
                                Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".tif"));
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, string.Format("{0}{1}", CurrentBatch.PatternNome, doc.FileName + ".tif"));
                            }  
                        }
                        else
                        {
                            if (b.TipoFile == TipoFileProcessato.Pdf)
                            {
                                Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, doc.FileName + ".pdf");
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".pdf");
                            } 
                            else{
                                Utility.CopiaFile(doc.Path, CurrentBatch.DirectoryOutput, doc.FileName + ".tif");
                                doc.Path = Path.Combine(CurrentBatch.DirectoryOutput, doc.FileName + ".tif");
                            }    
                        }
                    }

                    dbcache.InsertRecordDocumento(b, doc);
                    lastId++;
                    backgroundWorker.ReportProgress(x);
                }
            }
            else
            {
                MessageBox.Show("Metodo indicizzazione non selezionato");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Merge multiple TIFF Files into new one
        /// </summary>
        /// <param name="str_DestinationPath">The Path of the Source TIFF Files need to Merge</param>
        /// <param name="sourceFiles">The Path of the Destination Folder, the Merged file need to be saved</param>
        public void mergeTiffPages(string str_DestinationPath, string[] sourceFiles)
        {

            System.Drawing.Imaging.ImageCodecInfo codec = null;

            foreach (System.Drawing.Imaging.ImageCodecInfo cCodec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
            {
                if (cCodec.CodecName == "Built-in TIFF Codec")
                    codec = cCodec;
            }

            try
            {

                System.Drawing.Imaging.EncoderParameters imagePararms = new System.Drawing.Imaging.EncoderParameters(1);
                imagePararms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)System.Drawing.Imaging.EncoderValue.MultiFrame);

                if (sourceFiles.Length == 1)
                {
                    System.IO.File.Copy((string)sourceFiles[0], str_DestinationPath, true);

                }
                else if (sourceFiles.Length >= 1)
                {
                    System.Drawing.Image DestinationImage = (System.Drawing.Image)(new System.Drawing.Bitmap((string)sourceFiles[0]));
                    DestinationImage.Save(str_DestinationPath, codec, imagePararms);
                    imagePararms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)System.Drawing.Imaging.EncoderValue.FrameDimensionPage);

                    for (int i = 0; i < sourceFiles.Length - 1; i++)
                    {
                        System.Drawing.Image img = (System.Drawing.Image)(new System.Drawing.Bitmap((string)sourceFiles[i]));
                        DestinationImage.SaveAdd(img, imagePararms);
                        img.Dispose();
                    }

                    imagePararms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)System.Drawing.Imaging.EncoderValue.Flush);
                    DestinationImage.SaveAdd(imagePararms);
                    imagePararms.Dispose();
                    DestinationImage.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }


        }

        public Document GenerateDocument(string linerow, int i, Batch b, DatabaseHelper dbcache, int indexColumn)
        {
            Document doc = new Document();
            doc.Id = i + 1;
            try
            {
                string[] cells = linerow.Split(b.Applicazione.Separatore[0]);

                if (cells.Length == 1) // viene restituita la riga di partenza
                {
                    MessageBox.Show(
                        "Non è possibile leggere correttamente le righe del file csv di origine, sicuro di aver inserito il delimitatore di campo giusto?");
                    return new Document();
                }

                // Il nome del file corrisponde all'index (primario) impostato nel modello!
                string cleanValue = cells[indexColumn].Replace("'", "");
                string queryCheck = string.Format("SELECT Count(Id) FROM Documenti WHERE FileName = '{0}'", Path.GetFileNameWithoutExtension(cleanValue));
                if (dbcache.Count(queryCheck) > 0) return null;
                doc.FileName = cleanValue;

                doc.FileName = cells[indexColumn];
                string absPath = Path.Combine(CurrentBatch.DirectoryInput, cleanValue.Contains(".pdf") ? cleanValue : String.Format("{0}.pdf", cleanValue));
                doc.Path = absPath;
                doc.IsIndexed = true;

                for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                {
                    string colName = b.Applicazione.Campi.ElementAt(z).Nome;

                    string celValue = (z < cells.Length && !string.IsNullOrEmpty(cells[z])) ? cells[z] : "";
                    doc.Voci.Add(Record.Create(b.Applicazione.Campi.ElementAt(z), z, celValue));
                }

            }
            catch (Exception er)
            {
                logger.Error("### Init basato su file Csv esterno ### break at line: " + (i + 1));
                logger.Error(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message));
                ConsoleErrorPrint(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message), er);
                throw er;
            }
            return doc;
        }

        public Document GenerateDocumentEurob(string linerow, int i, Batch b, DatabaseHelper dbcache)
        {
            Document doc = new Document();
            doc.Id = i + 1;
            try
            {
                string[] cells = linerow.Split(';');

                if (cells.Length != 2) // viene restituita la riga di partenza
                {
                    logger.Warn("Generazione documento alla linea " + i + " potrebbe generare errori.");
                }

                // Il nome del file corrisponde all'index (primario) impostato nel modello!
                //ean;filename
                string codiceEAN = (cells[0].Length == 13) ? cells[0] : ("0" + cells[0]);
                string queryCheck = string.Format("SELECT Count(Id) FROM Documenti WHERE FileName = '{0}'", Path.GetFileNameWithoutExtension(codiceEAN));
                if (dbcache.Count(queryCheck) > 0) return null;
                doc.FileName = codiceEAN; // Il documento prende il nome del codice ean

                string absPath = Path.Combine(CurrentBatch.DirectoryInput, cells[1].Contains(".tif") ? cells[1] : String.Format("{0}.tif", cells[1]));
                doc.Path = absPath;
                doc.IsIndexed = false;
                // Aggiungo il codice ean al documento
                if (codiceEAN.Length != 13)
                    logger.Warn("Codice ean con lunghezza diversa da 13");
                doc.Voci.Add(Record.Create(b.Applicazione.Campi.ElementAt(0), 0, codiceEAN));
            }
            catch (Exception er)
            {
                logger.Error("### Init basato su file Csv esterno ### break at line: " + (i + 1));
                logger.Error(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message));
                ConsoleErrorPrint(string.Format("Error into string[] cells (Source Csv) [{0}] {1}", er.Source, er.Message), er);
                throw er;
            }
            return doc;
        }

        public Document GenerateDocument(string file, int id, Batch b)
        {
            Document doc = new Document();
            doc.Id = id + 1;
            doc.FileName = Path.GetFileNameWithoutExtension(file);
            doc.Path = file;
            doc.IsIndexed = false;

            if (!b.Applicazione.OrigineCsv)
            {
                for (int z = 0; z < b.Applicazione.Campi.Count; z++)
                {
                    string valore = (string.IsNullOrEmpty(b.Applicazione.Campi.ElementAt(z).ValorePredefinito))
                        ? ""
                        : b.Applicazione.Campi.ElementAt(z).ValorePredefinito;
                    doc.Voci.Add(Record.Create(b.Applicazione.Campi.ElementAt(z), z, valore));
                }
            }
            return doc;
        }

        public void Generate(Batch batch, DatabaseHelper dbc)
        {
            try
            {
                if (batch.Applicazione == null || batch.Applicazione.Id == 0) batch.LoadModel(db);
                if (batch.Applicazione.Campi == null || batch.Applicazione.Campi.Count == 0) batch.Applicazione.LoadCampi(db);

                List<string> campi = new List<string>();
                for (int i = 0; i < batch.Applicazione.Campi.Count; i++)
                    campi.Add(batch.Applicazione.Campi[i].Nome);

                string pathCacheDb = Path.Combine(batch.DirectoryOutput, ConfigurationManager.AppSettings["cache_db_name"]);

                if (!File.Exists(pathCacheDb))
                {                              
                    bool res = false;

                    dbc.CreateCacheDb(campi);
                    res = FillCacheDb(dbc, db, batch);
                }else if (File.Exists(pathCacheDb))
                {
                    // Appende al db cache i nuovi pdf
                    FillCacheDb(dbc, db, batch);
                }

                if (CheckOutputDirFiles(batch.DirectoryOutput))
                    CreateMissingFiles(true, batch);
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Console.WriteLine("Exception generazione cache db");
                    Console.WriteLine(ex.ToString());
                #endif
                logger.Error(ex.ToString());
            }
        }

        public void CreateMissingFiles(bool res, Batch m)
        {
            if (res)
                CreateDbCsv(m.DirectoryOutput);
        }

        public bool CheckOutputDirFiles(string output_path)
        {
            bool res = !File.Exists(Path.Combine(output_path, ConfigurationManager.AppSettings["csv_file_name"]));
            return res;
        }

        public void CreateDbCsv(string output_dir)
        {
            if (ConfigurationManager.AppSettings["csv_file_name"] == null)
                File.Create(Path.Combine(output_dir, @"db.csv"));
            else
                File.Create(Path.Combine(output_dir, ConfigurationManager.AppSettings["csv_file_name"]));
        }

        #endregion
    }
}
