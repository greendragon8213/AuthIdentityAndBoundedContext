using Microsoft.AspNet.Identity.EntityFramework;

namespace DL.Identity
{
    public class CustomUserStore : UserStore<CustomIdentityUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(IdentityContext context)
            : base(context)
        {
        }
    }
}