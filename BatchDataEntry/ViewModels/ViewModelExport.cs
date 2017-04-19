using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using NLog;

namespace BatchDataEntry.ViewModels
{
    public class ViewModelExport : ViewModelMain
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private DataTable _dtSource;
        private string _destination;

        private ObservableCollection<ExportColumn> _columns;
        public ObservableCollection<ExportColumn> ColumnList
        {
            get { return _columns; }
            set
            {
                if (_columns != value)
                    _columns = value;
                RaisePropertyChanged("ColumnList");
            }
        }

        private RelayCommand _cmdGen;
        public ICommand CmdGen
        {
            get
            {
                if (_cmdGen == null)
                {
                    _cmdGen = new RelayCommand(param => GenerateCsv());
                }
                return _cmdGen;
            }
        }

        private int LastExportedIndex { get; set; }

        public ViewModelExport()
        {
            ColumnList = new ObservableCollection<ExportColumn>();
            _dtSource = new DataTable();
            LastExportedIndex = -1;
        }

        public ViewModelExport(DataTable dt, string output_path)
        {
            ColumnList = new ObservableCollection<ExportColumn>();
            _dtSource = dt;
            _destination = output_path;
            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            for (int i=0;i<columnNames.Length;i++)
            {
                if(i < 4 && i != 1) // deseleziona i campi predefiniti della tabella tranne il nome del file
                    ColumnList.Add(new ExportColumn(columnNames[i], false));
                else
                    ColumnList.Add(new ExportColumn(columnNames[i]));
            }
            RaisePropertyChanged("Columns");
            LastExportedIndex = -1;
        }

        public void GenerateCsv()
        {
            List<string> tmpRecords = new List<string>();
            int columnChecked = 0;
            for (int r = 0; r < ColumnList.Count; r++)
            {
                if (ColumnList.ElementAt(r).IsChecked)
                    columnChecked++;
            }

            try
            {
                for (int i = 0; i < _dtSource.Rows.Count; i++)
                {
                    if(Convert.ToBoolean(_dtSource.Rows[i][3]) != true) continue;
                    if (i == _dtSource.Rows.Count - 1)
                        LastExportedIndex = Convert.ToInt32(_dtSource.Rows[i][0]);

                    StringBuilder record = new StringBuilder();
                    for (int z = 0; z < _dtSource.Columns.Count; z++)
                    {
                        if(!ColumnList.ElementAt(z).IsChecked)
                            continue;

                        record.Append(_dtSource.Rows[i][z]);
                        record.Append(';');
                    }

                    if (record[record.Length - 1] == ';')
                        record.Length = record.Length - 1;

                    tmpRecords.Add(record.ToString());
                }
                Csv.AddRows(_destination, tmpRecords);      
            }
            catch (Exception e)
            {
                logger.Error(e);
                LastExportedIndex = -1;
            }
            if (LastExportedIndex != -1)
                Properties.Settings.Default.LastExportIndex = LastExportedIndex;
        }
    }
}
