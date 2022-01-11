using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InBusinessForTests.Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InBusinessForTests.Data
{
    public class TestSeeder
    {
        private readonly ApplicationContext _dbContext;

        public TestSeeder(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed()
        {
            await _dbContext.Database.MigrateAsync();
            if (!_dbContext.Set<Product>().Any())
            {
                try
                {
                    if (!_dbContext.Database.IsSqlite())
                    {
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} ON", "Customers");
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} ON", "Products");
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} ON", "Orders");
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }

                var TestCustomer1 = 1;
                var TestCustomer2 = 2;
                _dbContext.Set<Customer>().AddRange(new Customer
                {
                    Id = TestCustomer1,
                    Name = "TestCustomerNoCredit",
                    Credit = 0,
                }, new Customer
                {
                    Id = TestCustomer2,
                    Name = "TestCustomerMinusCredit",
                    Credit = -10,
                });
                var TestProduct1 = 1;
                var TestProduct2 = 2;
                var TestProduct3 = 3;
                var TestProduct4 = 4;
                _dbContext.Set<Product>().AddRange(
                    new Product
                    {
                        Id = TestProduct1,
                        Price = 1,
                        Name = "Product1",
                        Reserved = 0,
                        Stock = 10,
                        DiscountInPercentage = 0,
                        AmountBoughtForDiscount = 0
                    },
                    new Product
                    {
                        Id = TestProduct2,
                        Price = 10,
                        Name = "Product2",
                        Reserved = 0,
                        Stock = 10,
                        DiscountInPercentage = 0,
                        AmountBoughtForDiscount = 0
                    },
                    new Product
                    {
                        Id = TestProduct3,
                        Price = 499,
                        Name = "Product3",
                        Reserved = 0,
                        Stock = 0,
                        DiscountInPercentage = 0,
                        AmountBoughtForDiscount = 0
                    },
                    new Product
                    {
                        Id = TestProduct4,
                        Price = 500,
                        Name = "Product4",
                        Reserved = 0,
                        Stock = 0,
                        DiscountInPercentage = 0,
                        AmountBoughtForDiscount = 0
                    }
                );

                var TestOrder1 = 1;
                _dbContext.Set<Order>().AddRange(
                    new Order
                    {
                        Id = TestOrder1,
                        CustomerId = TestCustomer2,
                        OrderLines = new List<OrderLine>
                        {
                            new OrderLine
                            {
                                ProductId = TestProduct2,
                                StockReserved = 1,
                                DiscountAtTimeOfPurchase = 0.0,
                                PriceAtTimeOfPurchase = 10,
                                DiscountAmountAtTimeOfPurchase = 0,
                            }
                        },
                        TimeOfPurchase = DateTime.UtcNow
                    }
                );
                await _dbContext.SaveChangesAsync();
                try
                {
                    if (!_dbContext.Database.IsSqlite())
                    {
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} OFF", "Customer");
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} OFF", "Product");
                        await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT {0} OFF", "Order");
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }
    }
}