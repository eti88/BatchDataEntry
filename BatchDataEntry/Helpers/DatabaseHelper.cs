using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Documents;
using BatchDataEntry.Business;
using BatchDataEntry.Components;
using BatchDataEntry.DBModels;
using BatchDataEntry.Models;
using NLog;
using SQLite;
using Batch = BatchDataEntry.Models.Batch;
using Campo = BatchDataEntry.Models.Campo;
using Modello = BatchDataEntry.Models.Modello;

namespace BatchDataEntry.Helpers
{
    
    public class DatabaseHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string DBNAME = @"database.db3";
        public string PATHDB = @"";

        public DatabaseHelper()
        {
            this.PATHDB = Path.Combine(Directory.GetCurrentDirectory(), DBNAME);
        }

        public DatabaseHelper(string path)
        {
            this.PATHDB = Path.Combine(path);
        }

        public DatabaseHelper(string dbname, string dbpath)
        {
            this.DBNAME = dbname;
            this.PATHDB = Path.Combine(dbpath, dbname);
        }

        public void InitTabs()
        {
            try
            {
                #if DEBUG
                Console.WriteLine(@"Init tabelle...");
                #endif

                if (!File.Exists(PATHDB))
                {
                    var db = new SQLiteConnection(PATHDB);
                    
                    db.CreateTable<DBModels.Campo>();
                    db.CreateTable<DBModels.Modello>();
                    db.CreateTable<DBModels.Batch>();
                    
                }

                #if DEBUG
                Console.WriteLine(@"Init tabelle completato.");
                #endif
            }
            catch (Exception e)
            {
                logger.Error(string.Format("{0} | {1}", e.Source, e.Message));
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
        }

        public void CreateCacheDb()
        {
            #if DEBUG
            Console.WriteLine(@"Create cache database...");
            #endif

            if (!File.Exists(PATHDB))
            {
                var dbCache = new SQLiteConnection(PATHDB);
                dbCache.CreateTable<DBModels.Documento>();
                dbCache.CreateTable<DBModels.Autocompletamento>();
            }

            #if DEBUG
            Console.WriteLine(@"Cache db creato....");
            #endif
            
        }

        private void ErrorCatch(Exception e)
        {
            logger.Error(e.ToString);
            #if DEBUG
            Console.WriteLine(e.ToString());
            #endif
        }

        public int InsertRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);
            int lastID = -1;

            try
            {
                #if DEBUG
                Console.WriteLine(@"Insertnella tabella " + typeof(T));
                #endif
                db.Insert(model);
                lastID = db.ExecuteScalar<int>("SELECT last_insert_rowid()");

            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return lastID;
        }

        public void UpdateRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Update nella tabella " + typeof(T));
                #endif
                db.Update(model);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
        }

        public void DeleteRecord<T>(T model, int id)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Delete nella tabella " + typeof(T));
                #endif

                db.Delete<T>(id);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
        }

        public Batch GetBatchById(int id)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Get elements by id nella tabella Batch");
                #endif
                Batch b = new Batch(db.Find<DBModels.Batch>(id));
                
                return b;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public Campo GetCampoById(int id)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Get elements by id nella tabella Campo");
                #endif
                Campo c = new Campo(db.Find<DBModels.Campo>(id));
                return c;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public Modello GetModelloById(int id)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get elements by id nella tabella Modello");
            #endif
                DBModels.Modello raw = new DBModels.Modello();
                raw = db.Find<DBModels.Modello>(id);
                if (raw != null)
                    return new Modello(raw);
                
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public Documento GetDocumento(string name)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                var query = db.Table<Documento>().Where(x => x.FileName.Equals(name)).First();
                return query;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public NavigationList<Doc> GetDocuments()
        {
            NavigationList<Doc> ret = new NavigationList<Doc>();
            var db = new SQLiteConnection(PATHDB);
            try
            {
                var tmp = db.Table<Documento>().ToList();
                foreach (Documento doc in tmp)
                {
                    ret.Add(new Doc(doc));
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }finally { db.Close();}

            return ret;
        }

        public List<string> GetAutocompleteList(string column)
        {
            var lst = new List<string>();
            var db = new SQLiteConnection(PATHDB);
            try
            {
                var tmp = db.Table<Autocompletamento>().Where(x => x.Colonna.Equals(column));
                if (tmp.Count() > 0)
                {
                    foreach (var record in tmp)
                    {
                        lst.Add(record.Valore);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }finally { db.Close(); }

            return lst;
        }

        public ObservableCollection<Batch> GetBatchRecords()
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Get record dalla tabella Batch");
                #endif

                var list = db.Table<DBModels.Batch>().ToList();
                var obsc = new ObservableCollection<Batch>();
                foreach (DBModels.Batch element in list)
                {
                    obsc.Add(new Batch(element));
                }


                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Campo> GetCampoRecords()
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get record dalla tabella Campo");
            #endif

                var list = db.Table<DBModels.Campo>().ToList();
                var obsc = new ObservableCollection<Campo>();
                foreach (DBModels.Campo element in list)
                {
                    obsc.Add(new Campo(element));
                }
                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> GetModelloRecords()
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get record dalla tabella Batch");
            #endif

                var list = db.Table<DBModels.Modello>().ToList();
                var obsc = new ObservableCollection<Modello>();
                foreach (DBModels.Modello element in list)
                {
                    obsc.Add(new Modello(element));
                }
                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Batch> BatchQuery(string query)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Query su tabella Batch");
                #endif

                var list = db.Query<DBModels.Batch>(query).ToList();
                var obsc = new ObservableCollection<Batch>();
                foreach (DBModels.Batch element in list)
                {
                    obsc.Add(new Batch(element));
                }
                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Campo> CampoQuery(string query)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Query su tabella Campo");
                #endif

                var list = db.Query<DBModels.Campo>(query).ToList();
                var obsc = new ObservableCollection<Campo>();
                foreach (DBModels.Campo element in list)
                {
                    obsc.Add(new Campo(element));
                }
                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> ModelloQuery(string query)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Query sulla tabella Modello");
                #endif
                var list = db.Query<DBModels.Modello>(query).ToList();
                var obsc = new ObservableCollection<Modello>();
                foreach (DBModels.Modello element in list)
                {
                    obsc.Add(new Modello(element));
                }
                return obsc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public IEnumerable<DBModels.Modello> IEnumerableModelli()
        {
            var db = new SQLiteConnection(PATHDB);
            string querycmd = "SELECT Id, Nome FROM Modello";
            try
            {
                #if DEBUG
                Console.WriteLine(@"Query sulla tabella Modello");
                #endif
                var list = db.Query<DBModels.Modello>(querycmd).ToList();
                IEnumerable<DBModels.Modello> tmp = list.AsEnumerable();

                return tmp;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public int CountRecords(string sqlCmdText)
        {
            SQLiteConnection dbc = new SQLiteConnection(PATHDB);
            int count = 0;
            try
            {
                count = dbc.ExecuteScalar<int>(sqlCmdText);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                dbc.Close();
            }
            return count;
        }
      
    }
}
