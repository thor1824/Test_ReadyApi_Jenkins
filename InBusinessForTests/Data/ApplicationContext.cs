using InBusinessForTests.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace InBusinessForTests.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public string DbPath { get; }

        public ApplicationContext(DbContextOptions<ApplicationContext> opt) : base(opt)
        {
            /*var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");*/
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            /*options.UseSqlite($"Data Source={DbPath}");*/
        }
    }
}