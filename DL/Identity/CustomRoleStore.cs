using Microsoft.AspNet.Identity.EntityFramework;

namespace DL.Identity
{
    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(IdentityContext context)
            : base(context)
        {
        }
    }
}