using System.Data.Entity;
using TestApp.DL.Application.ApplicationEntities;

namespace TestApp.DL.Application
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(string nameOrConnectionString) : base("ApplicationContext")
        {
        }

        public DbSet<Something> Somethings { get; set; }
        public DbSet<ApplicationUserToSomething> ApplicationUserToSomethings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
