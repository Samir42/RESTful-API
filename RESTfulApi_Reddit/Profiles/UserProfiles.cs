using AutoMapper;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
