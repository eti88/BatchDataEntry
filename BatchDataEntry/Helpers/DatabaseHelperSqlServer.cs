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

        public void Insert(Campo c)
        {
            if (cnn == null || c == null) return;
            SqlCommand cmdInsert = new SqlCommand("Campi", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;

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
            }catch(Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }  
        }

        public void Insert(Modello m)
        {
            if (cnn == null || m == null) return;
            SqlCommand cmdInsert = new SqlCommand("Modelli", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;

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

        public void Insert(Batch b)
        {
            if (cnn == null || b == null) return;
            SqlCommand cmdInsert = new SqlCommand("Batch", this.cnn);
            cmdInsert.CommandType = System.Data.CommandType.StoredProcedure;

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
            throw new NotImplementedException();
        }
        public Campo GetCampoById(int id) { throw new NotImplementedException(); }
        public Modello GetModelloById(int id) { throw new NotImplementedException(); }
        public ObservableCollection<Batch> GetBatchRecords() { throw new NotImplementedException(); }
        public ObservableCollection<Campo> GetCampoRecords() { throw new NotImplementedException(); }
        public ObservableCollection<Modello> GetModelloRecords() { throw new NotImplementedException(); }
        public ObservableCollection<Batch> BatchQuery(string query) { throw new NotImplementedException(); }
        public ObservableCollection<Campo> CampoQuery(string query) { throw new NotImplementedException(); }
        public ObservableCollection<Modello> ModelloQuery(string query) { throw new NotImplementedException(); }
        public IEnumerable<Modello> IEnumerableModelli() { throw new NotImplementedException(); }
        public int Count(string sql) { throw new NotImplementedException(); }
        // return Convert.ToInt32(ExecuteScalar("SELECT MAX(Id) from Autocompletamento")); 
        // negli insert

        // comandi liberi come count e altro vedi databasehelper

        // Implementare autocomp lato server

    }
}
