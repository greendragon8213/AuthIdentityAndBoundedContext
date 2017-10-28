using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using DB.ApplicationEntities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DB.IdentityEntities
{
    public class CustomIdentityUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomIdentityUser()
        {
        }

        public override int Id { get; set; }
        
        //[Required]
        [StringLength(100)]
        public override string PhoneNumber { get; set; }

        //[Required]
        [StringLength(255)]
        public override string Email { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "CustomIdentityUser name")]
        public override string UserName { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool IsDeleted { get; set; }
        
        public virtual ApplicationUser ApplicationUser { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CustomIdentityUser, int> manager)
        {
            // Note the authenticationType must match the one defined in
            // CookieAuthenticationOptions.AuthenticationType 
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here 
            return userIdentity;
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(FullDbContext context)
            : base(context)
        {
        }
    }

    public class CustomUserClaim : IdentityUserClaim<int> { }

    public class CustomUserRole : IdentityUserRole<int>
    {
        public CustomIdentityUser CustomIdentityUser { get; set; }
        public CustomRole Role { get; set; }
    }

    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserLogin: IdentityUserLogin<int> { }

    public class CustomUserStore : UserStore<CustomIdentityUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(FullDbContext context)
            : base(context)
        {
        }
    }
}
