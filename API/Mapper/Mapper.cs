using AutoMapper;

namespace API.Mapper
{
    public class Mapper
    {
        private static IMapper _mapper;

        private Mapper() { }

        public static IMapper GetMapperInstance => _mapper ?? (_mapper = AutoMapperAPIConfig.ServiceMapper());
    }
}