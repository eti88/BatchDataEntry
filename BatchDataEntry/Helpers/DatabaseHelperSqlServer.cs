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
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = serverUrl;
                builder.UserID = user;
                builder.Password = password;
                builder.InitialCatalog = databaseName;
                builder.ConnectTimeout = timeout;
                builder.MultipleActiveResultSets = true;
                cnn = new SqlConnection(builder.ConnectionString);
            }
            catch(Exception e)
            {
                logger.Error(e);
            }
        }

        public static bool IsServerConnected(SqlConnectionStringBuilder bui)
        {
            using (SqlConnection connection = new SqlConnection(bui.ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException e)
                {
                    logger.Error(e);
                    return false;
                }catch(Exception e)
                {
                    logger.Error(e);
                    return false;
                }
            }
        }

        public override int Insert(Campo c)
        {
            if (cnn == null || c == null) return -1;
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Campo(Nome, Posizione, SalvaValori, ValorePredefinito, IndicePrimario, TipoCampo, IdModello, Riproponi,Disabilitato, IndiceSecondario, SourceTableColumn) VALUES (@Nome, @Posizione, @SalvaValori, @ValorePredefinito, @IndicePrimario, @TipoCampo, @IdModello, @Riproponi, @Disabilitato, @IndiceSecondario, @SourceTableColumn)", this.cnn)
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
            cmdInsert.Parameters["@SourceTable"].Value = c.TabellaSorgente;          
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
            cmdInsert.Parameters.Add(new SqlParameter("@SourceTableColumn", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@SourceTableColumn"].Value = c.SourceTableColumn;

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
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Modello(Nome, OrigineCsv, PathFileCsv, Separatore, FocusColumn, CsvColumn) VALUES (@Nome, @OrigineCsv, @PathFileCsv, @Separatore, @FocusColumn, @CsvColumn)", this.cnn)
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

        public int Insert(Concatenation concat)
        {
            if (cnn == null || concat == null) return -1;
            SqlCommand cmdInsert = new SqlCommand(@"INSERT INTO Concatenazioni(Nome, Modello, Campi) VALUES (@Nome, @Modello, @Campi)", this.cnn)
            {
                CommandType = System.Data.CommandType.Text
            };
            int id = 0;

            cmdInsert.Parameters.Add(new SqlParameter("@Nome", System.Data.SqlDbType.VarChar, 255));
            cmdInsert.Parameters["@Nome"].Value = concat.Nome;
            cmdInsert.Parameters.Add(new SqlParameter("@Modello", System.Data.SqlDbType.Int));
            cmdInsert.Parameters["@Modello"].Value = concat.Modello;
            cmdInsert.Parameters.Add(new SqlParameter("@Campi", System.Data.SqlDbType.Text));
            cmdInsert.Parameters["@Campi"].Value = concat.SerializeDictionary();

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
            try
            {
                cnn.Open();
                using (SqlCommand cmdUpdate = new SqlCommand(sqlTxt.ToString(), this.cnn))
                {
                    cmdUpdate.ExecuteNonQuery();
                }
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

        public override void DeleteReference(string query)
        {
            if (cnn == null || string.IsNullOrWhiteSpace(query)) return;
            try
            {
                cnn.Open();
                using(SqlCommand cmdInsert = new SqlCommand(query, this.cnn))
                {
                    cmdInsert.ExecuteNonQuery();
                }
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
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("Nome", c.Nome);
            values.Add("Posizione", c.Posizione.ToString());
            values.Add("SalvaValori", Convert.ToInt32(c.SalvaValori).ToString());
            values.Add("ValorePredefinito", c.ValorePredefinito);
            values.Add("IndicePrimario", Convert.ToInt32(c.IndicePrimario).ToString());
            values.Add("TipoCampo", ((int)c.TipoCampo).ToString());
            values.Add("IdModello", c.IdModello.ToString());
            values.Add("Riproponi", Convert.ToInt32(c.Riproponi).ToString());
            values.Add("Disabilitato", Convert.ToInt32(c.IsDisabilitato).ToString());
            values.Add("IndiceSecondario", Convert.ToInt32(c.IndiceSecondario).ToString());
            values.Add("SourceTable", c.TabellaSorgente);
            values.Add("SourceTableColumn", Convert.ToInt32(c.SourceTableColumn).ToString());
            UpdateRaw("Campo", values, string.Format("Id={0}", c.Id));
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
            UpdateRaw("Modello", values, string.Format("Id={0}", m.Id));
        }
        
        public void Update(Concatenation concat)
        {
            if (cnn == null || concat == null) return;
            if (concat.Id == 0) throw new Exception("Non è possibile eseguire il comando update su un record con Id=0");
            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "Nome", concat.Nome },
                { "Modello", concat.Modello.ToString() },
                { "Campi", concat.SerializeDictionary() }
            };
            UpdateRaw("Concatenazioni", values, string.Format("Id={0}", concat.Id));
        }

        public override Batch GetBatchById(int id) {
            if (cnn == null) return null;
            string sql = "SELECT * FROM Batch WHERE Id = @id";
            Batch b = new Batch();

            try
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
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
                } 
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
            Campo c = new Campo();
            if (cnn == null) return c;
            string sql = string.Format("SELECT * FROM Campo WHERE Id = {0}", id);

            SqlCommand cmd = new SqlCommand(sql, cnn);

            try
            {
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader["Id"]);
                    c.Nome = Convert.ToString(reader["Nome"]);
                    c.Posizione = Convert.ToInt32(reader["Posizione"]);
                    c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                    c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                    c.TabellaSorgente = Convert.ToString(reader["SourceTable"]);
                    c.SourceTableColumn = Convert.ToInt32(reader["SourceTableColumn"]);
                    c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                    c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                    c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return c;
        }

        public override Modello GetModelloById(int id) {
            if (cnn == null) return null;
            string sql = string.Format("SELECT * FROM Modello WHERE Id = {0}", id);

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
            string sql = string.Format("SELECT * FROM Campo");
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
                        TabellaSorgente = Convert.ToString(reader["SourceTable"]),
                        SourceTableColumn = Convert.ToInt32(reader["SourceTableColumn"]),
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
            string sql = string.Format("SELECT * FROM Modello");
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
                    Campo c = new Campo();
                    c.Id = Convert.ToInt32(reader["Id"]);
                    c.Nome = Convert.ToString(reader["Nome"]);
                    c.Posizione = Convert.ToInt32(reader["Posizione"]);
                    c.SalvaValori = Convert.ToBoolean(reader["SalvaValori"]);
                    c.ValorePredefinito = Convert.ToString(reader["ValorePredefinito"]);
                    c.TabellaSorgente = Convert.ToString(reader["SourceTable"]);
                    c.SourceTableColumn = Convert.ToInt32(reader["SourceTableColumn"]);
                    c.IndicePrimario = Convert.ToBoolean(reader["IndicePrimario"]);
                    c.IndiceSecondario = Convert.ToBoolean(reader["IndiceSecondario"]);
                    c.TipoCampo = (EnumTypeOfCampo)Convert.ToInt32(reader["TipoCampo"]);
                    c.IdModello = Convert.ToInt32(reader["IdModello"]);
                    c.Riproponi = Convert.ToBoolean(reader["Riproponi"]);
                    c.IsDisabilitato = Convert.ToBoolean(reader["Disabilitato"]);
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
            string sql = string.Format("SELECT * FROM Modello");
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
            }catch(System.Data.SqlClient.SqlException e)
            {
                logger.Warn(e.ToString());
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
            string sql = string.Format("SELECT TOP 1 * FROM Modello");
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
                string sql = string.Format("SELECT TOP 1 * FROM Campo");
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
                    c.TabellaSorgente = Convert.ToString(reader["SourceTable"]);
                    c.SourceTableColumn = Convert.ToInt32(reader["SourceTableColumn"]);
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
            List<string> res = new List<string>();
            if (cnn == null) return res;

            try
            {
                cnn.Open();
                var dt = cnn.GetSchema(@"Tables");
                foreach (DataRow row in dt.Rows)
                {
                    string tabName = row[2] as string;
                    res.Add(tabName);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return res;
        }

        public List<string> GetRecord(string table, Dictionary<string, int> columns, int selectedCol, string val)
        {
            var rec = new List<string>();
            if (cnn == null) return rec;

            try
            {
                cnn.Open();
                string sql = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", 
                    table , 
                    columns.FirstOrDefault(x => x.Value == selectedCol).Key, 
                    val);
                SqlCommand cmd = new SqlCommand(sql, cnn);
                var reader = cmd.ExecuteReader();
                reader.Read();

                foreach(KeyValuePair<string, int> k in columns)
                {
                    rec.Add(reader[k.Value].ToString());
                }
                reader.Close();
            }catch(Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return rec;
        }

        public List<string> GetRecord(string table, Dictionary<string, int> columns, List<int> selectedCols, List<string> vals)
        {
            var rec = new List<string>();
            if (cnn == null) return rec;

            try
            {
                cnn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM ");
                sb.Append(table);
                sb.Append(" WHERE ");
                int z = 0;

                foreach(KeyValuePair<string, int> col in columns)
                {
                    if (selectedCols.Contains(col.Value))
                    {
                        sb.Append(col.Key + " = '" + vals.ElementAt(selectedCols.IndexOf(col.Value)) + "'");
                        z++;
                        if (z <= selectedCols.Count() - 1)
                        {
                            sb.Append(" AND ");
                        }
                    }

                    
                }

                SqlCommand cmd = new SqlCommand(sb.ToString(), cnn);
                var reader = cmd.ExecuteReader();
                reader.Read();

                foreach (KeyValuePair<string, int> k in columns)
                {
                    rec.Add(reader[k.Value].ToString());
                }
                reader.Close();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return rec;
        }

        public Dictionary<string, int> GetColumns(string table)
        {
            Dictionary<string, int> dres = new Dictionary<string, int>();
            if (cnn == null) return dres;
            if (string.IsNullOrEmpty(table)) return dres;
            
            string query = string.Format("select column_name from information_schema.columns where table_name = '{0}'", table);

            try
            {
                cnn.Open();
                //DataTable schemaTables = cnn.GetSchema();

                SqlCommand cmd = new SqlCommand(query, cnn);
                int i = 0;
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    dres.Add(Convert.ToString(reader["column_name"]), i);
                    i++;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }
            return dres;
        }

        /// <summary>
        /// Nel caso si cerchi di recuperare l'autocompletamento dalla località utilizza un suggestion apposito
        /// (per poter visualizzare correttamente le informazioni).
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnTable"></param>
        /// <returns></returns>
        public List<AbsSuggestion> GetAutocompleteList(string tableName, int columnTable, EnumTypeOfCampo type)
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
                {
                    if (type == EnumTypeOfCampo.AutocompletamentoLocalita)
                    {
                        suggestions.Add(new SuggestionLocalita(Convert.ToString(reader[1]), Convert.ToString(reader[3]), Convert.ToString(reader[2])));
                    }
                    else
                    {
                        suggestions.Add(new SuggestionSingleColumn(Convert.ToString(reader[columnTable])));
                    }
                }
                reader.Close();
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

        public ObservableCollection<Concatenation> LoadConcatenations(int modello)
        {
            if (cnn == null) return null;
            var res = new ObservableCollection<Concatenation>();
            string sql = string.Format("SELECT * FROM Concatenazioni WHERE Modello = {0}", modello);
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var tmp = new Concatenation();
                    tmp.Id = Convert.ToInt32(reader["Id"]);
                    tmp.Nome = Convert.ToString(reader["Nome"]);
                    tmp.Modello = modello;
                    string serializedDict = Convert.ToString(reader["Campi"]);
                    tmp.DeserializeDictionary(serializedDict);

                    res.Add(tmp);
                }
                reader.Close();
            }
            catch(Exception e)
            {
                logger.Error(e.ToString());
            }
            finally
            {
                cnn.Close();
            }

            return res;
        }
    }
}
