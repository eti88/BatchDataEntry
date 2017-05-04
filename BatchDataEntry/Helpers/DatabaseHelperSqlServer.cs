using BatchDataEntry.Abstracts;
using BatchDataEntry.Interfaces;
using BatchDataEntry.Models;
using BatchDataEntry.Suggestions;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BatchDataEntry.Helpers
{
    /*
     La tabella campi è contiene un campo addizionale: "SourceTable" per recuperare dal database mssql le sorgenti di dati per alcuni autocompletamenti.
     */
    public class DatabaseHelperSqlServer : AbsDbHelper, IMsql
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SqlConnection cnn;

        public DatabaseHelperSqlServer(string user, string password, string serverUrl, string databaseName, int timeout = 30)
        {
            cnn = new SqlConnection(string.Format("user id={0};password={1};server={2};Trusted_Connection=yes;database={3};connection timeout={4};MultipleActiveResultSets=True", user, password, serverUrl, databaseName, timeout));
        }

        public static bool IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public override int Insert(Campo c)
        {
            if (cnn == null || c == null) return -1;
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Campi(Nome, Posizione, SalvaValori, ValorePredefinito, IndicePrimario, TipoCampo, IdModello, Riproponi,Disabilitato, IndiceSecondario) VALUES (@Nome, @Posizione, @SalvaValori, @ValorePredefinito, @IndicePrimario, @TipoCampo, @IdModello, @Riproponi, @Disabilitato, @IndiceSecondario)", this.cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
            int id = 0;

            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = c.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@Posizione", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@Posizione"].Value = c.Posizione;
            cmdInsert.Parameters.Add(new SqlParameter("@SalvaValori", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@SalvaValori"].Value = c.SalvaValori;
            cmdInsert.Parameters.Add(new SqlParameter("@ValorePredefinito", System.Data.SqlDbType.VarChar,255));
            cmdInsert.Parameters["@ValorePredefinito"].Value = c.ValorePredefinito;
            cmdInsert.Parameters.Add(new SqlParameter("@SourceTable", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@SourceTable"].Value = c.SourceTableAutocomplete;
            cmdInsert.Parameters.Add(new SqlParameter("@IndicePrimario", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@IndicePrimario"].Value = c.IndicePrimario;
            cmdInsert.Parameters.Add(new SqlParameter("@TipoCampo", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@TipoCampo"].Value = c.TipoCampo;
            cmdInsert.Parameters.Add(new SqlParameter("@IdModello", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@IdModello"].Value = c.IdModello;
            cmdInsert.Parameters.Add(new SqlParameter("@Riproponi", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@Riproponi"].Value = c.Riproponi;
            cmdInsert.Parameters.Add(new SqlParameter("@Disabilitato", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@Disabilitato"].Value = c.IsDisabilitato;
            cmdInsert.Parameters.Add(new SqlParameter("@IndiceSecondario", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@IndiceSecondario"].Value = c.IndiceSecondario;

            try
            {
                cnn.Open();
                cmdInsert.ExecuteNonQuery();

                cmdInsert.Parameters.Clear();
                cmdInsert.CommandText = "Select @@Identity";
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

        public override int Insert(Modello m)
        {
            if (cnn == null || m == null) return -1;
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Modelli(Nome, OrigineCsv, PathFileCsv, Separatore, FocusColumn, CsvColumn) VALUES (@Nome, @OrigineCsv, @PathFileCsv, @Separatore, @FocusColumn, @CsvColumn)", this.cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
            int id = 0;
            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = m.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@OrigineCsv", System.Data.SqlDbType.Bit));
            cmdInsert.Parameters["@OrigineCsv"].Value = (m.OrigineCsv) ? 1 : 0;
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
                cmdInsert.CommandText = "Select @@Identity";
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

        public override int Insert(Batch b)
        {
            if (cnn == null || b == null) return -1;
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Batch(Nome, TipoFile, DirectoryInput, DirectoryOutput, IdModello, DocCorrente, UltimoIndicizzato, PatternNome, UltimoDocumentoEsportato) VALUES (@Nome, @TipoFile, @DirectoryInput, @DirectoryOutput, @IdModello, @DocCorrente, @UltimoIndicizzato, @PatternNome, @UltimoDocumentoEsportato)", this.cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
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
                cmdInsert.CommandText = "Select @@Identity";
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

        private void UpdateRaw(string table, Dictionary<string, string> values, string where) {
            StringBuilder frmtVals = new StringBuilder();
            if(values.Count >= 1)
            {
                foreach(KeyValuePair<string, string> val in values)
                {
                    if (string.IsNullOrWhiteSpace(val.Value))
                        frmtVals.AppendFormat(" {0} = '{1}',", DatabaseHelper.convertQuotes(val.Key.ToString()), string.Empty);
                    else
                        frmtVals.AppendFormat(" {0} = '{1}',", DatabaseHelper.convertQuotes(val.Key.ToString()), DatabaseHelper.convertQuotes(val.Value.ToString()));
                }
                frmtVals.Length--; // rimuovo l'ultima virgola (in eccesso)
            }
            StringBuilder sqlTxt = new StringBuilder();
            sqlTxt.AppendFormat("UPDATE {0} SET {1} WHERE {2};", table, frmtVals.ToString(), where);

            SqlCommand cmdUpdate = new SqlCommand(sqlTxt.ToString(), this.cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
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

        public override void Update(Batch b)
        {
            if (cnn == null || b == null) return;
            if (b.Id == 0) throw new Exception("Non è possibile eseguire il comando update su un record con Id=0");
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "Nome", b.Nome },
                { "TipoFile", string.Format("{0}", (int)b.TipoFile) },
                { "DirectoryInput", b.DirectoryInput },
                { "DirectoryOutput", b.DirectoryOutput },
                { "IdModello", b.IdModello.ToString() },
                { "DocCorrente", b.DocCorrente.ToString() },
                { "UltimoIndicizzato", b.UltimoIndicizzato.ToString() },
                { "PatternNome", b.PatternNome },
                { "UltimoDocumentoEsportato", b.UltimoDocumentoEsportato }
            };
            UpdateRaw("Batch", values, string.Format("Id={0}", b.Id));
        }

        public override void DeleteFromTable(string tableName, string condition)
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

        public void DeleteReference(string query)
        {
            if (cnn == null || string.IsNullOrWhiteSpace(query)) return;
            SqlCommand cmdInsert = new SqlCommand(query, this.cnn);

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

        public override void Update(Campo c)
        {
            if (cnn == null || c == null) return;
            if (c.Id == 0) throw new Exception("Non è possibile eseguire il comando update su un record con Id=0");
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "Nome", c.Nome },
                { "Posizione", c.Posizione.ToString() },
                { "SalvaValori", Convert.ToInt32(c.SalvaValori).ToString() },
                { "ValorePredefinito", c.ValorePredefinito },
                { "SourceTable", c.SourceTableAutocomplete },
                { "IndicePrimario", Convert.ToInt32(c.IndicePrimario).ToString() },
                { "TipoCampo", ((int)c.TipoCampo).ToString() },
                { "IdModello", c.IdModello.ToString() },
                { "Riproponi", Convert.ToInt32(c.Riproponi).ToString() },
                { "Disabilitato", Convert.ToInt32(c.IsDisabilitato).ToString() },
                { "IndiceSecondario", Convert.ToInt32(c.IndiceSecondario).ToString() }
            };
            UpdateRaw("Campi", values, string.Format("Id={0}", c.Id));
        }

        public override void Update(Modello m)
        {
            if (cnn == null || m == null) return;
            if (m.Id == 0) throw new Exception("Non è possibile eseguire il comando update su un record con Id=0");
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "Nome", m.Nome },
                { "OrigineCsv", Convert.ToInt32(m.OrigineCsv).ToString() },
                { "PathFileCsv", m.PathFileCsv },
                { "Separatore", m.Separatore },
                { "FocusColumn", m.StartFocusColumn.ToString() },
                { "CsvColumn", m.CsvColumn.ToString() }
            };
            UpdateRaw("Modelli", values, string.Format("Id={0}", m.Id));
        }
        
        public override Batch GetBatchById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Batch WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
            try
            {
                cnn.Open();
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

        public override Campo GetCampoById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Campi WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                Campo c = new Campo();
                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader["Id"]);
                    c.Nome = Convert.ToString(reader["Nome"]);
                    c.Posizione = Convert.ToInt32(reader["Posizione"]);
                    c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                    c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                    c.SourceTableAutocomplete = Convert.ToString(reader["SourceTable"]);
                    c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                    c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                    c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
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

        public override Modello GetModelloById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Modelli WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
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

        public override ObservableCollection<Batch> GetBatchRecords() {
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
                    Batch b = new Batch()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        TipoFile = (TipoFileProcessato)Convert.ToInt32(reader["TipoFile"]),
                        DirectoryInput = Convert.ToString(reader["DirectoryInput"]),
                        DirectoryOutput = Convert.ToString(reader["DirectoryOutput"]),
                        IdModello = Convert.ToInt32(reader["IdModello"]),
                        DocCorrente = Convert.ToInt32(reader["DocCorrente"]),
                        UltimoIndicizzato = Convert.ToInt32(reader["UltimoIndicizzato"]),
                        PatternNome = Convert.ToString(reader["PatternNome"]),
                        UltimoDocumentoEsportato = Convert.ToString(reader["UltimoDocumentoEsportato"])
                    };
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

        public override ObservableCollection<Campo> GetCampoRecords() {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Campi");
            try {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
                while (reader.Read())
                {
                    Campo c = new Campo()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        Posizione = Convert.ToInt32(reader["Posizione"]),
                        SalvaValori = Convert.ToBoolean(reader["SalvaValori"]),
                        ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]),
                        SourceTableAutocomplete = Convert.ToString(reader["SourceTable"]),
                        IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]),
                        IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]),
                        TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]),
                        IdModello = Convert.ToInt32(reader["IdModello"]),
                        Riproponi = Convert.ToBoolean(reader["Riproponi"]),
                        IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"])
                    };
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

        public override ObservableCollection<Modello> GetModelloRecords() {
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
                    Modello m = new Modello()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]),
                        PathFileCsv = Convert.ToString(reader["PathFileCsv"]),
                        Separatore = Convert.ToString(reader["Separatore"]),
                        StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]),
                        CsvColumn = Convert.ToInt32(reader["CsvColumn"])
                    };
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

        public override ObservableCollection<Batch> BatchQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn)
                {
                    CommandType = System.Data.CommandType.Text
                };
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Batch> batches = new ObservableCollection<Batch>();
                while (reader.Read())
                {
                    Batch b = new Batch()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        TipoFile = (TipoFileProcessato)Convert.ToInt32(reader["TipoFile"]),
                        DirectoryInput = Convert.ToString(reader["DirectoryInput"]),
                        DirectoryOutput = Convert.ToString(reader["DirectoryOutput"]),
                        IdModello = Convert.ToInt32(reader["IdModello"]),
                        DocCorrente = Convert.ToInt32(reader["DocCorrente"]),
                        UltimoIndicizzato = Convert.ToInt32(reader["UltimoIndicizzato"]),
                        PatternNome = Convert.ToString(reader["PatternNome"]),
                        UltimoDocumentoEsportato = Convert.ToString(reader["UltimoDocumentoEsportato"])
                    };
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

        public override ObservableCollection<Campo> CampoQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn)
                {
                    CommandType = System.Data.CommandType.Text
                };
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Campo> campi = new ObservableCollection<Campo>();
                while (reader.Read())
                {
                    Campo c = new Campo()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        Posizione = Convert.ToInt32(reader["Posizione"]),
                        SalvaValori = Convert.ToBoolean(reader["SalvaValori"]),
                        ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]),
                        SourceTableAutocomplete = Convert.ToString(reader["SourceTable"]),
                        IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]),
                        IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]),
                        TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]),
                        IdModello = Convert.ToInt32(reader["IdModello"]),
                        Riproponi = Convert.ToBoolean(reader["Riproponi"]),
                        IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"])
                    };
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

        public override ObservableCollection<Modello> ModelloQuery(string query) {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(query, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                ObservableCollection<Modello> models = new ObservableCollection<Modello>();
                while (reader.Read())
                {
                    Modello m = new Modello()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]),
                        PathFileCsv = Convert.ToString(reader["PathFileCsv"]),
                        Separatore = Convert.ToString(reader["Separatore"]),
                        StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]),
                        CsvColumn = Convert.ToInt32(reader["CsvColumn"])
                    };
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

        public override IEnumerable<Modello> IEnumerableModelli() {
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
                    Modello m = new Modello()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        OrigineCsv = Convert.ToBoolean(reader["OrigineCsv"]),
                        PathFileCsv = Convert.ToString(reader["PathFileCsv"]),
                        Separatore = Convert.ToString(reader["Separatore"]),
                        StartFocusColumn = Convert.ToInt32(reader["FocusColumn"]),
                        CsvColumn = Convert.ToInt32(reader["CsvColumn"])
                    };
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

        public override int Count(string sql) {
            int result = -1;
            if (cnn == null) return result;
            try {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.Text
                };
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

        public override void DropAllRowsFromTable(string tableName)
        {
            if (cnn == null) return;
            string sql = string.Format("DELETE FROM {0}", tableName);
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return;
        }

        public Modello GetFirstModello()
        {
            if (cnn == null) throw new Exception("Connessione al database non inizializzata");
            string sql = string.Format("SELECT TOP 1 * FROM Modelli");
            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
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

        public Batch GetFirstBatch()
        {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                string sql = string.Format("SELECT TOP 1 * FROM Batch");
                SqlCommand cmd = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.Text
                };
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

        public Campo GetFirstCampo()
        {
            if (cnn == null) return null;
            try
            {
                cnn.Open();
                string sql = string.Format("SELECT TOP 1 * FROM Campi");
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                Campo c = new Campo();
                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader["Id"]);
                    c.Nome = Convert.ToString(reader["Nome"]);
                    c.Posizione = Convert.ToInt32(reader["Posizione"]);
                    c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                    c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                    c.SourceTableAutocomplete = Convert.ToString(reader["SourceTable"]);
                    c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                    c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                    c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
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

        // Funzione per ritornare la lista di tabelle presenti nel server
        public List<string> GetTableList()
        {
            if (cnn == null) return null;
            List<string> res = new List<string>();

            try
            {
                cnn.Open();
                var dt = cnn.GetSchema(@"Tables");
                foreach (DataRow row in dt.Rows)
                {
                    string tabName = row[2] as string;
                    res.Add(tabName);
                }
                return res;
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

        public List<AbsSuggestion> GetAutocompleteList(string tableName, int columnTable)
        {
            if (cnn == null) return null;
            var suggestions = new List<AbsSuggestion>();
            string sql = string.Format("SELECT * FROM {0}", tableName);
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    suggestions.Add(new SuggestionSingleColumn(Convert.ToString(reader[columnTable])));
                reader.Close();
                //return suggestions;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return suggestions;
        }
    }
}
