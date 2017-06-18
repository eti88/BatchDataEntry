using BatchDataEntry.Abstracts;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelTools : ViewModelBase
    {
        private static string CACHEFILE = @"cache.db3";
        private AbsDbHelper db;
        private bool needReload = false;

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
        
        public ViewModelTools() {
            DateFormat = "yyyy-MM-dd"; // Set default date format
            GenerateOutputFile = false;
            CheckEmpty = true;
            ErrorRecordList = new ObservableCollection<ErrorRecord>();

            var settings = Properties.Settings.Default;
            if(!string.IsNullOrEmpty(settings.LastAssociato))
                CodiceAssociato = settings.LastAssociato;

            if (!string.IsNullOrEmpty(settings.LastNegozio))
                CodiceNegozio = settings.LastNegozio;

        }

        public void NeedReloadFile()
        {
            needReload = true;
        }

        // Carica il file sqlite nel modello FidelityCard
        public void LoadFile() {
            FileAttributes attr = File.GetAttributes(InputFilePath);
            if (attr.HasFlag(FileAttributes.Directory))
                InputFilePath = Path.Combine(InputFilePath, CACHEFILE);
            

            if(!string.IsNullOrEmpty(InputFilePath) && File.Exists(InputFilePath))
            {
                db = new DatabaseHelper(InputFilePath);
                Records = ((DatabaseHelper)db).GetRecords();
                needReload = false;
                Log("# Caricati " + Records.Count + " records");
            }
        }

        public void CheckFile() {
            if (ErrorRecordList.Count > 0) ErrorRecordList.Clear();
            if (db == null || needReload) LoadFile();

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
            }
        }

        private void Check(string path)
        {         
            if (File.Exists(path)) File.Delete(path);
            using(var file = new StreamWriter(path, true))
            {
                file.WriteLine("File\tCampo\tValore\tTag\tMessaggio");
                foreach(FidelityClient r in Records)
                {
                    foreach(ErrorRecord err in r.Checks(CheckEmpty, DateFormat))
                    {
                        file.WriteLine(err.ToString());
                    }
                }
            }
        }

        private void Check()
        {
            string tagEmpty = "Vuoto";
            string tagTel = "NumeroNonValido";
            string tagFormato = "ErroreFormato";

            if (Records.Count > 0)
            {
                foreach (FidelityClient r in Records)
                {
                    //ErrorRecord
                    if (!Utility.IsNotVoid(r.Card)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "FidelityCard", r.Card, tagEmpty));
                    if (!Utility.IsNotVoid(r.Cognome)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Cognome", r.Cognome, tagEmpty));
                    if (!Utility.IsNotVoid(r.Indirizzo)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Indirizzo", r.Indirizzo, tagEmpty));
                    if (!Utility.IsNotVoid(r.Civico)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Civico", r.Civico, tagEmpty));
                    if (!Utility.IsNotVoid(r.Localita)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Localita", r.Localita, tagEmpty));
                    if (!Utility.IsNotVoid(r.Provincia)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Provincia", r.Provincia, tagEmpty));
                    if (!Utility.IsNotVoid(r.Cap)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Cap", r.Cap, tagEmpty));
                    if (!Utility.IsNotVoid(r.Prefisso)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Prefisso", r.Prefisso, tagEmpty));
                    if (!Utility.IsNotVoid(r.Telefono)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Telefono", r.Telefono, tagEmpty));

                    if (!Utility.IsNotVoid(r.Cellulare))
                    {
                        ErrorRecordList.Add(new ErrorRecord(r.FileName, "Cellulare", r.Cellulare, tagEmpty));
                    }
                    else
                    {
                        if (!Utility.IsValidTelephone(r.Cellulare)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Cellulare", r.Cellulare, tagTel));
                    }

                    if (!Utility.IsNotVoid(r.Email))
                    {
                        ErrorRecordList.Add(new ErrorRecord(r.FileName, "Email", r.Email, tagEmpty));
                    }
                    else
                    {
                        if (!Utility.IsValidEmail(r.Email)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Email", r.Email, tagFormato));
                    }

                    if (!Utility.IsNotVoid(r.DataNascita))
                    {
                        ErrorRecordList.Add(new ErrorRecord(r.FileName, "DataNascita", r.Card, tagEmpty));
                    }
                    else
                    {
                        if (!Utility.IsValidDate(r.DataNascita, DateFormat)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "DataNascita", r.DataNascita, tagFormato));
                    }

                    if (!Utility.IsNotVoid(r.Luogo)) ErrorRecordList.Add(new ErrorRecord(r.FileName, "Luogo", r.Luogo, tagEmpty));
                }
            }
        }

        public void GeneraFiles() {
            if (ErrorRecordList.Count > 0) ErrorRecordList.Clear();
            if (db == null || needReload) LoadFile();

            if (string.IsNullOrEmpty(CodiceAssociato) || string.IsNullOrEmpty(CodiceNegozio))
            {
                MessageBox.Show("Codice Associato o Codice Negozio vuoti");
                return;
            }

            // Salvo i valori correnti
            Properties.Settings.Default.LastAssociato = CodiceAssociato;
            Properties.Settings.Default.LastNegozio = CodiceNegozio;

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
        private const string TAGEmpty = "VUOTO";
        private const string TAGTel = "NUMERO";
        private const string TAGEmail = "EMAIL";
        private const string TAGData = "DATA";

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
        public List<ErrorRecord> Checks(bool checkVoid, string dateformat)
        {
            var errors = new List<ErrorRecord>();
            if(checkVoid)
            {
                if (!Utility.IsNotVoid(this.Card)) errors.Add(new ErrorRecord(this.FileName, "FidelityCard", this.Card, TAGEmpty));
                if (!Utility.IsNotVoid(this.Cognome)) errors.Add(new ErrorRecord(this.FileName, "Cognome", this.Cognome, TAGEmpty));
                if (!Utility.IsNotVoid(this.Indirizzo)) errors.Add(new ErrorRecord(this.FileName, "Indirizzo", this.Indirizzo, TAGEmpty));
                if (!Utility.IsNotVoid(this.Civico)) errors.Add(new ErrorRecord(this.FileName, "Civico", this.Civico, TAGEmpty));
                if (!Utility.IsNotVoid(this.Localita)) errors.Add(new ErrorRecord(this.FileName, "Localita", this.Localita, TAGEmpty));
                if (!Utility.IsNotVoid(this.Provincia)) errors.Add(new ErrorRecord(this.FileName, "Provincia", this.Provincia, TAGEmpty));
                if (!Utility.IsNotVoid(this.Cap)) errors.Add(new ErrorRecord(this.FileName, "Cap", this.Cap, TAGEmpty));
            }

            if (Utility.IsNotVoid(this.Cellulare)) {
                if (!Utility.IsValidTelephone(this.Cellulare)) errors.Add(new ErrorRecord(this.FileName, "Cellulare", this.Cellulare, TAGTel));
            }

            if (Utility.IsNotVoid(this.Email))
            {
                if (!Utility.IsValidEmail(this.Email)) errors.Add(new ErrorRecord(this.FileName, "Email", this.Email, TAGEmail));
            }

            if (Utility.IsNotVoid(this.DataNascita)) {
                if(!Utility.IsValidDate(this.DataNascita, dateformat)) errors.Add(new ErrorRecord(this.FileName, "DataDiNascita", this.DataNascita, TAGData));
                if (Utility.IsValidDateRange(this.DataNascita, dateformat)) errors.Add(new ErrorRecord(this.FileName, "DataDiNascita", this.DataNascita, "ETA"));
            }

            return errors;
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
            sb.Append(DataNascita);
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

        private string _nome;
        public string NomeCampo
        {
            get { return _nome; }
            set
            {
                if (value != _nome)
                {
                    _nome = value;
                    OnPropertyChanged("NomeCampo");
                }
            }
        }

        private string _val;
        public string ValoreCorrente
        {
            get { return _val; }
            set
            {
                if (value != _val)
                {
                    _val = value;
                    OnPropertyChanged("ValoreCorrente");
                }
            }
        }

        private string _tipo;
        public string TipoErrore
        {
            get { return _tipo; }
            set
            {
                if (value != _tipo)
                {
                    _tipo = value;
                    OnPropertyChanged("TipoErrore");
                }
            }
        }

        public ErrorRecord() {
            RecordNumber = string.Empty;
            NomeCampo = string.Empty;
            ValoreCorrente = string.Empty;
            TipoErrore = string.Empty;
        }

        public ErrorRecord(string record, string nome, string valore, string tipo)
        {
            RecordNumber = record;
            NomeCampo = nome;
            ValoreCorrente = valore;
            TipoErrore = tipo;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t\t{3}", this.RecordNumber, this.NomeCampo, this.ValoreCorrente, this.TipoErrore);
        }
    }
}
