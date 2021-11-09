using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectAveryCommon.Model.Application;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.Automation;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Server> ServerSet { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        //TODO CKE check permission before adding stuff
        //TODO CKE fill with actual stuff
        public State GenerateStateObject()
        {
            State result = new State();
            result.Entities = new List<IEntity>();

            ServerVersion dummyVersion = new ServerVersion
                { Build = 1337, Type = VersionType.Vanilla, Version = "4.20", JarLink = "https://google.com" };
            JavaSettings dummyJavaSettings = new JavaSettings { JavaPath = "java", MaxRam = 2048, StartupParameters = "" };
            Server dummyServer = new Server("Dummy Server", dummyVersion, new VanillaSettings("world"), dummyJavaSettings);
            result.Entities.Add(dummyServer);

            return result;
        }
    }
}