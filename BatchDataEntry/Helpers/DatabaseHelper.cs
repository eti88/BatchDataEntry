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
using BatchDataEntry.Models;
using NLog;
using SQLite;

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
                // Tabella cache generata poi
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

        public void GenerateCacheTable(ObservableCollection<Campo> colonne)
        {
            var dbCache = new SQLiteConnection(PATHDB);
            string sql = "CREATE TABLE Cache ('Id' integer NOT NULL PRIMARY KEY AUTOINCREMENT,";

            try
            {
                if (colonne.Count > 0)
                {
                    for (int i = 0; i < colonne.Count; i++)
                    {
                        // è l'ultimo elemento
                        if (i == colonne.Count - 1)
                        {
                            sql += " '" + SafeColName(colonne[i].Nome) + "' varchar";
                        }
                        else
                        {
                            sql += " '" + SafeColName(colonne[i].Nome) + "' varchar,";
                        }
                    }
                    sql += ")";
                    SQLiteCommand command = dbCache.CreateCommand(sql);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                dbCache.Close();
            }  
        }

        public int CountRecords(string sqlCmdText)
        {
            SQLiteConnection dbc = new SQLiteConnection(PATHDB);
            int count = 0;
            try
            {
                count = dbc.Execute(sqlCmdText);
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

        public DataTable GetBatchCachedTable(ObservableCollection<Campo> campi)
        {
            DataTable dt = new DataTable();
            SQLiteConnection dbc = new SQLiteConnection(PATHDB);
            try
            {
                
                dynamic obj = Utility.GenerateClass(campi);
                Type unknown = ((ObjectHandle) obj).Unwrap().GetType();
                TableMapping tm = new TableMapping(unknown);

                var tmp = dbc.Query(tm, "SELECT * FROM Cache");

                foreach (var a in tmp)
                {
                    foreach (PropertyInfo info in a.GetType().GetProperties())
                    {
                        Console.WriteLine("=== x ===");
                        Console.WriteLine(info);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                dbc.Close();
            }
            return null;
        }



        private static string SafeColName(string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char letter in input)
            {
                char l;
                if (!char.IsLetterOrDigit(letter))
                {
                    if (char.IsWhiteSpace(letter))
                    {
                        l = '_';
                    }
                    else
                    {
                        continue;
                    }   
                }
                else
                {
                    l = letter;
                }
                sb.Append(l);
            }

            return sb.ToString();
        }
    }
}
