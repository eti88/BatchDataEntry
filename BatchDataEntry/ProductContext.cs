using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Models;

namespace BatchDataEntry
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("name=dbEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Campo> Campi { get; set; }
        public virtual DbSet<Modello> Modelli { get; set; }
        public virtual DbSet<FileCSV> File { get; set; }

        public static void ReloadEntity<TEntity>(DbContext context, TEntity entity) where TEntity : class
        {
            context.Entry(entity).Reload();
        }
    }
}
