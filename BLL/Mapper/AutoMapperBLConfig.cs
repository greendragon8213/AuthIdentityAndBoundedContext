using System.Reflection;
using AutoMapper;

namespace BLL.Mapper
{
    public static class AutoMapperBLConfig
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
