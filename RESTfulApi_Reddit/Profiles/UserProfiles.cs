using AutoMapper;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Models;

namespace RESTfulApi_Reddit.Profiles
{
    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<User, UserFullDto>();
            CreateMap<User, UserDto>();
        }
    }
}
