using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using BatchDataEntry.Components;
using BatchDataEntry.Models;
using NLog;
using Batch = BatchDataEntry.Models.Batch;
using Campo = BatchDataEntry.Models.Campo;
using Modello = BatchDataEntry.Models.Modello;
using BatchDataEntry.Suggestions;
using BatchDataEntry.Interfaces;
using BatchDataEntry.Abstracts;
using BatchDataEntry.ViewModels;

namespace BatchDataEntry.Helpers
{
    
    public class DatabaseHelper : AbsDbHelper, ISQLite 
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string dbConnection { get; set; }

        #region Constructors

        public DatabaseHelper()
        {
            string dtsource = Path.Combine(Directory.GetCurrentDirectory(), @"database.db3");
            dbConnection = string.Format("Data Source={0}", UNCPathFormat(dtsource));
            #if DEBUG
            Console.WriteLine("CNN:" + dbConnection);
            #endif
        }

        public DatabaseHelper(string path)
        {
            string dtsource = UNCPathFormat(path);
            dbConnection = string.Format("Data Source={0}", dtsource);
            #if DEBUG
            Console.WriteLine("CNN:" + dbConnection);
            #endif
        }

        public DatabaseHelper(string dbname, string dbpath)
        {
            string tmppath = Path.Combine(dbpath, dbname);
            string dtsource = UNCPathFormat(tmppath);
            dbConnection = string.Format("Data Source={0}", dtsource);
            #if DEBUG
            Console.WriteLine("CNN:" + dbConnection);
            #endif
        }

        private string UNCPathFormat(string intxt)
        {
            if (intxt.StartsWith("\\"))
            {
                string tmp = intxt.Substring(2, intxt.Length - 2);
                return @"\\\\" + tmp;
            }
            else
                return intxt;
        }
#endregion

#region DataTables

        /// <summary>
        /// Permette di recuperare sottoforma di Datatable una tabella dal database
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public  DataTable GetDataTable(string tablename)
        {
            DataTable dt = new DataTable();
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            try
            {
                string cmd = "SELECT * FROM @tablename";
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(cmd, cnn)) {
                    myCmd.Parameters.AddWithValue("@tablename", tablename);

                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return dt;
        }

        public DataTable GetDataTableDocumenti()
        {
            DataTable dt = new DataTable();
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            try
            {
                string cmd = String.Format("SELECT * FROM Documenti");              
                
                cnn.Open();

                using (SQLiteCommand myCmd = new SQLiteCommand(cmd, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return dt;
        }

        public DataTable GetDataTableWithQuery(string sqlcmd)
        {
            DataTable dt = new DataTable();
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            if (string.IsNullOrEmpty(sqlcmd))
                return null;

            sqlcmd = convertQuotes(sqlcmd);

            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sqlcmd, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return dt;
        }

#endregion

#region AzioniGeneriche

        /// <summary>
        /// Permette di eseguire una query che ritorna un intero
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int Count(string sql)
        {
            int result = -1;
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            try
            {
                cnn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, cnn))
                {
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
                result = -1;
            }
            finally
            {
                cnn.Close();
            }
            return result;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            int rowsUpdated = 0;
            sql = convertQuotes(sql);
            try
            {
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand(sql,cnn))
                {
                    rowsUpdated = mycommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }

            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            sql = convertQuotes(sql);
            try
            {
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand(sql,cnn))
                {
                    object value = mycommand.ExecuteScalar();
                    if (value != null)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
                return "";
            }
            finally
            {
                cnn.Close();
            }
        }

        public static string convertQuotes(string str)
        {
            try
            {
                if(str == null)
                    return String.Empty;

                return str.Replace("'", "''");
            }
            catch (Exception)
            {
                return String.Empty;
            }
            
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    if(val.Value == null)
                        vals += String.Format(" {0} = '{1}',", convertQuotes(val.Key.ToString()), String.Empty);
                    else
                        vals += String.Format(" {0} = '{1}',", convertQuotes(val.Key.ToString()), convertQuotes(val.Value.ToString()));
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
#if DEBUG
                Console.WriteLine(String.Format("SQL: update {0} set {1} where {2};", tableName, vals, where));
#endif

                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch (Exception e)
            {
                returnCode = false;
                ErrorCatch(e);
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception e)
            {
                ErrorCatch(e);
                returnCode = false;
            }
            return returnCode;
        }

        public bool Delete(String query)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", convertQuotes(val.Key.ToString()));
                values += String.Format(" '{0}',", convertQuotes(val.Value));
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
#if DEBUG
            Console.WriteLine(String.Format("Columns: {0} \n Values: {1}\n", columns, values));
#endif

            try
            {
#if DEBUG
                Console.WriteLine(String.Format("SQL: insert into {0}({1}) values({2});", tableName, columns, values));
#endif

                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));             
            }
            catch (Exception e)
            {
                ErrorCatch(e);
                returnCode = false;
            }
            return returnCode;
        }

        private void ErrorCatch(Exception e)
        {
            logger.Error(e.ToString);
#if DEBUG
            Console.WriteLine(e.ToString());
#endif
        }

#endregion

#region TableCreation

        public void InitTabs()
        {
            try
            {
                CreateTableModello();
                CreateTableCampo();
                CreateTableBatch();
            }
            catch (Exception e)
            {
                logger.Error(string.Format("{0} | {1}", e.Source, e.Message));
            }
        }

        public void CreateCacheDb(List<string> columns)
        {
#if DEBUG
            Console.WriteLine(@"Create cache database...");
#endif
            CreateTableAutocompletamento();
            CreateTableDocumenti(columns);
#if DEBUG
            Console.WriteLine(@"Cache db creato....");
#endif

        }

        public bool CreateTableCampo()
        {
            string SQLCmd = @"CREATE TABLE IF NOT EXISTS Campo (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "Nome VARCHAR(254)," +
                            "Posizione INTEGER," +
                            "SalvaValori INTEGER," +
                            "ValorePredefinito VARCHAR(254)," +
                            "IndicePrimario INTEGER," +
                            "TipoCampo INTEGER," +
                            "IdModello INTEGER," +
                            "Riproponi INTEGER," +
                            "Disabilitato INTEGER," +
                            "IndiceSecondario INTEGER)";
            try
            {
                this.ExecuteNonQuery(SQLCmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool CreateTableModello()
        {
            string SQLCmd = @"CREATE TABLE IF NOT EXISTS Modello (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "Nome VARCHAR(254)," +
                            "OrigineCsv INTEGER," +
                            "PathFileCsv TEXT," +
                            "Separatore VARCHAR(1)," +
                            "FocusColumn INTEGER DEFAULT 0," +
                            "CsvColumn INTEGER DEFAULT 0)";
            try
            {
                this.ExecuteNonQuery(SQLCmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateTableBatch()
        {
            string SQLCmd = @"CREATE TABLE IF NOT EXISTS Batch (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "Nome VARCHAR(254)," +
                            "TipoFile INTEGER," +
                            "DirectoryInput TEXT," +
                            "DirectoryOutput TEXT," +
                            "IdModello INTEGER," +
                            "NumDoc INTEGER," +
                            "NumPages INTEGER," +
                            "DocCorrente INTEGER," +
                            "UltimoIndicizzato INTEGER," +
                            "PatternNome TEXT," +
                            "UltimoDocumentoEsportato TEXT)";
            try
            {
                this.ExecuteNonQuery(SQLCmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Crea la tabella dei documenti con alcune colonne predefinite e inoltre aggiunge le colonne per il salvataggio dei valori
        /// </summary>
        /// <param name="columns">Lista dei nomi delle colonne</param>
        /// <returns></returns>
        public bool CreateTableDocumenti(List<string> columns)
        {
            // Nel caso si voglia aggiungere il supporto per più tipi di file va modificato in dictionary il parametro columns
            // per passare anche il tipo di campo (adesso si assume che sia sempre string)
            StringBuilder sqlCmd = new StringBuilder();
            sqlCmd.Append(@"CREATE TABLE IF NOT EXISTS Documenti (Id INTEGER PRIMARY KEY AUTOINCREMENT,FileName VARCHAR(254),Path TEXT,isIndicizzato INTEGER");

            if (columns.Count > 0)
            {
                sqlCmd.Append(",");
                for (int i = 0; i < columns.Count; i++)
                {
                    if (i == columns.Count - 1)
                        sqlCmd.Append(string.Format("{0} VARCHAR(254)", columns[i]));
                    else
                        sqlCmd.Append(string.Format("{0} VARCHAR(254),", columns[i]));
                }

            }
            sqlCmd.Append(")");
            
            #if DEBUG
            Console.WriteLine(sqlCmd);
            #endif
            try
            {
                this.ExecuteNonQuery(sqlCmd.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateTableAutocompletamento()
        {
            string SQLCmd = @"CREATE TABLE IF NOT EXISTS Autocompletamento (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                            "Colonna INTEGER, Valore VARCHAR(254))";
            try
            {
                this.ExecuteNonQuery(SQLCmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

#endregion

#region InsertTables

        public override int Insert(Batch b)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            //values.Add("Id", b.Id.ToString());
            values.Add("Nome", b.Nome);
            values.Add("TipoFile", string.Format("{0}", (int)b.TipoFile));
            values.Add("DirectoryInput", b.DirectoryInput);
            values.Add("DirectoryOutput", b.DirectoryOutput);
            values.Add("IdModello", b.IdModello.ToString());
            values.Add("NumDoc", b.NumDoc.ToString());
            values.Add("NumPages", b.NumPages.ToString());
            values.Add("DocCorrente", b.DocCorrente.ToString());
            values.Add("UltimoIndicizzato", b.UltimoIndicizzato.ToString());
            values.Add("PatternNome", b.PatternNome);
            values.Add("UltimoDocumentoEsportato", b.UltimoDocumentoEsportato);

            bool r = Insert("Batch", values);
            if (r)
                return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Batch"));
          
            return -1;
        }

        public override int Insert(Campo c)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            //values.Add("Id", c.Id.ToString());
            values.Add("Nome", c.Nome);
            values.Add("Posizione", c.Posizione.ToString());
            int tmp1 = Convert.ToInt32(c.SalvaValori);
            values.Add("SalvaValori", tmp1.ToString());
            values.Add("ValorePredefinito", c.ValorePredefinito);
            int tmp2 = Convert.ToInt32(c.IndicePrimario);
            values.Add("IndicePrimario", tmp2.ToString());
            int tmpS = Convert.ToInt32(c.IndiceSecondario);
            values.Add("IndiceSecondario", tmpS.ToString());
            values.Add("TipoCampo", ((int)c.TipoCampo).ToString());
            values.Add("IdModello", c.IdModello.ToString());
            int rip = Convert.ToInt32(c.Riproponi);
            values.Add("Riproponi", rip.ToString());
            int ds = Convert.ToInt32(c.IsDisabilitato);
            values.Add("Disabilitato", ds.ToString());

            bool r = Insert("Campo", values);
            if (r)
                return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Campo"));

            return -1;
        }

        public int Insert(SuggestionSingleColumn a)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            if(a is SuggestionSingleColumn)
            {
                var tmp = a as SuggestionSingleColumn;
                values.Add("Colonna", tmp.Colonna.ToString());
                values.Add("Valore", tmp.Valore);

                bool r = Insert("Autocompletamento", values);
                if (r)
                    return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Autocompletamento"));
            }
            return -1;
        }

        public override int Insert(Modello m)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            if(m.Id > 0)
                values.Add("Id", m.Id.ToString());
            values.Add("Nome", m.Nome);

            int getBool = Convert.ToInt32(m.OrigineCsv);

            values.Add("OrigineCsv", getBool.ToString());
            values.Add("PathFileCsv", m.PathFileCsv);
            values.Add("Separatore", m.Separatore);
            values.Add("FocusColumn", m.StartFocusColumn.ToString());
            values.Add("CsvColumn", m.CsvColumn.ToString());
            bool r = Insert("Modello", values);
            if (r)
                return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Modello"));

            return -1;
        }

        public int InsertRecordDocumento(Batch b, Document d)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Id", d.Id.ToString());
            values.Add("FileName", d.FileName);
            values.Add("Path", d.Path);
            int tmp = Convert.ToInt32(d.IsIndexed);
            values.Add("isIndicizzato", tmp.ToString());

            for (int i = 0; i < d.Voci.Count; i++)
            {
                if (string.IsNullOrEmpty(d.Voci[i].Valore))
                    values.Add(d.Voci[i].Nome, string.Empty);
                else
                    values.Add(b.Applicazione.Campi.ElementAt(i).Nome, d.Voci[i].Valore);
            }

            //foreach (Campo col in d.Voci)
            //{
                
            //}
         
            bool r = Insert("Documenti", values);
            if (r)
                return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Documenti"));

            return -1;
        }

#endregion

#region UpdateTables

        public override void Update(Batch b)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Id", b.Id.ToString());
            values.Add("Nome", b.Nome);
            values.Add("TipoFile", string.Format("{0}", (int)b.TipoFile));
            values.Add("DirectoryInput", b.DirectoryInput);
            values.Add("DirectoryOutput", b.DirectoryOutput);
            values.Add("IdModello", b.IdModello.ToString());
            values.Add("NumDoc", b.NumDoc.ToString());
            values.Add("NumPages", b.NumPages.ToString());
            values.Add("DocCorrente", b.DocCorrente.ToString());
            values.Add("UltimoIndicizzato", b.UltimoIndicizzato.ToString());
            values.Add("PatternNome", b.PatternNome);
            values.Add("UltimoDocumentoEsportato", b.UltimoDocumentoEsportato);
            Update("Batch", values, string.Format("Id={0}", b.Id));
        }

        public override void Update(Campo c)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Id", c.Id.ToString());
            values.Add("Nome", c.Nome);
            values.Add("Posizione", c.Posizione.ToString());
            int tmp1 = Convert.ToInt32(c.SalvaValori);
            values.Add("SalvaValori", tmp1.ToString());
            values.Add("ValorePredefinito", c.ValorePredefinito);
            int tmp2 = Convert.ToInt32(c.IndicePrimario);
            values.Add("IndicePrimario", tmp2.ToString());
            int tmpS = Convert.ToInt32(c.IndiceSecondario);
            values.Add("IndiceSecondario", tmpS.ToString());
            values.Add("TipoCampo", ((int)c.TipoCampo).ToString());
            values.Add("IdModello", c.IdModello.ToString());
            int rip = Convert.ToInt32(c.Riproponi);
            values.Add("Riproponi", rip.ToString());
            int ds = Convert.ToInt32(c.IsDisabilitato);
            values.Add("Disabilitato", ds.ToString());

            Update("Campo", values, string.Format("Id={0}", c.Id));
        }

        public override void Update(Modello m)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Id", m.Id.ToString());
            values.Add("Nome", m.Nome);
            int getBool = Convert.ToInt32(m.OrigineCsv);
            values.Add("OrigineCsv", getBool.ToString());
            values.Add("PathFileCsv", m.PathFileCsv);
            values.Add("Separatore", m.Separatore);
            values.Add("FocusColumn", m.StartFocusColumn.ToString());
            values.Add("CsvColumn", m.CsvColumn.ToString());
            Update("Modello", values, string.Format("Id={0}", m.Id));
        }

        public void UpdateRecordDocumento(Document d)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Id", d.Id.ToString());
            values.Add("FileName", d.FileName);
            values.Add("Path", d.Path);
            int b = Convert.ToInt32(d.IsIndexed);
            values.Add("isIndicizzato", b.ToString());

            foreach (Record col in d.Voci)
            {
                if(string.IsNullOrEmpty(col.Valore))
                    values.Add(col.Nome, string.Empty);
                else
                    values.Add(col.Nome, col.Valore);
            }

            Update("Documenti", values, string.Format("Id={0}", d.Id));
        }

#endregion

        public override Batch GetBatchById(int id)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = string.Format("SELECT * FROM Batch WHERE Id = @Id");
            Batch b = new Batch();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    myCmd.Parameters.AddWithValue("@Id", id);

                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        b.Id = Convert.ToInt32(reader["Id"]);
                        b.Nome = Convert.ToString(reader["Nome"]);
                        b.TipoFile = (TipoFileProcessato)Convert.ToInt32(reader["TipoFile"]);
                        b.DirectoryInput = Convert.ToString(reader["DirectoryInput"]);
                        b.DirectoryOutput = Convert.ToString(reader["DirectoryOutput"]);
                        b.IdModello = Convert.ToInt32(reader["IdModello"]);
                        b.NumDoc = Convert.ToInt32(reader["NumDoc"]);
                        b.NumPages = Convert.ToInt32(reader["NumPages"]);
                        b.DocCorrente = Convert.ToInt32(reader["DocCorrente"]);
                        b.UltimoIndicizzato = Convert.ToInt32(reader["UltimoIndicizzato"]);
                        b.PatternNome = Convert.ToString(reader["PatternNome"]);
                        b.UltimoDocumentoEsportato = Convert.ToString(reader["UltimoDocumentoEsportato"]);
                    }
                    reader.Close();
                }               
                return b;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override Campo GetCampoById(int id)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Campo WHERE Id = @Id";
            Campo c = new Campo();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    myCmd.Parameters.AddWithValue("@Id", id);
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        c.Id = Convert.ToInt32(reader["Id"]);
                        c.Nome = Convert.ToString(reader["Nome"]);
                        c.Posizione = Convert.ToInt32(reader["Posizione"]);
                        c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                        c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                        c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                        c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                        c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                        c.IdModello = Convert.ToInt32(reader["IdModello"]);
                        c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                        c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
                    }
                    reader.Close();
                }                  
                return c;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override Modello GetModelloById(int id)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Modello WHERE Id = @Id";
            Modello m = new Modello();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    myCmd.Parameters.AddWithValue("@Id", id);
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        m.Id = Convert.ToInt32(reader["Id"]);
                        m.Nome = Convert.ToString(reader["Nome"]);
                        m.OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]);
                        m.PathFileCsv = Convert.ToString(reader["PathFileCsv"]);
                        m.Separatore = Convert.ToString(reader["Separatore"]);
                        m.StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]);
                        m.CsvColumn = Convert.ToInt32(reader["CsvColumn"]);
                    }
                    reader.Close();
                }
                return m;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public Dictionary<int, string> GetDocumento(int id)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Documenti WHERE Id = @Id";
            Dictionary<int, string> doc = new Dictionary<int, string>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    myCmd.Parameters.AddWithValue("@Id", id);
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int i = 0;
                        doc.Add(i, Convert.ToString(reader["Id"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["FileName"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["Path"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["isIndicizzato"]));
                        i++;

                        for (int z = i; z < reader.FieldCount; z++)
                        {
                            doc.Add(z, Convert.ToString(reader[z]));
                        }
                    }
                    reader.Close();
                }
                return doc;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public NavigationList<Dictionary<int, string>> GetDocuments()
        {
            NavigationList<Dictionary<int, string>> ret = new NavigationList<Dictionary<int, string>>();
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Documenti";
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Dictionary<int, string> doc = new Dictionary<int, string>();
                        int i = 0;
                        doc.Add(i, Convert.ToString(reader["Id"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["FileName"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["Path"]));
                        i++;
                        doc.Add(i, Convert.ToString(reader["isIndicizzato"]));
                        i++;

                        for (int z = i; z < reader.FieldCount; z++)
                        {
                            doc.Add(z, Convert.ToString(reader[z]));

                        }
                        ret.Add(doc);
                    }
                    reader.Close();
                }  
                return ret;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public List<AbsSuggestion> GetAutocompleteList(int column)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT Valore FROM Autocompletamento WHERE Colonna = @column";
            var suggestions = new List<AbsSuggestion>();
            try
            {
                cnn.Open();
                using(SQLiteCommand myCmd = new SQLiteCommand(sql, cnn)) {
                    myCmd.Parameters.AddWithValue("@column", column);
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                        suggestions.Add(new SuggestionSingleColumn(Convert.ToString(reader["Valore"])));

                    reader.Close();
                }
                return suggestions;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<AbsSuggestion> GetAutocompleteListOb(int column)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT Valore FROM Autocompletamento WHERE Colonna = @column";
            var suggestions = new ObservableCollection<AbsSuggestion>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    myCmd.Parameters.AddWithValue("@column", column);
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    
                    while (reader.Read())
                        suggestions.Add(new SuggestionSingleColumn(Convert.ToString(reader["Valore"])));

                    reader.Close();
                }
                   
                return suggestions;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Batch> GetBatchRecords()
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Batch";
            ObservableCollection<Batch> batches = new ObservableCollection<Batch>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Batch b = new Batch();
                        b.Id = Convert.ToInt32(reader["Id"]);
                        b.Nome = Convert.ToString(reader["Nome"]);
                        b.TipoFile = (TipoFileProcessato)Convert.ToInt32(reader["TipoFile"]);
                        b.DirectoryInput = Convert.ToString(reader["DirectoryInput"]);
                        b.DirectoryOutput = Convert.ToString(reader["DirectoryOutput"]);
                        b.IdModello = Convert.ToInt32(reader["IdModello"]);
                        b.NumDoc = Convert.ToInt32(reader["NumDoc"]);
                        b.NumPages = Convert.ToInt32(reader["NumPages"]);
                        b.DocCorrente = Convert.ToInt32(reader["DocCorrente"]);
                        b.UltimoIndicizzato = Convert.ToInt32(reader["UltimoIndicizzato"]);
                        b.PatternNome = Convert.ToString(reader["PatternNome"]);
                        b.UltimoDocumentoEsportato = Convert.ToString(reader["UltimoDocumentoEsportato"]);
                        batches.Add(b);
                    }

                    reader.Close();
                }      
                return batches;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public List<Document> GetDocumentsListPartial()
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Documenti";
            List<Document> docs = new List<Document>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Document doc = new Document();
                        doc.Id = Convert.ToInt32(reader["Id"]);
                        doc.FileName = Convert.ToString(reader["FileName"]);
                        doc.Path = Convert.ToString(reader["Path"]);
                        doc.IsIndexed = Convert.ToBoolean(reader["isIndicizzato"]);
                        docs.Add(doc);
                    }

                    reader.Close();
                } 
                return docs;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public List<FidelityClient> GetRecords()
        {
            var res = new List<FidelityClient>();
            var cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Documenti";
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        FidelityClient record = new FidelityClient();
                        record.FileName = Convert.ToString(reader["FileName"]);
                        record.Card = Convert.ToString(reader["FidelityCard"]);
                        record.Cognome = Convert.ToString(reader["Cognome"]);
                        record.Nome = Convert.ToString(reader["Nome"]);
                        record.Indirizzo = Convert.ToString(reader["Indirizzo"]);
                        record.Civico = Convert.ToString(reader["Civico"]);
                        record.Localita = Convert.ToString(reader["Localita"]);
                        record.Provincia = Convert.ToString(reader["Provincia"]);
                        record.Cap = Convert.ToString(reader["Cap"]);
                        record.Prefisso = Convert.ToString(reader["Prefisso"]);
                        record.Telefono = Convert.ToString(reader["Telefono"]);
                        record.Cellulare = Convert.ToString(reader["Cellulare"]);
                        record.Email = Convert.ToString(reader["Email"]);
                        record.DataNascita = Convert.ToString(reader["DataDiNascita"]);
                        record.Luogo = Convert.ToString(reader["Luogo"]);

                        res.Add(record);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return res;
        }

        public List<Document> GetDocumentsListPartial(string query)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            List<Document> docs = new List<Document>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(query, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Document doc = new Document();
                        doc.Id = Convert.ToInt32(reader["Id"]);
                        doc.FileName = Convert.ToString(reader["FileName"]);
                        doc.Path = Convert.ToString(reader["Path"]);
                        doc.IsIndexed = Convert.ToBoolean(reader["isIndicizzato"]);
                        docs.Add(doc);
                    }

                    reader.Close();
                }    
                return docs;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Campo> GetCampoRecords()
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Campo";
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Campo c = new Campo();
                        c.Id = Convert.ToInt32(reader["Id"]);
                        c.Nome = Convert.ToString(reader["Nome"]);
                        c.Posizione = Convert.ToInt32(reader["Posizione"]);
                        c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                        c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                        c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                        c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                        c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                        c.IdModello = Convert.ToInt32(reader["IdModello"]);
                        c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                        c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
                        campi.Add(c);
                    }

                    reader.Close();
                }        
                return campi;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Modello> GetModelloRecords()
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Modello";
            ObservableCollection<Modello> models = new ObservableCollection<Modello>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Modello m = new Modello();
                        m.Id = Convert.ToInt32(reader["Id"]);
                        m.Nome = Convert.ToString(reader["Nome"]);
                        m.OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]);
                        m.PathFileCsv = Convert.ToString(reader["PathFileCsv"]);
                        m.Separatore = Convert.ToString(reader["Separatore"]);
                        m.StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]);
                        m.CsvColumn = Convert.ToInt32(reader["CsvColumn"]);
                        models.Add(m);
                    }
                    reader.Close();
                }                
                return models;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Batch> BatchQuery(string query)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            ObservableCollection<Batch> batches = new ObservableCollection<Batch>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(query, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Batch b = new Batch();
                        b.Id = Convert.ToInt32(reader["Id"]);
                        b.Nome = Convert.ToString(reader["Nome"]);
                        b.TipoFile = (TipoFileProcessato)Convert.ToInt32(reader["TipoFile"]);
                        b.DirectoryInput = Convert.ToString(reader["DirectoryInput"]);
                        b.DirectoryOutput = Convert.ToString(reader["DirectoryOutput"]);
                        b.IdModello = Convert.ToInt32(reader["IdModello"]);
                        b.NumDoc = Convert.ToInt32(reader["NumDoc"]);
                        b.NumPages = Convert.ToInt32(reader["NumPages"]);
                        b.DocCorrente = Convert.ToInt32(reader["DocCorrente"]);
                        b.UltimoIndicizzato = Convert.ToInt32(reader["UltimoIndicizzato"]);
                        b.PatternNome = Convert.ToString(reader["PatternNome"]);
                        b.UltimoDocumentoEsportato = Convert.ToString(reader["UltimoDocumentoEsportato"]);
                        batches.Add(b);
                    }

                    reader.Close();
                } 
                return batches;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Campo> CampoQuery(string query)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(query, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Campo c = new Campo();
                        c.Id = Convert.ToInt32(reader["Id"]);
                        c.Nome = Convert.ToString(reader["Nome"]);
                        c.Posizione = Convert.ToInt32(reader["Posizione"]);
                        c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                        c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                        c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                        c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                        c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                        c.IdModello = Convert.ToInt32(reader["IdModello"]);
                        c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                        c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
                        campi.Add(c);
                    }

                    reader.Close();
                }    
                return campi;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override ObservableCollection<Modello> ModelloQuery(string query)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            ObservableCollection<Modello> models = new ObservableCollection<Modello>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(query, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Modello m = new Modello();
                        m.Id = Convert.ToInt32(reader["Id"]);
                        m.Nome = Convert.ToString(reader["Nome"]);
                        m.OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]);
                        m.PathFileCsv = Convert.ToString(reader["PathFileCsv"]);
                        m.Separatore = Convert.ToString(reader["Separatore"]);
                        m.StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]);
                        m.CsvColumn = Convert.ToInt32(reader["CsvColumn"]);
                        models.Add(m);
                    }
                    reader.Close();
                }      
                return models;
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override IEnumerable<Modello> IEnumerableModelli()
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            string sql = "SELECT * FROM Modello";
            List<Modello> models = new List<Modello>();
            try
            {
                cnn.Open();
                using (SQLiteCommand myCmd = new SQLiteCommand(sql, cnn))
                {
                    SQLiteDataReader reader = myCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Modello m = new Modello();
                        m.Id = Convert.ToInt32(reader["Id"]);
                        m.Nome = Convert.ToString(reader["Nome"]);
                        m.OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]);
                        m.PathFileCsv = Convert.ToString(reader["PathFileCsv"]);
                        m.Separatore = Convert.ToString(reader["Separatore"]);
                        m.StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]);
                        m.CsvColumn = Convert.ToInt32(reader["CsvColumn"]);
                        models.Add(m);
                    }

                    reader.Close();
                } 
                return models.AsEnumerable();
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public override void DeleteFromTable(string tableName, string condition)
        {          
            try
            {
                Delete(tableName, condition);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
        }

        public override void DropAllRowsFromTable(string tableName)
        {
            try
            {
                Delete(tableName, @"Id > 0");
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
        }

        public override void DeleteReference(string v)
        {
            // ES: DELETE FROM Campi WHERE IdModello = x
            if (string.IsNullOrWhiteSpace(v)) return;
            try
            {
                Delete(v);
            }
            catch (Exception e)
            {
                ErrorCatch(e);
            }
        }
    }
}
