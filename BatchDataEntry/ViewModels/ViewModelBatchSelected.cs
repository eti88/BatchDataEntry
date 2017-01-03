using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;

namespace BatchDataEntry.ViewModels
{
    class ViewModelBatchSelected : ViewModelMain
    {
        public ViewModelBatchSelected() { }

        public ViewModelBatchSelected(Batch batch)
        {
            _currentBatch = batch;
            long bytes = GetDirectorySize(batch.DirectoryInput);
            Dimensioni = String.Format("{0} MB",ConvertSize((double)bytes, "MB").ToString("0.00"));
            NumeroDocumenti = CountFiles(batch.DirectoryInput, batch.TipoFile);      
        }

        private Batch _currentBatch { get; set; }

        private DataView _dtSource;

        public DataView DataSource
        {
            get { return _dtSource; }
            set
            {
                _dtSource = value;
                RaisePropertyChanged("DataSource");
            }
        }
        private int _ndocs;
        public int NumeroDocumenti
        {
            get { return _ndocs; }
            set
            {
                _ndocs = value;
                RaisePropertyChanged("NumeroDocumenti");
            }
        }

        private string _dimfiles;
        public string Dimensioni
        {
            get { return _dimfiles; }
            set
            {
                _dimfiles = value;
                RaisePropertyChanged("Dimensioni");
            }
        }

        private int _curDoc;
        public int DocumentoCorrente
        {
            get { return _curDoc; }
            set
            {
                _curDoc = value;
                RaisePropertyChanged("DocumentoCorrente");
            }
        }

        private int _ulti;
        public int UltimoIndicizzato
        {
            get { return _ulti; }
            set
            {
                _ulti = value;
                RaisePropertyChanged("UltimoIndicizzato");
            }
        }

        private RelayCommand _continuaCmd;
        public ICommand ContinuaCmd
        {
            get
            {
                if (_continuaCmd == null)
                {
                    _continuaCmd = new RelayCommand(param => this.ContinuaInserimento());
                }
                return _continuaCmd;
            }
        }

        private RelayCommand _checkCmd;
        public ICommand CheckCmd
        {
            get
            {
                if (_checkCmd == null)
                {
                    _checkCmd = new RelayCommand(param => this.CheckBatch());
                }
                return _checkCmd;
            }
        }

        private void ContinuaInserimento()
        {
            if (_currentBatch != null)
            {
                Views.Documento inserimento = new Views.Documento();
                inserimento.DataContext = new ViewModelDataEntry(_currentBatch);
                inserimento.Show();
            }
        }

        private void CheckBatch()
        {
            MessageBox.Show("Non implementato!");
            //TODO: da verificare come implementarlo
        }

        private int CountFiles(string path, TipoFileProcessato ext)
        {
            DirectoryInfo dinfo = new DirectoryInfo(path);
            return dinfo.GetFiles(string.Format("*.{0}", ext)).Length;
        }

        private long GetDirectorySize(string fullDirectoryPath)
        {
            long startDirectorySize = 0;
            if (!Directory.Exists(fullDirectoryPath))
                return startDirectorySize; //Return 0 while Directory does not exist.

            var currentDirectory = new DirectoryInfo(fullDirectoryPath);
            //Add size of files in the Current Directory to main size.
            currentDirectory.GetFiles().ToList().ForEach(f => startDirectorySize += f.Length);

            //Loop on Sub Direcotries in the Current Directory and Calculate it's files size.
            currentDirectory.GetDirectories().ToList()
                .ForEach(d => startDirectorySize += GetDirectorySize(d.FullName));

            return startDirectorySize;  //Return full Size of this Directory.
        }

        private double ConvertSize(double bytes, string type)
        {
            try
            {
                const int CONVERSION_VALUE = 1024;
                //determine what conversion they want
                switch (type)
                {
                    case "BY":
                        //convert to bytes (default)
                        return bytes;
                        break;
                    case "KB":
                        //convert to kilobytes
                        return (bytes / CONVERSION_VALUE);
                        break;
                    case "MB":
                        //convert to megabytes
                        return (bytes / CalculateSquare(CONVERSION_VALUE));
                        break;
                    case "GB":
                        //convert to gigabytes
                        return (bytes / CalculateCube(CONVERSION_VALUE));
                        break;
                    default:
                        //default
                        return bytes;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        private double CalculateSquare(Int32 number)
        {
            return Math.Pow(number, 2);
        }

        private double CalculateCube(Int32 number)
        {
            return Math.Pow(number, 3);
        }
    }
}
