using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Fork.Logic.Persistence;

namespace Fork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //Migrate
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    string persistencePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "ForkApp", "persistence");
                    var persistenceDir = new DirectoryInfo(persistencePath);
                    if (!persistenceDir.Exists)
                        persistenceDir.Create();
                    var databaseFile = new FileInfo(Path.Combine(persistencePath, "app.db"));
                    if (!databaseFile.Exists)
                        databaseFile.Create().Close();
                    using var context = services.GetService<ApplicationDbContext>();
                    context.Database.Migrate();
                    logger.LogInformation("Finished database migration");
                }
                catch (Exception e)
                {
                    logger.LogError("Error while migrating database! Aborting...\n" + e);
                    return;
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}