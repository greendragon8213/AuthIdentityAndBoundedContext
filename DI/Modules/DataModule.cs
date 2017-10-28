using Autofac;
using DL.Identity;
using Microsoft.AspNet.Identity;

namespace DI.Modules
{
    internal class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityContext>().As<IdentityContext>().InstancePerRequest();
            builder.RegisterType<CustomUserStore>().As<IUserStore<CustomIdentityUser, int>>().InstancePerRequest();
            builder.RegisterType<CustomUserManager>().As<CustomUserManager>().InstancePerRequest();
        }
    }
}
