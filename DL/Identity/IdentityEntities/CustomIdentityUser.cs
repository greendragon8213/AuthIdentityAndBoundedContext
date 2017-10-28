using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DL.Identity
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
}
