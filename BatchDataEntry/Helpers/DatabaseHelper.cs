using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BatchDataEntry.Models;
using NLog;
using SQLite;

namespace BatchDataEntry.Helpers
{
    
    public class DatabaseHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static readonly string DBNAME = @"database.db3";
        public static readonly string PATHDB = Path.Combine(Directory.GetCurrentDirectory(), DBNAME);

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

                    db.CreateTable<Campo>();
                    db.CreateTable<FileCSV>();
                    db.CreateTable<Modello>();
                    db.CreateTable<Batch>();
                    
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

        public void InsertRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);
            try
            {
                #if DEBUG
                Console.WriteLine(@"Insertnella tabella " + typeof(T));
                #endif

                db.Insert(model);

            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
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
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
        }

        public void DeleteRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Delete nella tabella " + typeof(T));
                #endif
                db.Delete(model);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
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

                return db.Find<Batch>(id);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
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

                return db.Find<Campo>(id);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public FileCSV GetFileCSVById(int id)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get elements by id nella tabella FileCSV");
            #endif

                return db.Find<FileCSV>(id);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
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

                return db.Find<Modello>(id);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Batch> GetBatchRecords(Batch model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Get record dalla tabella Batch");
                #endif

                var list = db.Table<Batch>().ToList();
                var obsc = new ObservableCollection<Batch>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Campo> GetCampoRecords(Campo model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get record dalla tabella Campo");
            #endif

                var list = db.Table<Campo>().ToList();
                var obsc = new ObservableCollection<Campo>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> GetBatchRecords(Modello model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
            #if DEBUG
            Console.WriteLine(@"Get record dalla tabella Batch");
            #endif

                var list = db.Table<Modello>().ToList();
                var obsc = new ObservableCollection<Modello>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
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

                var list = db.Query<Batch>(query).ToList();
                var obsc = new ObservableCollection<Batch>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
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

                var list = db.Query<Campo>(query).ToList();
                var obsc = new ObservableCollection<Campo>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<FileCSV> FileCSVQuery(string query)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Query su tabella FileCSV");
                #endif
                var list = db.Query<FileCSV>(query).ToList();
                var obsc = new ObservableCollection<FileCSV>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> Query(string query)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Query sulla tabella Modello");
                #endif
                var list = db.Query<Modello>(query).ToList();
                var obsc = new ObservableCollection<Modello>(list);

                return obsc;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString);
                #if DEBUG
                Console.WriteLine(e.ToString());
                #endif
            }
            finally
            {
                db.Close();
            }
            return null;
        }
    }
}
