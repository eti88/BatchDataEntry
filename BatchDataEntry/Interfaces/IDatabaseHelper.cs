using BatchDataEntry.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BatchDataEntry.Interfaces
{
    public interface IDatabaseHelper
    {
        int Count(string sql);

        int Insert(Batch b);
        int Insert(Modello m);
        int Insert(Campo c);

        void Update(Batch b);
        void Update(Modello m);
        void Update(Campo c);

        Batch GetBatchById(int id);
        Modello GetModelloById(int id);
        Campo GetCampoById(int id);

        ObservableCollection<Batch> GetBatchRecords();
        ObservableCollection<Campo> GetCampoRecords();
        ObservableCollection<Modello> GetModelloRecords();

        ObservableCollection<Batch> BatchQuery(string query);
        ObservableCollection<Campo> CampoQuery(string query);
        ObservableCollection<Modello> ModelloQuery(string query);

        IEnumerable<Modello> IEnumerableModelli();

        void DeleteFromTable(string tableName, string condition);
        void DropAllRowsFromTable(string tableName);

        
    }
}
