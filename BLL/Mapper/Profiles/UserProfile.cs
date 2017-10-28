using System.Linq;
using AutoMapper;
using BLL.Models;
using DL.Identity;

namespace BLL.Mapper.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForRegisterDTO, CustomIdentityUser>()
                .ForMember(x => x.ApplicationUser, o => o.ResolveUsing(c => new ApplicationUser
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserImagePath = c.UserImagePath
                }));
            
            CreateMap<CustomIdentityUser, UserPermissionDTO>()
                .ForMember(x => x.Roles, o => o.ResolveUsing(c => c.Roles.Select(pd => pd.Role).ToList()));

            CreateMap<CustomIdentityUser, UserForReadDTO>();
        }
    }
}