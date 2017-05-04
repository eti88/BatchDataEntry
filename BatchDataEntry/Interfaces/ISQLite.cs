using BatchDataEntry.Abstracts;
using BatchDataEntry.Components;
using BatchDataEntry.Models;
using BatchDataEntry.Suggestions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace BatchDataEntry.Interfaces
{
    public interface ISQLite
    {
        string dbConnection { get; set; }

        int ExecuteNonQuery(string sql);
        string ExecuteScalar(string sql);

        DataTable GetDataTable(string tablename);
        DataTable GetDataTableDocumenti();
        DataTable GetDataTableWithQuery(string sql);

        bool Insert(String tableName, Dictionary<String, String> data);
        bool Update(String tableName, Dictionary<String, String> data, String where);
        bool Delete(String tableName, String where);

        int InsertRecordDocumento(Batch b, Document d);
        void UpdateRecordDocumento(Document d);
        Dictionary<int, string> GetDocumento(int id);
        NavigationList<Dictionary<int, string>> GetDocuments();

        int Insert(SuggestionSingleColumn a);

        void InitTabs();
        void CreateCacheDb(List<string> columns);
        bool CreateTableCampo();
        bool CreateTableModello();
        bool CreateTableBatch();
        bool CreateTableDocumenti(List<string> columns);
        bool CreateTableAutocompletamento();

        List<Document> GetDocumentsListPartial();
        List<Document> GetDocumentsListPartial(string query);

        List<AbsSuggestion> GetAutocompleteList(int column);
        ObservableCollection<AbsSuggestion> GetAutocompleteListOb(int column);
    }
}
