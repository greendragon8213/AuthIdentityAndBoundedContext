using API.Models;
using AutoMapper;
using BLL.Models;

namespace API.Mapper.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForRegisterVM, UserForRegisterDTO>();
        }
    }
}
