using AutoMapper;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Profiles {
    public class UserPostProfile : Profile{
        public UserPostProfile() {
            CreateMap<UserPost, UserPostDto>();
            CreateMap<UserPost, UserPostForCreationDto>();
            CreateMap<UserPostDto, UserPost>();
        }
    }
}
