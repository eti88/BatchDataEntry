using BatchDataEntry.Abstracts;
using BatchDataEntry.Business;
using BatchDataEntry.Components;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using BatchDataEntry.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelTools : ViewModelBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private AbsDbHelper dbinfo;
        private DatabaseHelper dbcache;
        private bool needReload = false;
        private Batch _batch;

        private String _dtformat;
        public String DateFormat
        {
            get
            {
                return _dtformat;
            }
            set
            {
                if (_dtformat != value)
                {
                    _dtformat = value;
                    RaisePropertyChanged("DateFormat");
                }

            }
        }

        private String _inputFilePath;
        public String InputFilePath
        {
            get
            {
                return _inputFilePath;
            }
            set
            {
                if (_inputFilePath != value)
                {
                    _inputFilePath = value;
                    RaisePropertyChanged("InputFilePath");
                }

            }
        }

        private bool _genOutFile;
        public bool GenerateOutputFile
        {
            get
            {
                return _genOutFile;
            }
            set
            {
                if (_genOutFile != value)
                {
                    _genOutFile = value;
                    RaisePropertyChanged("GenerateOutputFile");
                }

            }
        }

        private bool _checkEmptyVal;
        public bool CheckEmpty
        {
            get
            {
                return _checkEmptyVal;
            }
            set
            {
                if (_checkEmptyVal != value)
                {
                    _checkEmptyVal = value;
                    RaisePropertyChanged("CheckEmpty");
                }

            }
        }

        private ObservableCollection<ErrorRecord> _errorRecordList;
        public ObservableCollection<ErrorRecord> ErrorRecordList
        {
            get
            {
                return _errorRecordList;
            }
            set
            {
                if (_errorRecordList != value)
                {
                    _errorRecordList = value;
                    RaisePropertyChanged("ErrorRecordList");
                }

            }
        }

        public List<FidelityClient> Records { get; set; }

        private NavigationList<Dictionary<int, string>> DocumentsWithErrors;

        private string _associato;
        public string CodiceAssociato
        {
            get
            {
                return _associato;
            }
            set
            {
                if (_associato != value)
                {
                    _associato = value;
                    RaisePropertyChanged("CodiceAssociato");
                }

            }
        }

        private string _cliente;
        public string CodiceNegozio
        {
            get
            {
                return _cliente;
            }
            set
            {
                if (_cliente != value)
                {
                    _cliente = value;
                    RaisePropertyChanged("CodiceNegozio");
                }

            }
        }

        private RelayCommand _checkCommand;
        public ICommand CheckCommand
        {
            get
            {
                if (_checkCommand == null)
                {
                    _checkCommand = new RelayCommand(param => this.CheckFile());
                }
                return _checkCommand;
            }
        }

        private RelayCommand _genCommand;
        public ICommand GenerateCommand
        {
            get
            {
                if (_genCommand == null)
                {
                    _genCommand = new RelayCommand(param => this.GeneraFiles());
                }
                return _genCommand;
            }
        }

        private RelayCommand _reloadCommand;
        public ICommand CmdSelectedPath
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand(param => this.NeedReloadFile());
                }
                return _reloadCommand;
            }
        }

        private RelayCommand _correctCommand;
        public ICommand CorrectCommand
        {
            get
            {
                if (_correctCommand == null)
                {
                    _correctCommand = new RelayCommand(param => this.StartErrorCorrection(), param => this.CanCorrect);
                }
                return _correctCommand;
            }
        }

        private bool CanCorrect
        {
            get { return (DocumentsWithErrors != null) && DocumentsWithErrors.Count > 0; }
        }

        public ViewModelTools()
        {

        }

        public ViewModelTools(AbsDbHelper db) {
            DateFormat = "dd-MM-yyyy"; // Set default date format
            GenerateOutputFile = false;
            CheckEmpty = true;
            ErrorRecordList = new ObservableCollection<ErrorRecord>();
            dbinfo = db;
            // Genero un batch temporaneo
            var tmods = dbinfo.GetModelloRecords();
            Modello mod = tmods.SingleOrDefault(x => x.Nome.ToLower().Contains("eurobrico"));
            if (mod == null) logger.Error("Impossibile trovare modello eurobrico [ViewTools Constructor]");
            _batch = new Batch();
            _batch.IsTemp = true;
            _batch.Nome = "TempBatch";
            _batch.Applicazione = mod;
            _batch.Applicazione.LoadCampi(dbinfo);

            var settings = Properties.Settings.Default;
            if(!string.IsNullOrEmpty(settings.LastAssociato))
                CodiceAssociato = settings.LastAssociato;

            if (!string.IsNullOrEmpty(settings.LastNegozio))
                CodiceNegozio = settings.LastNegozio;

            DocumentsWithErrors = new NavigationList<Dictionary<int, string>>();
        }

        public void NeedReloadFile()
        {
            needReload = true;
        }

        private void CleanErrorList()
        {
            if (ErrorRecordList != null && ErrorRecordList.Count > 0)
                ErrorRecordList.Clear();
        }

        // Carica il file sqlite nel modello FidelityCard
        public void LoadFile() {
            FileAttributes attr = File.GetAttributes(InputFilePath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                List<string> qfiles = Directory.GetFiles(InputFilePath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".db3")).ToList();
                if (qfiles.Count == 0)
                    MessageBox.Show("Impossibile recupeare database dalla directory selezionata");
                else
                    InputFilePath = qfiles.ElementAt(0);
            }
                
            if(!string.IsNullOrEmpty(InputFilePath) && File.Exists(InputFilePath))
            {
                dbcache = new DatabaseHelper(InputFilePath);
                Records = dbcache.GetRecords();
                needReload = false;
                Log("# Caricati " + Records.Count + " records");
            }

            _batch.DirectoryInput = Path.GetDirectoryName(dbcache.GetDocumento(1).ElementAt(1).Value);
            _batch.DirectoryOutput = Path.GetDirectoryName(InputFilePath);
        }

        public void CheckFile() {
            if (ErrorRecordList.Count > 0) ErrorRecordList.Clear();
            if (dbcache == null || needReload) LoadFile();

            if(Records.Count > 0)
            {
                Log("# Inizio Check Campi");
                foreach(FidelityClient record in Records)
                {
                    if (GenerateOutputFile)
                    {
                        string dir = Path.GetDirectoryName(InputFilePath);
                        string filename = "check-report.txt";
                        Check(Path.Combine(dir, filename));
                    }
                    else
                    {
                        Check();
                    }
                }
                DocumentsWithErrors = GetSublistDocumentWithErrors(ref _errorRecordList);
            }
        }

        private void Check(string path)
        {
            CleanErrorList();
            if (File.Exists(path)) File.Delete(path);
            using(var file = new StreamWriter(path, true))
            {
                file.WriteLine("File\tErrori");
                foreach(FidelityClient r in Records)
                        file.WriteLine(r.Checks(CheckEmpty, DateFormat).ToString());
            }
        }

        private void Check()
        {

            CleanErrorList();

            if (Records.Count > 0)
            {
                foreach (FidelityClient r in Records)
                {
                    var err = r.Checks(CheckEmpty, DateFormat);
                    if(err != null)
                        ErrorRecordList.Add(err);                      
                }
            }
        }

        public void GeneraFiles() {
            if (ErrorRecordList.Count > 0) ErrorRecordList.Clear();
            if (dbcache == null || needReload) LoadFile();

            if (string.IsNullOrEmpty(CodiceAssociato) || string.IsNullOrEmpty(CodiceNegozio))
            {
                MessageBox.Show("Codice Associato o Codice Negozio vuoti");
                return;
            }

            // Salvo i valori correnti
            Properties.Settings.Default.LastAssociato = CodiceAssociato;
            Properties.Settings.Default.LastNegozio = CodiceNegozio;
            Properties.Settings.Default.Save();

            string dir = Path.GetDirectoryName(InputFilePath);
            string fileFID = string.Format("FID{0}{1}{2}.FID", CodiceAssociato, CodiceNegozio, DateTime.Now.ToString("yyyyMMdd"));
            string fileCHK = string.Format("FID{0}{1}{2}.CHK", CodiceAssociato, CodiceNegozio, DateTime.Now.ToString("yyyyMMdd"));
            GeneraCHK(dir, fileCHK);
            GeneraFID(dir, fileFID, CodiceAssociato, CodiceNegozio);
        }

        private void GeneraFID(string dir, string filename, string codiceassociato, string codicenegozio) {
            string path = Path.Combine(dir, filename);
            if (File.Exists(path)) File.Delete(path);

            using (var file = new StreamWriter(path, true))
            {
                foreach(FidelityClient fi in Records)
                {
                    fi.CodiceAssociato = codiceassociato;
                    fi.CodiceNegozio = codicenegozio;
                    file.WriteLine(fi.PrintRecord());
                }
            }
        }

        private void GeneraCHK(string dir, string filename) {
            string path = Path.Combine(dir, filename);
            if (File.Exists(path)) File.Delete(path);

            using (var file = new StreamWriter(path, true))
            {
                int len = 10;
                file.WriteLine(Records.Count.ToString("D"+len));
            }
        }

        private string Line(string num, string campo, string valore, string tipo, string msg)
        {
            return string.Format("[{0}]\t{1}\t{2}\t{3}\t{4}", num,campo,valore,tipo,msg);
        }

        public void StartErrorCorrection()
        {
            if(DocumentsWithErrors != null && DocumentsWithErrors.Count > 0)
            {
                var continua = new Documento();
                continua.DataContext = new ViewModelDocumento(_batch, dbinfo, DocumentsWithErrors);
                continua.ShowDialog();
                CleanErrorList();
                LoadFile();
            }
        }

        private NavigationList<Dictionary<int, string>> GetSublistDocumentWithErrors(ref ObservableCollection<ErrorRecord> errors)
        {
            NavigationList<Dictionary<int, string>> sublist = new NavigationList<Dictionary<int, string>>();
            NavigationList<Dictionary<int, string>> tmp = new NavigationList<Dictionary<int, string>>();
            if (dbcache == null) LoadFile();
            try
            {
                tmp = dbcache.GetDocuments();
                List<string> derr = new List<string>();

                foreach(var err in errors)
                {
                    if (!derr.Contains(err.RecordNumber))
                        derr.Add(err.RecordNumber);
                }

                if(tmp.Count > 0 && derr.Count() > 0)
                {
                    // campo 1 per il nome
                    foreach(string error in derr)
                    {
                        var found = tmp.Find(x => x[1] == error);
                        sublist.Add(found);
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
                throw e;
            }
            return sublist;
        }

        [Conditional("DEBUG")]
        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    /// <summary>
    /// Classe utilizzata per la tipologia di modello già definita
    /// </summary>
    public class FidelityClient
    {

        #region Attributi
        private static string _associatedCode;
        public string CodiceAssociato
        {
            get { return _associatedCode; }
            set
            {
                if (value.Length == 5)
                    _associatedCode = value;
            }
        }

        private static string _shopCode;
        public string CodiceNegozio
        {
            get { return _shopCode; }
            set
            {
                if (value.Length == 5)
                    _shopCode = value;
            }
        }

        private string _file;
        public string FileName
        {
            get { return _file; }
            set
            {
                _file = value;
            }
        }

        private string _card;
        public string Card
        {
            get { return _card; }
            set
            {
                if (value.Length == 13)
                    _card = value;
            }
        }

        private string _cognome;
        public string Cognome
        {
            get { return _cognome; }
            set
            {
                if(value.Length <= 30)
                _cognome = value;
            }
        }

        private string _nome;
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (value.Length <= 30)
                    _nome = value;
            }
        }

        private string _indirizzo;
        public string Indirizzo
        {
            get { return _indirizzo; }
            set
            {
                if (value.Length <= 50)
                    _indirizzo = value;
            }
        }

        private string _civico;
        public string Civico
        {
            get { return _civico; }
            set
            {
                if (value.Length <= 5)
                    _civico = value;
            }
        }

        private string _localita;
        public string Localita
        {
            get { return _localita; }
            set
            {
                if (value.Length <= 50)
                    _localita = value;
            }
        }

        private string _provincia;
        public string Provincia
        {
            get { return _provincia; }
            set
            {
                if (value.Length <= 2)
                    _provincia = value;
            }
        }

        private string _cap;
        public string Cap
        {
            get { return _cap; }
            set
            {
                if (value.Length <= 5)
                    _cap = value;
            }
        }

        private string _prefisso;
        public string Prefisso
        {
            get { return _prefisso; }
            set
            {
                if (value.Length <= 4)
                    _prefisso = value;
            }
        }

        private string _telefono;
        public string Telefono
        {
            get { return _telefono; }
            set
            {
                if (value.Length <= 15)
                    _telefono = value;
            }
        }

        private string _cellulare;
        public string Cellulare
        {
            get { return _cellulare; }
            set
            {
                if (value.Length <= 15)
                    _cellulare = value;
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (value.Length <= 30)
                    _email = value;
            }
        }

        private string _datanascita;
        public string DataNascita
        {
            get { return _datanascita; }
            set
            {
                    _datanascita = value;
            }
        }

        private string _luogo;
        public string Luogo
        {
            get { return _luogo; }
            set
            {
                if (value.Length <= 30)
                    _luogo = value;
            }
        }

        // Campi richiesti ma non più utilizzati
        #region CampiFissi
        private int _sesso;                     // 0
        public int Sesso
        {
            get { return _sesso; }
            set
            {
                _sesso = value;
            }
        }

        private int _famigliari;                // 0
        public int Famigliari
        {
            get { return _famigliari; }
            set
            {
                _famigliari = value;
            }
        }

        private string _statocivile;            // 00
        public string StatoCivile
        {
            get { return _statocivile; }
            set
            {
                _statocivile = value;
            }
        }

        private string _casapropieta;           // 00
        public string Casa
        {
            get { return _casapropieta; }
            set
            {
                _casapropieta = value;
            }
        }

        private string _professione;            // 00
        public string Professione
        {
            get { return _professione; }
            set
            {
                _professione = value;
            }
        }

        private string _animali;                // 00
        public string Animali
        {
            get { return _animali; }
            set
            {
                _animali = value;
            }
        }

        private string _studio;                 // 00
        public string Studio
        {
            get { return _studio; }
            set
            {
                _studio = value;
            }
        }

        private string _interessi;              // 00
        public string Interessi
        {
            get { return _interessi; }
            set
            {
                _interessi = value;
            }
        }
        #endregion

        public bool PubbDiretta;
        public bool AccettaSMS;
        public bool AccettaEmail;
        #endregion

        // Consts
        private const string TAGEmpty = "[VUOTO]";
        private const string TAGInvalid = "[INVALIDO]";

        public FidelityClient()
        {
            Sesso = 0;
            Famigliari = 0;
            StatoCivile = "00";
            Casa = "00";
            Professione = "00";
            Animali = "00";
            Studio = "00";
            Interessi = "00";
        }

        public FidelityClient(string associato, string negozio)
        {
            CodiceAssociato = associato;
            CodiceNegozio = negozio;
            Sesso = 0;
            Famigliari = 0;
            StatoCivile = "00";
            Casa = "00";
            Professione = "00";
            Animali = "00";
            Studio = "00";
            Interessi = "00";
        }

        public FidelityClient(string associato, string negozio, string codice, 
            string cognome, string nome, string indirizzo, string civico, string localita,
            string provincia, string cap, string prefisso, string telefono, string cellulare,
            string email, string data, string luogo)
        {
            CodiceAssociato = associato;
            CodiceNegozio = negozio;
            Card = codice;
            Cognome = cognome;
            Nome = nome;
            Indirizzo = indirizzo;
            Civico = civico;
            Localita = localita;
            Provincia = provincia;
            Cap = cap;
            Prefisso = prefisso;
            Telefono = telefono;
            Cellulare = cellulare;
            Email = email;
            DataNascita = data;
            Luogo = luogo;
            Sesso = 0;
            Famigliari = 0;
            StatoCivile = "00";
            Casa = "00";
            Professione = "00";
            Animali = "00";
            Studio = "00";
            Interessi = "00";
            PubbDiretta = AccettaPubblicita(this.Indirizzo, this.Civico, this.Localita, this.Provincia);
            AccettaSMS = AccettaPubblicita(this.Cellulare);
            AccettaEmail = AccettaPubblicita(this.Email);
        }

        //Checks
        public ErrorRecord Checks(bool checkVoid, string dateformat)
        {
            var errors = new List<string>();
            if(checkVoid)
            {
                if (!Utility.IsNotVoid(this.Card))  errors.Add(string.Format("FidelityCard {0}", TAGEmpty));
                if (!Utility.IsNotVoid(this.Cognome)) errors.Add(string.Format("Cognome {0}", TAGEmpty));
                if (!Utility.IsNotVoid(this.Indirizzo)) errors.Add(string.Format("Indirizzo {0}", TAGEmpty));
                if (!Utility.IsNotVoid(this.Civico)) errors.Add(string.Format("Civico {0}", TAGEmpty));
                if (!Utility.IsNotVoid(this.Localita)) errors.Add(string.Format("Localita {0}", TAGEmpty));
                if (!Utility.IsNotVoid(this.Provincia)) errors.Add(string.Format("Provincia {0}", TAGEmpty));
            }

            if (Utility.IsNotVoid(this.Cellulare)) {
                if (!Utility.IsValidTelephone(this.Cellulare)) errors.Add(string.Format("Cellulare {0}", TAGInvalid));
            }

            if (Utility.IsNotVoid(this.Email))
            {
                if (!Utility.IsValidEmail(this.Email)) errors.Add(string.Format("Email {0}", TAGInvalid));
            }

            if (Utility.IsNotVoid(this.DataNascita)) {
                if(!Utility.IsValidDate(this.DataNascita, dateformat)) errors.Add(string.Format("DataDiNascita {0}", TAGInvalid));
                if (!Utility.IsValidDateRange(this.DataNascita, dateformat)) errors.Add(string.Format("DataDiNascita {0}", TAGInvalid));
            }

            return (errors.Count > 0) ? new ErrorRecord(this.FileName, String.Join(", ", errors)) : null;
        }

        // Controlla se i campi hanno un valore e nel caso imposta la pubblicità Diretta
        public bool AccettaPubblicita(string indirizzo, string cv, string localita, string provincia)
        {
            bool c1 = !string.IsNullOrEmpty(indirizzo) || !string.IsNullOrWhiteSpace(indirizzo);
            bool c2 = !string.IsNullOrEmpty(cv) || !string.IsNullOrWhiteSpace(cv);
            bool c3 = !string.IsNullOrEmpty(localita) || !string.IsNullOrWhiteSpace(localita);
            bool c4 = !string.IsNullOrEmpty(provincia) || !string.IsNullOrWhiteSpace(provincia);
            return c1 && c2 && c3 && c4;
        }

        // Controlla se il campo telefono e/o quello email sono vuoti
        public bool AccettaPubblicita(string txt) { return !string.IsNullOrEmpty(txt) || !string.IsNullOrWhiteSpace(txt); }

        public string ConvertiBool(bool v)
        {
            return (v) ? "S" : "N";
        }

        public string PrintRecord()
        {
            StringBuilder sb = new StringBuilder();

            PubbDiretta = AccettaPubblicita(this.Indirizzo, this.Civico, this.Localita, this.Provincia);
            AccettaSMS = AccettaPubblicita(this.Cellulare);
            AccettaEmail = AccettaPubblicita(this.Email);

            sb.Append(CodiceAssociato.PadRight(5));
            sb.Append("\t");
            sb.Append(CodiceNegozio.PadRight(5));
            sb.Append("\t");
            sb.Append(Card.PadRight(13));
            sb.Append("\t");
            sb.Append(Cognome);
            sb.Append("\t");
            sb.Append(Nome);
            sb.Append("\t");
            sb.Append(Indirizzo);
            sb.Append("\t");
            sb.Append(Civico);
            sb.Append("\t");
            sb.Append(Localita);
            sb.Append("\t");
            sb.Append(Provincia);
            sb.Append("\t");
            sb.Append(Cap);
            sb.Append("\t");
            sb.Append(ConvertiBool(PubbDiretta));
            sb.Append("\t");
            sb.Append(Prefisso);
            sb.Append("\t");
            sb.Append(Telefono);
            sb.Append("\t");
            sb.Append(Cellulare);
            sb.Append("\t");
            sb.Append(ConvertiBool(AccettaSMS));
            sb.Append("\t");
            sb.Append(Email);
            sb.Append("\t");
            sb.Append(ConvertiBool(AccettaEmail));
            sb.Append("\t");
            DateTime dtemp = new DateTime();
            try
            {
                dtemp = DateTime.ParseExact(DataNascita, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                dtemp = new DateTime();
            }
            if (dtemp != new DateTime())
                sb.Append(dtemp.ToString("yyyy-MM-dd"));
            else
                sb.Append("");

            sb.Append("\t");
            sb.Append(Luogo);
            sb.Append("\t");
            sb.Append(Sesso);
            sb.Append("\t");
            sb.Append(Famigliari);
            sb.Append("\t");
            sb.Append(StatoCivile);
            sb.Append("\t");
            sb.Append(Casa);
            sb.Append("\t");
            sb.Append(Professione);
            sb.Append("\t");
            sb.Append(Animali);
            sb.Append("\t");
            sb.Append(Studio);
            sb.Append("\t");
            sb.Append(Interessi);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Elemento della lista contenente le informazioni sull'errore
    /// </summary>
    public class ErrorRecord : BaseModel
    {
        private string _recordNumber;
        public string RecordNumber
        {
            get { return _recordNumber; }
            set
            {
                if (value != _recordNumber)
                {
                    _recordNumber = value;
                    OnPropertyChanged("RecordNumber");
                }
            }
        }

        private string _tipo;
        public string TipoErrori
        {
            get { return _tipo; }
            set
            {
                if (value != _tipo)
                {
                    _tipo = value;
                    OnPropertyChanged("TipoErrori");
                }
            }
        }

        public ErrorRecord() {
            RecordNumber = string.Empty;
            TipoErrori = string.Empty;
        }

        public ErrorRecord(string record, string err)
        {
            RecordNumber = record;
            TipoErrori = err;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}", this.RecordNumber, this.TipoErrori);
        }
    }
}
