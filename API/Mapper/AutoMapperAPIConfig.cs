using System.Reflection;
using AutoMapper;

namespace API.Mapper
{
    public static class AutoMapperAPIConfig
    {
        public static IMapper ServiceMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
            });

            return config.CreateMapper();
        }
    }
}
