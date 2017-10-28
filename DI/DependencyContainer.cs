using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using DI.Modules;

namespace DI
{
    public static class DependencyContainer
    {
        internal static IContainer Container;

        public static IContainer CreateContainer(Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(assembly);

            IModule[] modules =
            {
                new DataModule(),
                new DataAccessModule(),
                new BusinessLogicModule()
            };
            
            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }

            Container = builder.Build();
            return Container;
        }
    }
}
