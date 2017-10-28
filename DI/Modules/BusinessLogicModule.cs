using System;
using Autofac;
using AutoMapper;
using BLL.Interfaces;
using BLL.Mapper;
using Module = Autofac.Module;

namespace DI.Modules
{
    internal class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            builder.Register<IMapper>(c => AutoMapperBLConfig.ServiceMapper()).SingleInstance();
            
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => typeof(IRunOnEachRequest).IsAssignableFrom(t))
                .AsImplementedInterfaces()
                .InstancePerRequest();
        }
    }
}
