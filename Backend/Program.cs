using System;
using System.IO;
using Fork.Logic.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fork;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();

        //Migrate
        using (IServiceScope scope = host.Services.CreateScope())
        {
            IServiceProvider services = scope.ServiceProvider;
            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                string persistencePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ForkApp", "persistence");
                DirectoryInfo persistenceDir = new DirectoryInfo(persistencePath);
                if (!persistenceDir.Exists)
                {
                    persistenceDir.Create();
                }

                FileInfo databaseFile = new FileInfo(Path.Combine(persistencePath, "app.db"));
                if (!databaseFile.Exists)
                {
                    databaseFile.Create().Close();
                }

                using ApplicationDbContext context = services.GetService<ApplicationDbContext>();
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

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}