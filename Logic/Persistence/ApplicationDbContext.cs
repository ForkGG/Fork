using Microsoft.EntityFrameworkCore;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Server> ServerSet { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}