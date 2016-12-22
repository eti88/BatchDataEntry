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
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Campo> Campi { get; set; }
        public DbSet<Modello> Modelli { get; set; }
    }
}
