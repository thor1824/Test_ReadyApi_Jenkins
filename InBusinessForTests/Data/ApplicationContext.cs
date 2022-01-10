using System;
using System.Collections.Generic;
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

    public class Entity
    {
        public int Id { get; set; }
    }

    public class Customer: Entity
    {
        public string Name { get; set; }
        public double Credit { get; set; }
        public List<Order> Orders { get; } = new List<Order>();
    }

    public class Product: Entity
    {
        public string Name { get; set; } = "N/A";
        public int Stock { get; set; } 
        public int Reserved { get; set; }
        public double Price { get; set; } = 100.0;
        public bool HasDiscount => DiscountInPercentage != 0.0;
        public double DiscountInPercentage { get; set; } = 0.0;
        public int AmountBoughtForDiscount { get; set; } = 0;
    }

    public class Order: Entity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime TimeOfPurchase { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }

    public class OrderLine: Entity
    {
        public int StockReserved { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public int DiscountAmountAtTimeOfPurchase { get; set; }
        public bool HadDiscount => DiscountAtTimeOfPurchase != 0.0;

        public double DiscountAtTimeOfPurchase { get; set; }
        public double PriceAtTimeOfPurchase { get; set; }
    }
}