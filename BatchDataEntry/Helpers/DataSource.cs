using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using BatchDataEntry.Models;

namespace BatchDataEntry.Helpers
{
    public static class DataSource
    {
        public static ICollection CreateDataSourceFromCsv(string filecsv, ObservableCollection<Campo> colonne)
        {
            DataTable dt = new DataTable();
            DataRow dr;

            foreach (Campo campo in colonne)
            {
                dt.Columns.Add(new DataColumn(campo.Nome, typeof(string)));
            }

            foreach (string riga in File.ReadLines(filecsv))
            {
                dr = dt.NewRow();
                for (int i = 0; i < colonne.Count; i++)
                {
                    dr[i] = riga.Split(';').ElementAt(i);
                }
                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);
            return dv;
        }
    }
}
