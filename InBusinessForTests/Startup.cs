using System;
using InBusinessForTests.Data;
using InBusinessForTests.Data.Managers;
using InBusinessForTests.Data.Managers.Facade;
using InBusinessForTests.Data.Model;
using InBusinessForTests.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace InBusinessForTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" 
                || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
            {
                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();

                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseSqlite(keepAliveConnection).EnableSensitiveDataLogging();
                });
                /*services.AddDbContext<ApplicationContext>(opt =>
                    opt.UseInMemoryDatabase("BusinessDB")
                        .EnableSensitiveDataLogging());*/
                services.AddTransient<TestSeeder>();
            }

            services.AddScoped<ICustomerManager, CustomerManager>();
            services.AddScoped<IProductManager, ProductManager>();
            services.AddScoped<IOrderManager, OrderManager>();

            services.AddScoped<Repository<Customer>>();
            services.AddScoped<Repository<Product>>();
            services.AddScoped<Repository<Order>>();
            services.AddScoped<Repository<Order>>();

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Development") || env.IsEnvironment("Testing"))
            {
                app.UseDeveloperExceptionPage();
                
                
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var dbInitializer = sp.GetService<TestSeeder>();
                    dbInitializer.Seed();
                }
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}