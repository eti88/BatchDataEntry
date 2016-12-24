using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using BatchDataEntry.Models;
using NLog;

namespace BatchDataEntry.Helpers
{
    /// <summary>
    /// Helper class for entity framework
    /// </summary>
    
    public static class HDatabase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

#region QueryTasks

        /// <summary>
        /// Restituisce tutto il contenuto della tabella Batch
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Batch>> GetBatchesTask()
        {
            List<Batch> result = new List<Batch>();
            try
            {
                using (var context = new ProductContext())
                {
                    var query = from c in context.Batches orderby c.Id select c;
                    result = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetBatchesTask] " + e.Message);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Restituisce la lista di dei Campi appartenenti ad un determinato Modello
        /// </summary>
        /// <param name="idModello">Id del modello</param>
        /// <returns>Lista dei campi sotto forma di List</returns>
        public static async Task<List<Campo>> GetCampiTask(long idModello)
        {
            List<Campo> result = new List<Campo>();

            try
            {
                using (var context = new ProductContext())
                {
                    result = await (context.Campi.Where(x => x.FKModello == idModello).ToListAsync());
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetCampiTask] " + e.Message);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Restituisce la lista dei Modelli presenti nella tabella
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Modello>> GetModelliTask()
        {
            List<Modello> result = new List<Modello>();

            try
            {
                using (var context = new ProductContext())
                {
                    var query = from c in context.Modelli orderby c.Id select c;
                    result = await query.ToListAsync();
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetModelliTas] " + e.Message);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Restituisce il file csv associato
        /// </summary>
        /// <param name="idFile"></param>
        /// <returns></returns>
        public static async Task<FileCSV> GetOrigineDatiTask(long idFile)
        {
            FileCSV result = new FileCSV();

            try
            {
                using (var context = new ProductContext())
                {
                    result = await (context.File.Where(x => x.Id == idFile).FirstOrDefaultAsync());
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetOrigineDatiTask] " + e.Message);
                result = null;
            }
            return result;
        }

        public static async Task<Modello> GetModelloTask(long idModello)
        {
            Modello result = new Modello();

            try
            {
                using (var context = new ProductContext())
                {
                    result = await (context.Modelli.Where(x => x.Id == idModello).FirstOrDefaultAsync());
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetModelloTask] " + e.Message);
                result = null;
            }

            return result;
        }

        public static async Task<Batch> GetBatchTask(long idBatch)
        {
            Batch result = new Batch();

            try
            {
                using (var context = new ProductContext())
                {
                    result = await (context.Batches.Where(x => x.Id == idBatch).FirstOrDefaultAsync());
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetBatchTask] " + e.Message);
                result = null;
            }

            return result;
        }


        public static async Task<Campo> GetCampoTask(long idCampo)
        {
            Campo result = new Campo();

            try
            {
                using (var context = new ProductContext())
                {
                    result = await (context.Campi.Where(x => x.Id == idCampo).FirstOrDefaultAsync());
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:GetCampoTask] " + e.Message);
                result = null;
            }

            return result;
        }

        #endregion

        #region InsertTasks

        public static async Task<bool> InsertBatchTask(Batch b)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    context.Batches.Add(b);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:InsertBatchTask] " + e.Message);
            }

            return result;
        }

        public static async Task<bool> InsertCampoTask(Campo c)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    context.Campi.Add(c);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:InsertCampoTask] " + e.Message);
            }

            return result;
        }


        public static async Task<bool> InsertModelloTask(Modello m)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    context.Modelli.Add(m);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:InsertModelloTask] " + e.Message);
            }

            return result;
        }

        public static async Task<bool> InsertOrigineCsvTask(FileCSV f)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    context.File.Add(f);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:InsertOrigineCsvTask] " + e.Message);
            }

            return result;
        }

#endregion

        #region UpdateTasks

        public static async Task<bool> UpdateBatchTask(Batch b)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Batch oldOne = await context.Batches.FindAsync(b.Id);
                    // Iterazione sulle propietà
                    PropertyInfo[] properties = typeof(Batch).GetProperties();
                    foreach (PropertyInfo p in properties)
                    {
                        object first = p.GetValue(oldOne, null);
                        object second = p.GetValue(b, null);
                        // se le due propietà sono differenti allora le aggiorna
                        if (!object.Equals(first, second))
                        {
                            p.SetValue(first, second);
                        }
                    }
                  
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:UpdateBatchTask] " + e.Message);
            }

            return result;
        }


        public static async Task<bool> UpdateCampoTask(Campo c)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Batch oldOne = await context.Batches.FindAsync(c.Id);
                    // Iterazione sulle propietà
                    PropertyInfo[] properties = typeof(Campo).GetProperties();
                    foreach (PropertyInfo p in properties)
                    {
                        object first = p.GetValue(oldOne, null);
                        object second = p.GetValue(c, null);
                        // se le due propietà sono differenti allora le aggiorna
                        if (!object.Equals(first, second))
                        {
                            p.SetValue(first, second);
                        }
                    }

                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:UpdateCampoTask] " + e.Message);
            }

            return result;
        }
        
        public static async Task<bool> UpdateModelloTask(Modello m)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Batch oldOne = await context.Batches.FindAsync(m.Id);
                    // Iterazione sulle propietà
                    PropertyInfo[] properties = typeof(Modello).GetProperties();
                    foreach (PropertyInfo p in properties)
                    {
                        object first = p.GetValue(oldOne, null);
                        object second = p.GetValue(m, null);
                        // se le due propietà sono differenti allora le aggiorna
                        if (!object.Equals(first, second))
                        {
                            p.SetValue(first, second);
                        }
                    }

                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:UpdateModelloTask] " + e.Message);
            }

            return result;
        }
        
        public static async Task<bool> UpdateOrigineDatiTask(FileCSV f)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Batch oldOne = await context.Batches.FindAsync(f.Id);
                    // Iterazione sulle propietà
                    PropertyInfo[] properties = typeof(FileCSV).GetProperties();
                    foreach (PropertyInfo p in properties)
                    {
                        object first = p.GetValue(oldOne, null);
                        object second = p.GetValue(f, null);
                        // se le due propietà sono differenti allora le aggiorna
                        if (!object.Equals(first, second))
                        {
                            p.SetValue(first, second);
                        }
                    }

                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:UpdateOrigineDatiTask] " + e.Message);
            }

            return result;
        }

        #endregion

        #region DeleteTasks

        public static async Task<bool> DeleteBatchTask(long id)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Batch existing = await context.Batches.FindAsync(id);
                    context.Batches.Remove(existing);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:DeleteBatchTask] " + e.Message);
            }

            return result;
        }

        public static async Task<bool> DeleteCampoTask(long id)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Campo existing = await context.Campi.FindAsync(id);
                    context.Campi.Remove(existing);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:DeleteCampoTask] " + e.Message);
            }

            return result;
        }
        
        public static async Task<bool> DeleteModelloTask(long id)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    Modello existing = await context.Modelli.FindAsync(id);
                    context.Modelli.Remove(existing);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:DeleteModelloTask] " + e.Message);
            }

            return result;
        }
        
        public static async Task<bool> DeleteOrigineDatiTask(long id)
        {
            bool result = false;

            try
            {
                using (var context = new ProductContext())
                {
                    FileCSV existing = await context.File.FindAsync(id);
                    context.File.Remove(existing);
                    await context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("[HDatabase:DeleteOrigineDatiTask] " + e.Message);
            }

            return result;
        }

        #endregion

    }
}
