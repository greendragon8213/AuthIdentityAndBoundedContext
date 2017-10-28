using Microsoft.AspNet.Identity.EntityFramework;

namespace DL.Identity
{
    public class CustomUserRole : IdentityUserRole<int>
    {
        public CustomIdentityUser CustomIdentityUser { get; set; }
        public CustomRole Role { get; set; }
    }
}