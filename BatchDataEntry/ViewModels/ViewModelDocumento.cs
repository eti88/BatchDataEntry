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

namespace BatchDataEntry.ViewModels
{
    public class ViewModelDocumento : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DatabaseHelper _db;
        private DatabaseHelperSqlServer dbsql = null;
        private static Batch _batch;
        private NavigationList<Dictionary<int, string>> _dc;
        private  Document _doc;
        private int _selectElementFocus;
        private string[] repeatValues;
        private MoonPdfPanel _PdfWrapper;

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
        }

        public ViewModelDocumento(Batch _currentBatch, int indexRowVal, DatabaseHelperSqlServer dbc)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0)
            {
                if(dbc == null)
                    Batch.LoadModel();
                else
                    Batch.LoadModel(dbc);
            }
                
            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0)
            {
                if(dbc == null)
                    Batch.Applicazione.LoadCampi();
                else
                    Batch.Applicazione.LoadCampi(dbc);
            }
                
            PdfWrapper = new MoonPdfPanel();
            LoadDocsList();
            if (DocFiles.CurrentIndex > DocFiles.Count)
            {
                logger.Error(string.Format("DocFiles index: {0}, file totali nel dictionary {1} troppo alto!", DocFiles.CurrentIndex, DocFiles.Count));
                return;
            }
            DocFiles.CurrentIndex = indexRowVal;
            DocFile = new Document(Batch, DocFiles.Current);

            PdfWrapper.Background = System.Windows.Media.Brushes.LightGray;
            PdfWrapper.OpenFile(DocFile.Path);
            PdfWrapper.ViewType = ViewType.SinglePage;
            PdfWrapper.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;

            _selectElementFocus = Batch.Applicazione.StartFocusColumn;
            repeatValues = Batch.Applicazione.Campi.Count > 0 ? new string[Batch.Applicazione.Campi.Count] : new string[1];
            Properties.Settings.Default.CurrentBatch = Batch.Id;
            Properties.Settings.Default.Save();
        }

        public ViewModelDocumento(Batch _currentBatch, DatabaseHelperSqlServer dbc)
        {
            if (_currentBatch != null)
                Batch = _currentBatch;
            _db = new DatabaseHelper(ConfigurationManager.AppSettings["cache_db_name"], Batch.DirectoryOutput);
            if (Batch.Applicazione == null || Batch.Applicazione.Id == 0)
            {
                if (dbc == null)
                    Batch.LoadModel();
                else
                    Batch.LoadModel(dbc);
            }

            if (Batch.Applicazione.Campi == null || Batch.Applicazione.Campi.Count == 0)
            {
                if (dbc == null)
                    Batch.Applicazione.LoadCampi();
                else
                    Batch.Applicazione.LoadCampi(dbc);
            }
            PdfWrapper = new MoonPdfPanel();
            LoadDocsList();
            
            DocFiles.CurrentIndex = GetId();
            if(DocFiles.CurrentIndex > DocFiles.Count)
            {
                logger.Error(string.Format("DocFiles index: {0}, file totali nel dictionary {1} troppo alto!", DocFiles.CurrentIndex, DocFiles.Count));
                return;
            }
            DocFile = new Document(Batch, DocFiles.Current);

            PdfWrapper.OpenFile(DocFile.Path);
            PdfWrapper.ViewType = ViewType.SinglePage;
            PdfWrapper.Background = System.Windows.Media.Brushes.LightGray;
            PdfWrapper.PageRowDisplay = PageRowDisplayType.ContinuousPageRows;

            _selectElementFocus = Batch.Applicazione.StartFocusColumn;
            repeatValues = Batch.Applicazione.Campi.Count > 0 ? new string[Batch.Applicazione.Campi.Count] : new string[1];
            Properties.Settings.Default.CurrentBatch = Batch.Id;
            Properties.Settings.Default.Save();           
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

        public void Indexes()
        {
            DocFile.IsIndexed = true;
            _db.UpdateRecordDocumento(DocFile);

            // Salva il valore se bisogna riproporlo
            for (int z = 0; z < Batch.Applicazione.Campi.Count; z++)
            {
                if (Batch.Applicazione.Campi[z].Riproponi)
                    repeatValues[z] = string.Format(DocFile.Voci.ElementAt(z).Value);
            }
            Batch.UltimoIndicizzato = DocFiles.CurrentIndex + 1;

            if(dbsql == null)
            {
                DatabaseHelper maindb = new DatabaseHelper();
                maindb.UpdateRecordBatch(Batch);
            }
            else
                dbsql.Update(Batch);

            // controllare se bisogna salvare il valore inserito per l'autocomletamento
            foreach (var col in DocFile.Voci)
            {
                try
                {
                    if (col.IsAutocomplete == true && col.AUTOCOMPLETETYPE.Equals("DB"))
                    {
                        if (!string.IsNullOrEmpty(col.Value) && col.Id > 0)
                        {
                            var auto = new Autocompletamento();
                            auto.Colonna = col.Id;
                            auto.Valore = col.Value;
                            _db.InsertRecordAutocompletamento(auto);
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
                    Utility.CopiaPdf(DocFile.Path, Batch.DirectoryOutput, string.Format("{0}{1}", Batch.PatternNome, DocFile.FileName + ".pdf"));
                else
                    Utility.CopiaPdf(DocFile.Path, Batch.DirectoryOutput, DocFile.FileName + ".pdf");
            }

            MoveNextItem();
        }

        public void MovePreviousItem()
        {
            if (DocFiles.hasPrevious)
            {
                DocFile = new Document(Batch, DocFiles.MovePrevious);
                if (DocFile.Voci == null || DocFile.Voci.Count == 0)
                    DocFile.AddInputsToPanel(Batch, _db);
                for (int i = 0; i < repeatValues.Length; i++)
                {
                    if (!string.IsNullOrEmpty(repeatValues[i]))
                        DocFile.Voci.ElementAt(i).Value = repeatValues[i];
                }
                PdfWrapper.OpenFile(DocFile.Path);
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
                for (int i = 0; i < repeatValues.Length; i++)
                {
                    if (!string.IsNullOrEmpty(repeatValues[i]))
                        DocFile.Voci.ElementAt(i).Value = repeatValues[i];
                }
                PdfWrapper.OpenFile(DocFile.Path);
            }
            
            RaisePropertyChanged("DocFile");
        }
        
        public void Interrompi()
        {
            Batch.DocCorrente = DocFiles.CurrentIndex + 1;

            #if DEBUG
            Console.WriteLine("Documento corrente: " + Batch.DocCorrente);
            #endif
            if(dbsql == null)
            {
                DatabaseHelper maindb = new DatabaseHelper();
                maindb.UpdateRecordBatch(Batch);
            }
            else
                dbsql.Update(Batch);
            
            CloseWindow(true);
        }

        public void EnterActionFunction(object parameter)
        {
            var sugg = parameter as Suggestion;

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
                if (i < countVoci)
                {
                    DocFile.Voci[i].Value = record[i];
                }
            }
            RaisePropertyChanged("ViewModelDocumento");
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

        #endregion
    }
}