using System.Data.Entity;
using DB.ApplicationEntities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DB.IdentityEntities
{
    public class FullDbContext : IdentityDbContext<CustomIdentityUser, CustomRole,
        int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<CustomUserLogin> UserLogins { get; set; }
        public DbSet<CustomUserClaim> UserClaims { get; set; }
        public DbSet<CustomUserRole> UserRoles { get; set; }

        public DbSet<Something> Somethings { get; set; }
        public DbSet<ApplicationUserToSomething> ApplicationUserToSomethings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        public FullDbContext() : base("ApplicationContext")
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomIdentityUser>()
                .ToTable("IdentityUsers")
                .Ignore(x => x.Password)
                //.Ignore(x => x.ConfirmPassword)
                .Ignore(x => x.AccessFailedCount)
                //.Ignore(x => x.Email)
                .Ignore(x => x.EmailConfirmed)
                .Ignore(x => x.LockoutEnabled)
                .Ignore(x => x.LockoutEndDateUtc)
                //.Ignore(x => x.PhoneNumber)
                .Ignore(x => x.PhoneNumberConfirmed)
                .Ignore(x => x.AccessFailedCount)
                //.Ignore(x => x.SecurityStamp)
                .Ignore(x => x.TwoFactorEnabled);

            modelBuilder.Entity<CustomUserRole>()
               .HasKey(r => new { r.UserId, r.RoleId })
               .ToTable("UserRoles");

            modelBuilder.Entity<CustomUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("UserLogins");
        }
    }
}
