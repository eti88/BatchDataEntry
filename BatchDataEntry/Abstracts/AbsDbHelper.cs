using BatchDataEntry.Interfaces;
using System.Collections.Generic;
using BatchDataEntry.Models;
using System.Collections.ObjectModel;

namespace BatchDataEntry.Abstracts
{
    public abstract class AbsDbHelper : IDatabaseHelper
    {
        public abstract ObservableCollection<Batch> BatchQuery(string query);
        public abstract ObservableCollection<Campo> CampoQuery(string query);
        public abstract int Count(string sql);
        public abstract void DeleteFromTable(string tableName, string condition);
        public abstract void DropAllRowsFromTable(string tableName);
        public abstract Batch GetBatchById(int id);
        public abstract ObservableCollection<Batch> GetBatchRecords();
        public abstract Campo GetCampoById(int id);
        public abstract ObservableCollection<Campo> GetCampoRecords();
        public abstract Modello GetModelloById(int id);
        public abstract ObservableCollection<Modello> GetModelloRecords();
        public abstract IEnumerable<Modello> IEnumerableModelli();
        public abstract int Insert(Batch b);
        public abstract int Insert(Modello m);
        public abstract int Insert(Campo c);
        public abstract ObservableCollection<Modello> ModelloQuery(string query);
        public abstract void Update(Batch b);
        public abstract void Update(Modello m);
        public abstract void Update(Campo c);
        public abstract void DeleteReference(string v);
    }
}
