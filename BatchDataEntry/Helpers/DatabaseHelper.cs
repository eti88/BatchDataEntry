using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        private void ErrorCatch(Exception e)
        {
            logger.Error(e.ToString);
            #if DEBUG
            Console.WriteLine(e.ToString());
            #endif
        }

        /* TODO: Verificare se effettivamente serve effettuare la conversione degli oggetti in DBModels
         * prima di effettuare le operazioni di inserimento, modifica, eliminazione su generic types
         */

        public void InsertRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);
            try
            {
                #if DEBUG
                Console.WriteLine(@"Insertnella tabella " + typeof(T));
                #endif

                switch (typeof(T).ToString())
                {
                    case "Batch":
                        var a = model as Batch;
                        var tmpa = new DBModels.Batch(a);
                        db.Insert(tmpa);
                        break;

                    case "Campo":
                        var b = model as Campo;
                        var tmpb = new DBModels.Campo(b);
                        db.Insert(tmpb);
                        break;

                    case "Modello":
                        var c = model as Modello;
                        var tmpc = new DBModels.Modello(c);
                        db.Insert(tmpc);
                        break;
                }
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

        public void UpdateRecord<T>(T model)
        {
            var db = new SQLiteConnection(PATHDB);

            try
            {
                #if DEBUG
                Console.WriteLine(@"Update nella tabella " + typeof(T));
                #endif

                switch (typeof(T).ToString())
                {
                    case "Batch":
                        var a = model as Batch;
                        var tmpa = new DBModels.Batch(a);
                        db.Update(tmpa);
                        break;

                    case "Campo":
                        var b = model as Campo;
                        var tmpb = new DBModels.Campo(b);
                        db.Update(tmpb);
                        break;

                    case "Modello":
                        var c = model as Modello;
                        var tmpc = new DBModels.Modello(c);
                        db.Update(tmpc);
                        break;
                }
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
                Modello m = new Modello(db.Find<DBModels.Modello>(id));
                return m;
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
    }
}
