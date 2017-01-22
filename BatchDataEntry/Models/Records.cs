using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace BatchDataEntry.Models
{
    /// <summary>
    /// Contiene tutti i record del file
    /// </summary>
    [Serializable]
    public class Records
    {
        public List<RecordRow> Rows { get; set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Records()
        {
            this.Rows = new List<RecordRow>();
        }

        public Records(List<RecordRow> r)
        {
            this.Rows = r;
        }

        public async Task Save(string path)
        {
            if(this.Rows.Count == 0)
                return;

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    formatter.Serialize(file, this);
                }
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
                logger.Error("[HashTableSave] " + e.ToString());
            }

        }

        public void Load(string path)
        {
            if(!File.Exists(path))
                return;

            Records rec = null;
            try
            {
                BinaryFormatter bw = new BinaryFormatter();
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    rec = (Records) bw.Deserialize(file);
                }
                this.Rows = rec.Rows;
            }
            catch (Exception e)
            {
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
                logger.Error("[ReadHashTable] " + e.ToString());
            }       
        }
    }

    [Serializable]
    public class RecordRow
    {
        public RecordRow()
        {
            this.Cells = new Dictionary<string, string>();
        }

        public RecordRow(int i, Dictionary<string, string> c)
        {
            this.Id = i;
            this.Cells = c;
        }

        public int Id { get; set; }
        public Dictionary<string, string> Cells;
    }
}
