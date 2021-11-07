using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Notification;

namespace ProjectAvery
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
            var builder = new SqliteConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
            builder.DataSource = builder.DataSource.Replace("|datadirectory|",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avery", "persistence"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlite(builder.ToString()));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseLazyLoadingProxies().UseSqlite(builder.ToString()).UseLoggerFactory(MyLoggerFactory).EnableSensitiveDataLogging());
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllers();
            services.AddSingleton<INotificationCenter, DefaultNotificationCenter>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ProjectAvery", Version = "v1"});
            });
            services.Configure<RouteOptions>(o => o.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectAvery v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}