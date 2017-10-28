using AutoMapper;
using BLL.Models;
using DL.Identity;

namespace BLL.Mapper.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CustomRole, RoleDTO>();
        }
    }
}
