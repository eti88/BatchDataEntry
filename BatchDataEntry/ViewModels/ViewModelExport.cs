﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BatchDataEntry.Business;
using BatchDataEntry.Helpers;
using BatchDataEntry.Models;
using NLog;

namespace BatchDataEntry.ViewModels
{
    internal class ViewModelExport : ViewModelMain
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

        public ViewModelExport()
        {
            ColumnList = new ObservableCollection<ExportColumn>();
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
                ColumnList.Add(new ExportColumn(columnNames[i]));
            }
            RaisePropertyChanged("Columns");
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
            }
        }
    }
}
