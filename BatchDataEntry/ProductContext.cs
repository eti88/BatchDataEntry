using System.Data.Entity;
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

        public virtual DbSet<Models.Batch> Batches { get; set; }
        public virtual DbSet<Models.Campo> Campi { get; set; }
        public virtual DbSet<Models.Modello> Modelli { get; set; }
        public virtual DbSet<Models.FileCSV> File { get; set; }

        public static void ReloadEntity<TEntity>(DbContext context, TEntity entity) where TEntity : class
        {
            context.Entry(entity).Reload();
        }
    }
}
