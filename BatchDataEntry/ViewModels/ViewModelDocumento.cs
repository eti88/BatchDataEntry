using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Components;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using MoonPdfLib;
using NLog;
using BatchDataEntry.Suggestions;
using BatchDataEntry.Abstracts;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelDocumento : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DatabaseHelper _db; // <-- db cache
        private AbsDbHelper db = null;
        private static Batch _batch;
        private NavigationList<Dictionary<int, string>> _dc;
        private  Document _doc;
        private int _selectElementFocus;
        private string[] repeatValues;
        private MoonPdfPanel _PdfWrapper;
        private Image _img;
        private NavigationList<Image> _imgs;
        private List<Concatenation> _concatenations;

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
        public MoonPdfPanel PdfWrapper
        {
            get { return _PdfWrapper; }
            set
            {
                if (_PdfWrapper != value)
                    _PdfWrapper = value;
                RaisePropertyChanged("PdfWrapper");
            }
        }
        public NavigationList<Image> Images
        {
            get { return _imgs; }
            set
            {
                if (_imgs != value)
                    _imgs = value;
                RaisePropertyChanged("Images");
            }
        }
        public Image Image
        {
            get { return _img; }
            set
            {
                if (_img != value)
                    _img = value;
                RaisePropertyChanged("Image");
            }
        }     

        private bool CanMoveNext
        {
            get { return DocFiles != null && DocFiles.Count > 0 && DocFiles.hasNext; }
        }
        private bool CanFocused {  get { return _selectElementFocus >= 0; } }
        private bool CanMovePrevious
        {
            get { return DocFiles != null && DocFiles.Count > 0 && DocFiles.hasPrevious; }
        }
        

        public ViewModelDocumento()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            Batch = new Batch();
            repeatValues = new string[10];
            _concatenations = new List<Concatenation>();
        }

        public ViewModelDocumento(Batch _currentBatch, int indexRowVal, AbsDbHelper dbc)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            db = dbc;
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0) Batch.LoadModel(db);               
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0) Batch.Applicazione.LoadCampi(db);
            LoadConcatenations(db, Batch.Applicazione.Id);

            LoadDocsList();
            if (DocFiles.CurrentIndex > DocFiles.Count)
            {
                logger.Error(string.Format("DocFiles index: {0} > {1} , Index out of range", DocFiles.CurrentIndex, DocFiles.Count));
                return;
            }
            DocFiles.CurrentIndex = indexRowVal;
            DocFile = new Document(db ,Batch, DocFiles.Current);
            DocFile.AddInputsToPanel(Batch, db, _db, DocFiles.Current);
            // Inverte il valore per l'abilitazione del campo
            foreach (Campo c in DocFile.Voci)
            {
                c.IsDisabilitato = !c.IsDisabilitato;
            }

            if (Batch.TipoFile == TipoFileProcessato.Pdf)
                SetPdfWrapper(DocFile.Path);
            else if (Batch.TipoFile == TipoFileProcessato.Tiff)
            {
                SetTiffWrapper(DocFile.Path);
            }
                
            RaisePropertyChanged("DocFile");
            _selectElementFocus = Batch.Applicazione.StartFocusColumn;
            repeatValues = Batch.Applicazione.Campi.Count > 0 ? new string[Batch.Applicazione.Campi.Count] : new string[1];
            Properties.Settings.Default.CurrentBatch = Batch.Id;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Costruttore utilizzato per la correzione degli errori
        /// </summary>
        /// <param name="_currentBatch"></param>
        /// <param name="dbc"></param>
        /// <param name="SubDocList"></param>
        public ViewModelDocumento(Batch _currentBatch, AbsDbHelper dbc, NavigationList<Dictionary<int, string>> SubDocList)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            db = dbc;
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0) Batch.LoadModel(db);
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0) Batch.Applicazione.LoadCampi(db);

            if (SubDocList == null || SubDocList.Count == 0) return;
            DocFiles = SubDocList;
            
            DocFile = new Document(db, Batch, DocFiles.Current);
            DocFile.AddInputsToPanel(Batch, db, _db, DocFiles.Current);
            // Inverte il valore per l'abilitazione del campo
            foreach (Campo c in DocFile.Voci)
            {
                c.IsDisabilitato = !c.IsDisabilitato;
            }

            if (Batch.TipoFile == TipoFileProcessato.Pdf)
                SetPdfWrapper(DocFile.Path);
            else if (Batch.TipoFile == TipoFileProcessato.Tiff)
            {
                SetTiffWrapper(DocFile.Path);
            }

            RaisePropertyChanged("DocFile");
            _selectElementFocus = Batch.Applicazione.StartFocusColumn;
            repeatValues = new string[1];
        }

        public ViewModelDocumento(Batch _currentBatch, AbsDbHelper dbc)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            db = dbc;
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0) Batch.LoadModel(db);
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0) Batch.Applicazione.LoadCampi(db);
            LoadConcatenations(db, Batch.Applicazione.Id);

            LoadDocsList();          
            DocFiles.CurrentIndex = GetId();
            if(DocFiles.CurrentIndex > DocFiles.Count)
            {
                logger.Error(string.Format("DocFiles index: {0}, file totali nel dictionary {1} troppo alto!", DocFiles.CurrentIndex, DocFiles.Count));
                return;
            }
            DocFile = new Document(db, Batch, DocFiles.Current);
            DocFile.AddInputsToPanel(Batch, db, _db, DocFiles.Current);
            // Inverte il valore per l'abilitazione del campo
            foreach (Campo c in DocFile.Voci)
            {
                c.IsDisabilitato = !c.IsDisabilitato;            
            }

            if (Batch.TipoFile == TipoFileProcessato.Pdf)
                SetPdfWrapper(DocFile.Path);
            else if (Batch.TipoFile == TipoFileProcessato.Tiff)
            {
                SetTiffWrapper(DocFile.Path);
            }

            RaisePropertyChanged("DocFile");
            _selectElementFocus = Batch.Applicazione.StartFocusColumn;
            repeatValues = Batch.Applicazione.Campi.Count > 0 ? new string[Batch.Applicazione.Campi.Count] : new string[1];
            Properties.Settings.Default.CurrentBatch = Batch.Id;
            Properties.Settings.Default.Save();
        }

        private void SetTiffWrapper(string file)
        {
            this.Images = new NavigationList<Image>();
            var tmp = GetAllPages(file);
            foreach (Image i in tmp)
                Images.Add(i);
            Image = Images.ElementAt(0); // Imposto la visualizzazione dell aprima pagina
            RaisePropertyChanged("Image");
        }

        private void SetPdfWrapper(string file)
        {
            PdfWrapper = new MoonPdfPanel();
            PdfWrapper.Background = System.Windows.Media.Brushes.LightGray;
            PdfWrapper.OpenFile(file);
            PdfWrapper.ViewType = ViewType.SinglePage;
            PdfWrapper.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;
        }

        private List<Image> GetAllPages(string file)
        {

            List<Image> images = new List<Image>();
            Bitmap bitmap = (Bitmap)Image.FromFile(file);
            int count = bitmap.GetFrameCount(FrameDimension.Page);
            for (int idx = 0; idx < count; idx++)
            {
                // save each frame to a bytestream
                bitmap.SelectActiveFrame(FrameDimension.Page, idx);
                MemoryStream byteStream = new MemoryStream();
                bitmap.Save(byteStream, ImageFormat.Tiff);

                // and then create a new Image from it
                images.Add(Image.FromStream(byteStream));
            }
            bitmap.Dispose();
            return images;
        }

        private void LoadDocsList()
        {
            try {
                var dbCache = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
                DocFiles = dbCache.GetDocuments();
            } catch (Exception ex) {
                logger.Error(ex.ToString());
            }
            finally
            {
                if (DocFiles == null || DocFiles.Count == 0)
                {
                    logger.Error("La lista dei documenti risulta vuota o è fallita per qualche motivo(Errore Precedente): LoadDocsList()");
                }
            }
            RaisePropertyChanged("DocFiles");
        }

        private int GetId()
        {
            return Batch.UltimoIndicizzato;
        }

        private void LoadConcatenations(AbsDbHelper _db, int idmodello)
        {
            
            if (_db is DatabaseHelperSqlServer)
            {
                _concatenations = ((DatabaseHelperSqlServer)_db).LoadConcatenations(idmodello).ToList();
                if(_concatenations == null)
                    _concatenations = new List<Concatenation>();
            }
            else
            {
                _concatenations = new List<Concatenation>();
            }

        }

        /// <summary>
        /// Se il record corrisponde a una concatenazione viene popolato
        /// la posizione minore definisce il punto di start, analogo per l'end.
        /// se il campo concatenato non è quello di inizio o presente nella concatenazione bisogna impedire la popolazione dei campi
        /// l'evento di concatenazione viene gestito dall'evento invio del autocompletetextbox
        /// </summary>
        /// <param name="r"></param>
        private void HandleConcatenations(int pos, List<Concatenation> concats, AbsSuggestion suggestion) 
        {
            foreach (Concatenation concat in concats)
            {
                // Se il campo è contenuto in una concatenazione 
                if (IsRecordInConcatenation(pos, concat))
                {
                    // Controllare che corrisponda al record di start
                    if(concat.START_POS == pos)
                    {
                        List<string> trow = new List<string>();
                        // Recupera il record corrispondente dalla tabella
                        if (suggestion is SuggestionLocalita)
                            trow = GetTableRow("Localita", 1, suggestion.Valore);
                        else
                            trow = GetTableRow(Batch.Applicazione.Campi[pos].TabellaSorgente, Batch.Applicazione.Campi[pos].SourceTableColumn, suggestion.Valore);

                        if (trow.Count == 0) return;
                        string reftab = Batch.Applicazione.Campi[pos].TabellaSorgente;

                        foreach (Record r in DocFile.Voci)
                        {
                            if(IsRecordInConcatenation(r.Posizione, concat))
                                r.Valore = trow.ElementAt(r.SourceTableColumn).ToString();
                        }
                    }             
                    return;
                }
            }
        }

        private bool IsRecordInConcatenation(int pos, Concatenation concatenation)
        {
            foreach (KeyValuePair<string, object> c in concatenation.CampiSelezionati)
            {
                Campo campo;
                try
                {
                    campo = JsonConvert.DeserializeObject<Campo>(c.Value.ToString());

                    if (campo != null)
                    {
                        if (campo.Posizione == pos)
                            return true;
                        else
                            continue;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return false;
        }

        public void Indexes()
        {
            DocFile.IsIndexed = true;
            foreach (Record rec in DocFile.Voci)
            {
                if (rec.TipoCampo == EnumTypeOfCampo.Data && rec.Valore.Equals("__-__-____"))
                {
                    rec.Valore = "";
                }
            }

            _db.UpdateRecordDocumento(DocFile);

            // Salva il valore se bisogna riproporlo
            if(!Batch.IsTemp)
            {
                for (int z = 0; z < Batch.Applicazione.Campi.Count; z++)
                {
                    if (Batch.Applicazione.Campi[z].Riproponi)
                        repeatValues[z] = string.Format(DocFile.Voci.ElementAt(z).Valore);
                }
                Batch.UltimoIndicizzato = DocFiles.CurrentIndex + 1;
                db.Update(Batch);

                // controllare se bisogna salvare il valore inserito per l'autocomletamento in base al tipo di database usato
                foreach (var col in DocFile.Voci)
                {
                    try
                    {
                        if (col.TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSqlite)
                        {
                            if (!string.IsNullOrEmpty(col.Valore) && col.Id >= 0)
                            {
                                // Conreolla se è già presente l'autocompletamento nella tabella e in tal caso skip
                                string query = string.Format("SELECT Count(Id) FROM Autocompletamento WHERE Colonna = {0} AND Valore = '{0}'", col.Id, col.Valore);
                                if (_db.Count(query) > 0) continue;

                                var auto = new SuggestionSingleColumn();
                                auto.Colonna = col.Id;
                                auto.Valore = col.Valore;
                                _db.Insert(auto);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Errore aggiunta voce all'autocompletamento: " + ex.ToString());
                    }
                }

                if (!File.Exists(Path.Combine(Batch.DirectoryOutput, DocFile.FileName)))
                {
                    if (!string.IsNullOrEmpty(Batch.PatternNome))
                    {
                        if (Batch.TipoFile == TipoFileProcessato.Pdf)
                            Utility.CopiaFile(DocFile.Path, Batch.DirectoryOutput, string.Format("{0}{1}", Batch.PatternNome, DocFile.FileName + ".pdf"));
                        else
                            Utility.CopiaFile(DocFile.Path, Batch.DirectoryOutput, string.Format("{0}{1}", Batch.PatternNome, DocFile.FileName + ".tif"));
                    }
                    else
                    {
                        if (Batch.TipoFile == TipoFileProcessato.Pdf)
                            Utility.CopiaFile(DocFile.Path, Batch.DirectoryOutput, DocFile.FileName + ".pdf");
                        else
                            Utility.CopiaFile(DocFile.Path, Batch.DirectoryOutput, DocFile.FileName + ".tif");
                    }
                }
            }
            
            MoveNextItem();
        }

        public void MovePreviousItem()
        {
            if (DocFiles.hasPrevious)
            {
                DocFile = new Document(db, Batch, DocFiles.MovePrevious);

                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, db, _db, DocFiles.Current);
                for (int i = 0; i < repeatValues.Length; i++)
                {
                    if (!string.IsNullOrEmpty(repeatValues[i]))
                        DocFile.Voci.ElementAt(i).Valore = repeatValues[i];
                }

                // Inverte il valore per l'abilitazione del campo
                foreach (Campo c in DocFile.Voci)
                {
                    c.IsDisabilitato = !c.IsDisabilitato;
                }
                if(Batch.TipoFile == TipoFileProcessato.Pdf && PdfWrapper != null)
                    PdfWrapper.OpenFile(DocFile.Path);
                else if(Batch.TipoFile == TipoFileProcessato.Tiff)
                    SetTiffWrapper(DocFile.Path);
                RaisePropertyChanged("DocFile");
                GC.Collect();
            }
        }

        public void MoveNextItem()
        {
            
            if (DocFiles.hasNext)
            {
                DocFile = new Document(db, Batch, DocFiles.MoveNext);
                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, db, _db, DocFiles.Current);
                for (int i = 0; i < repeatValues.Length; i++)
                {
                    if (!string.IsNullOrEmpty(repeatValues[i]))
                        DocFile.Voci.ElementAt(i).Valore = repeatValues[i];
                }

                // Inverte il valore per l'abilitazione del campo
                foreach (Campo c in DocFile.Voci)
                {
                    c.IsDisabilitato = !c.IsDisabilitato;
                }

                if (Batch.TipoFile == TipoFileProcessato.Pdf && PdfWrapper != null)
                    PdfWrapper.OpenFile(DocFile.Path);
                else if (Batch.TipoFile == TipoFileProcessato.Tiff)
                    SetTiffWrapper(DocFile.Path);

                RaisePropertyChanged("DocFile");
                GC.Collect();
            }
        }
        
        public void Interrompi()
        {
            if(!Batch.IsTemp)
            {
                Batch.DocCorrente = DocFiles.CurrentIndex + 1;
                #if DEBUG
                Console.WriteLine("Documento corrente: " + Batch.DocCorrente);
                #endif
                db.Update(Batch);
            }
            
            CloseWindow(true);
        }

        public void EnterActionFunction(object parameter)
        {
            var sugg = parameter as SuggestionDoubleColumn;

            if (sugg == null) return;

            #if DEBUG
            Console.WriteLine("Passato parametro per autocompletamento:\t" + sugg.ColumnA + " : " + sugg.ColumnB);
            #endif

            if (string.IsNullOrEmpty(sugg.ColumnA)) return;
            int col1 = Batch.Applicazione.Campi.Where(x => x.IndicePrimario == true).Select(x => x.Posizione).FirstOrDefault();
            int col2 = Batch.Applicazione.Campi.Where(x => x.IndiceSecondario == true).Select(x => x.Posizione).FirstOrDefault();

            List<string> record = Csv.SearchRow(_batch.Applicazione.PathFileCsv, sugg.ColumnA, sugg.ColumnB, col1, col2, Convert.ToChar(_batch.Applicazione.Separatore));

            if (record.Count == 0) return;

            int countVoci = DocFile.Voci.Count;
            for (int i = 0; i < record.Count; i++)
            {
                if (i < countVoci) DocFile.Voci[i].Valore = record[i];
            }
            RaisePropertyChanged("ViewModelDocumento");
        }

        public void EnterActionFunctionSql(object parameter)
        {
            if (parameter == null) return;
            var lstVal = parameter as List<object>;

            int pos = Convert.ToInt32(lstVal[0]);
            AbsSuggestion sugg;
            if (lstVal[1] is SuggestionLocalita)
                sugg = lstVal[1] as SuggestionLocalita;
            else if (lstVal[1] is SuggestionDoubleColumn)
                sugg = lstVal[1] as SuggestionDoubleColumn;
            else
                sugg = lstVal[1] as SuggestionSingleColumn;

            if (sugg == null) return;

            #if DEBUG
            Console.WriteLine(string.Format("Record {0} SelectedValue {1}", pos,sugg.Valore));
            #endif

            if (string.IsNullOrEmpty(sugg.Valore)) return;
            HandleConcatenations(pos, this._concatenations, sugg);
        }
        
        /// <summary>
        /// Ricerca nella tabella del database mssql il record corrispondente e restituisce la lista
        /// sotto forma di lista.
        /// </summary>
        /// <param name="table">Nome della tabella</param>
        /// <param name="column">Colonna nel quale cercare il valore</param>
        /// <param name="text">Testo da cercare</param>
        /// <returns>Lista ordinata (per colonna) dei valori contenuti nella row</returns>
        public List<string> GetTableRow(string table, int column, string text)
        {
            var row = new List<string>();
            if(db != null && db is DatabaseHelperSqlServer)
            {
                var columns = ((DatabaseHelperSqlServer)db).GetColumns(table);
                row = ((DatabaseHelperSqlServer)db).GetRecord(table, columns, column, text);
            }
            return row;
        }

        private void ZoomInPdf()
        {
            PdfWrapper?.ZoomIn();
        }

        private void ZoomOutPdf()
        {
            PdfWrapper?.ZoomOut();
        }

        private void ScrollPageDownPdf()
        {
            PdfWrapper?.GotoNextPage();
        }

        private void ScrollPageUpPdf()
        {
            PdfWrapper?.GotoPreviousPage();
        }

        private void ImageNextPage()
        {
            if (Images.Count == 1) return;
            Image tmp = null;

            try
            {
                if (Images.hasNext)
                    tmp = Images.MoveNext;

                if (tmp != null)
                    Image = tmp;
            }
            catch (Exception)
            {
                Image = Images.Current;
            }
        }

        private void ImagePrevPage()
        {
            if (Images.Count == 1) return;
            Image tmp = null;
            try
            {
                if (Images.hasPrevious)
                {
                    tmp = Images.MovePrevious;
                }

                if (tmp != null)
                    Image = tmp;
            }
            catch (Exception)
            {
                Image = Images.Current; 
            }
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

        private RelayCommand _textboxCmd;
        public ICommand TextboxCmd
        {
            get
            {
                if (_textboxCmd == null)
                {
                    _textboxCmd = new RelayCommand((param) => EnterActionFunction(param));
                }
                return _textboxCmd;
            }
        }

        private RelayCommand _textboxSqlCmd;
        public ICommand TextboxSqlCmd
        {
            get
            {
                if (_textboxSqlCmd == null)
                {
                    _textboxSqlCmd = new RelayCommand((param) => EnterActionFunctionSql(param));
                }
                return _textboxSqlCmd;
            }
        }

        // Pdf commands

        private RelayCommand _ZoomInCmd;
        public ICommand ZoomInCmd
        {
            get
            {
                if (_ZoomInCmd == null)
                {
                    _ZoomInCmd = new RelayCommand((param) => ZoomInPdf());
                }
                return _ZoomInCmd;
            }
        }

        private RelayCommand _ZoomOutCmd;
        public ICommand ZoomOutCmd
        {
            get
            {
                if (_ZoomOutCmd == null)
                {
                    _ZoomOutCmd = new RelayCommand((param) => ZoomOutPdf());
                }
                return _ZoomOutCmd;
            }
        }

        private RelayCommand _PageDownCmd;
        public ICommand PageDownCmd
        {
            get
            {
                if (_PageDownCmd == null)
                {
                    _PageDownCmd = new RelayCommand((param) => ScrollPageDownPdf());
                }
                return _PageDownCmd;
            }
        }

        private RelayCommand _PageUpCmd;
        public ICommand PageUpCmd
        {
            get
            {
                if (_PageUpCmd == null)
                {
                    _PageUpCmd = new RelayCommand((param) => ScrollPageUpPdf());
                }
                return _PageUpCmd;
            }
        }

        private RelayCommand _imgPageNext;
        public ICommand ImgPageNext
        {
            get
            {
                if (_imgPageNext == null)
                {
                    _imgPageNext = new RelayCommand(param => ImageNextPage());
                }
                return _imgPageNext;
            }
        }

        private RelayCommand _imgPagePrev;
        public ICommand ImgPagePrev
        {
            get
            {
                if (_imgPagePrev == null)
                {
                    _imgPagePrev = new RelayCommand(param => ImagePrevPage());
                }
                return _imgPagePrev;
            }
        }

        #endregion
    }  
}