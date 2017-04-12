using BatchDataEntry.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Helpers
{
    public class DatabaseHelperSqlServer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SqlConnection cnn;

        public DatabaseHelperSqlServer(string user, string password, string serverUrl, string databaseName, int timeout = 30)
        {
            cnn = new SqlConnection(string.Format("user id={0};password={1};server={2};Trusted_Connection=yes;database={3};connection timeout={4}", user, password, serverUrl, databaseName, timeout));
        }

        public int Insert(Campo c)
        {
            if (cnn == null || c == null) return -1;
            SqlCommand cmdInsert = new SqlCommand("Campi", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;
            int id = 0;

            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = c.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@Posizione", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@Posizione"].Value = c.Posizione;
            cmdInsert.Parameters.Add(new SqlParameter("@SalvaValori", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@SalvaValori"].Value = c.SalvaValori;
            cmdInsert.Parameters.Add(new SqlParameter("@ValorePredefinito", System.Data.SqlDbType.VarChar,255));
            cmdInsert.Parameters["@ValorePredefinito"].Value = c.ValorePredefinito;
            cmdInsert.Parameters.Add(new SqlParameter("@IndicePrimario", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@IndicePrimario"].Value = c.IndicePrimario;
            cmdInsert.Parameters.Add(new SqlParameter("@TipoCampo", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@TipoCampo"].Value = c.TipoCampo;
            cmdInsert.Parameters.Add(new SqlParameter("@IdModello", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@IdModello"].Value = c.IdModello;
            cmdInsert.Parameters.Add(new SqlParameter("@Riproponi", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@Riproponi"].Value = c.Riproponi;
            cmdInsert.Parameters.Add(new SqlParameter("@Disabilitato", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@Disabilitato"].Value = c.IsDisabled;
            cmdInsert.Parameters.Add(new SqlParameter("@IndiceSecondario", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@IndiceSecondario"].Value = c.IndiceSecondario;

            try
            {
                cnn.Open();
                cmdInsert.ExecuteNonQuery();

                cmdInsert.Parameters.Clear();
                cmdInsert.CommandText = "SELECT SCOPE_IDENTITY()";
                id = Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
            catch(Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return id;
        }

        public int Insert(Modello m)
        {
            if (cnn == null || m == null) return -1;
            SqlCommand cmdInsert = new SqlCommand("Modelli", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;
            int id = 0;
            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = m.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@OrigineCsv", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@OrigineCsv"].Value = m.OrigineCsv;
            cmdInsert.Parameters.Add(new SqlParameter("@PathFileCsv", System.Data.SqlDbType.VarChar));
            cmdInsert.Parameters["@PathFileCsv"].Value = m.PathFileCsv;
            cmdInsert.Parameters.Add(new SqlParameter("@Separatore", System.Data.SqlDbType.VarChar, 1));
            cmdInsert.Parameters["@Separatore"].Value = m.Separatore;
            cmdInsert.Parameters.Add(new SqlParameter("@FocusColumn", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@FocusColumn"].Value = m.StartFocusColumn;
            cmdInsert.Parameters.Add(new SqlParameter("@CsvColumn", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@CsvColumn"].Value = m.CsvColumn;

            try
            {
                cnn.Open();
                cmdInsert.ExecuteNonQuery();

                cmdInsert.Parameters.Clear();
                cmdInsert.CommandText = "SELECT SCOPE_IDENTITY()";
                id = Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return id;
        }

        public int Insert(Batch b)
        {
            if (cnn == null || b == null) return -1;
            SqlCommand cmdInsert = new SqlCommand("Batch", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;
            int id = 0;
            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = b.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@TipoFile", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@TipoFile"].Value = b.TipoFile;
            cmdInsert.Parameters.Add(new SqlParameter("@DirectoryInput", System.Data.SqlDbType.VarChar));
            cmdInsert.Parameters["@DirectoryInput"].Value = b.DirectoryInput;
            cmdInsert.Parameters.Add(new SqlParameter("@DirectoryOutput", System.Data.SqlDbType.VarChar));
            cmdInsert.Parameters["@DirectoryOutput"].Value = b.DirectoryOutput;
            cmdInsert.Parameters.Add(new SqlParameter("@IdModello", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@IdModello"].Value = b.IdModello;
            cmdInsert.Parameters.Add(new SqlParameter("@NumDoc", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@NumDoc"].Value = b.NumDoc;
            cmdInsert.Parameters.Add(new SqlParameter("@NumPages", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@NumPages"].Value = b.NumPages;
            cmdInsert.Parameters.Add(new SqlParameter("@DocCorrente", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@DocCorrente"].Value = b.DocCorrente;
            cmdInsert.Parameters.Add(new SqlParameter("@UltimoIndicizzato", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@UltimoIndicizzato"].Value = b.UltimoIndicizzato;
            cmdInsert.Parameters.Add(new SqlParameter("@PatternNome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@PatternNome"].Value = b.PatternNome;
            cmdInsert.Parameters.Add(new SqlParameter("@UltimoDocumentoEsportato", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@UltimoDocumentoEsportato"].Value = b.UltimoDocumentoEsportato;

            try
            {
                cnn.Open();
                cmdInsert.ExecuteNonQuery();

                cmdInsert.Parameters.Clear();
                cmdInsert.CommandText = "SELECT SCOPE_IDENTITY()";
                id = Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return id;
        }

        public void Update(Batch b)
        {
            if (cnn == null || b == null) return;
            string sqltxt = string.Format("UPDATE Batch(Nome, TipoFile, DirectoryInput, DirectoryOutput, " +
                "IdModello, NumDoc,NumPages,DocCorrente,UltimoIndicizzato,PatternNome,UltimoDocumentoEsportato) " +
                "VALUES (@Nome,@TipoFile,@DirectoryInput,@DirectoryOutput,@IdModello," +
                "@NumDoc,@NumPages,@DocCorrente,@UltimoIndicizzato,@PatternNome,@UltimoDocumentoEsportato) WHERE Id='{0}'", b.Id);
            SqlCommand cmdUpdate = new SqlCommand(sqltxt, this.cnn);

            cmdUpdate.Parameters.AddWithValue("@Nome", b.Nome);
            cmdUpdate.Parameters.AddWithValue("@TipoFile", b.TipoFile);
            cmdUpdate.Parameters.AddWithValue("@DirectoryInput", b.DirectoryInput);
            cmdUpdate.Parameters.AddWithValue("@DirectoryOutput", b.DirectoryOutput);
            cmdUpdate.Parameters.AddWithValue("@IdModello", b.IdModello);
            cmdUpdate.Parameters.AddWithValue("@NumDoc", b.NumDoc);
            cmdUpdate.Parameters.AddWithValue("@NumPages", b.NumPages);
            cmdUpdate.Parameters.AddWithValue("@DocCorrente", b.DocCorrente);
            cmdUpdate.Parameters.AddWithValue("@UltimoIndicizzato", b.UltimoIndicizzato);
            cmdUpdate.Parameters.AddWithValue("@PatternNome", b.PatternNome);
            cmdUpdate.Parameters.AddWithValue("@UltimoDocumentoEsportato", b.UltimoDocumentoEsportato);

            try
            {
                cnn.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
        }

        public void DeleteFromTable(string tableName, string condition)
        {
            if (cnn == null || string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(condition)) return;
            string sql = string.Format("DELETE FROM {0} WHERE {1}", tableName, condition);
            SqlCommand cmdInsert = new SqlCommand(sql, this.cnn);

            try
            {
                cnn.Open();
                cmdInsert.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
        }

        public void Update(Campo c)
        {
            if (cnn == null || c == null) return;
            string sqltxt = string.Format("UPDATE Campi(Nome,Posizione,SalvaValori,ValorePredefinito," +
                "IndicePrimario,TipoCampo,IdModello,Riproponi,Disabilitato,IndiceSecondario) " +
                "VALUES (@Nome,@Posizione,@SalvaValori,@ValorePredefinito,@IndicePrimario," +
                "@TipoCampo,@IdModello,@Riproponi,@Disabilitato,@IndiceSecondario) WHERE Id='{0}'", c.Id);

            SqlCommand cmdUpdate = new SqlCommand(sqltxt, this.cnn);

            cmdUpdate.Parameters.AddWithValue("@Nome", c.Nome);
            cmdUpdate.Parameters.AddWithValue("@Posizione", c.Posizione);
            cmdUpdate.Parameters.AddWithValue("@SalvaValori", c.SalvaValori);
            cmdUpdate.Parameters.AddWithValue("@ValorePredefinito", c.ValorePredefinito);
            cmdUpdate.Parameters.AddWithValue("@IndicePrimario", c.IndicePrimario);
            cmdUpdate.Parameters.AddWithValue("@TipoCampo", c.TipoCampo);
            cmdUpdate.Parameters.AddWithValue("@IdModello", c.IdModello);
            cmdUpdate.Parameters.AddWithValue("@Riproponi", c.Riproponi);
            cmdUpdate.Parameters.AddWithValue("@Disabilitato", c.IsDisabled);
            cmdUpdate.Parameters.AddWithValue("@IndiceSecondario", c.IndiceSecondario);
            
            try
            {
                cnn.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
        }

        public void Update(Modello m)
        {
            if (cnn == null || m == null) return;
            string sqltxt = string.Format("UPDATE Modelli(Nome,OrigineCsv,PathFileCsv,Separatore," +
                "FocusColumn,CsvColumn) " +
                "VALUES (@Nome,@OrigineCsv,@PathFileCsv,@Separatore,@FocusColumn,@CsvColumn) WHERE Id='{0}'", m.Id);

            SqlCommand cmdUpdate = new SqlCommand(sqltxt, this.cnn);

            cmdUpdate.Parameters.AddWithValue("@Nome", m.Nome);
            cmdUpdate.Parameters.AddWithValue("@OrigineCsv", m.OrigineCsv);
            cmdUpdate.Parameters.AddWithValue("@PathFileCsv", m.PathFileCsv);
            cmdUpdate.Parameters.AddWithValue("@Separatore", m.Separatore);
            cmdUpdate.Parameters.AddWithValue("@FocusColumn", m.StartFocusColumn);
            cmdUpdate.Parameters.AddWithValue("@CsvColumn", m.CsvColumn);

            try
            {
                cnn.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
        }
        
        public Batch GetBatchById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Batch WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql ,cnn);

            try
            {
                cnn.Open();
                cmd.ExecuteReader();
                SqlDataReader reader = cmd.ExecuteReader();
                Batch b = new Batch();
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
                return b;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public Campo GetCampoById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Campi WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
                cmd.ExecuteReader();
                SqlDataReader reader = cmd.ExecuteReader();
                Campo c = new Campo();
                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader["Id"]);
                    c.Nome = Convert.ToString(reader["Nome"]);
                    c.Posizione = Convert.ToInt32(reader["Posizione"]);
                    c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                    c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                    c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                    c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                    c.TipoCampo = Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabled = Convert.ToBoolean(reader["Disabilitato"]);
                }
                reader.Close();
                return c;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public Modello GetModelloById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Modelli WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
                cmd.ExecuteReader();
                SqlDataReader reader = cmd.ExecuteReader();
                Modello m = new Modello();
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
                return m;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Batch> GetBatchRecords() {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Batch");

            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Batch> batches = new ObservableCollection<Batch>();
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
                return batches;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Campo> GetCampoRecords() {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Campi");
            try {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
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
                    c.TipoCampo = Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabled = Convert.ToBoolean(reader["Disabilitato"]);
                    campi.Add(c);
                }

                reader.Close();
                return campi;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> GetModelloRecords() {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Modelli");
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Modello> models = new ObservableCollection<Modello>();
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
                return models;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Batch> BatchQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Batch> batches = new ObservableCollection<Batch>();
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
                return batches;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Campo> CampoQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
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
                    c.TipoCampo = Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabled = Convert.ToBoolean(reader["Disabilitato"]);
                    campi.Add(c);
                }

                reader.Close();
                return campi;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public ObservableCollection<Modello> ModelloQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Modello> models = new ObservableCollection<Modello>();
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
                return models;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public IEnumerable<Modello> IEnumerableModelli() {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Modelli");
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Modello> models = new List<Modello>();
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
                return models.AsEnumerable();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return null;
        }

        public int Count(string sql) {
            int result = -1;
            if (cnn == null) return result;
            try {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return result;
        }

        // Implementare autocomp lato server

    }
}
