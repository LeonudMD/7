using AuthService.Application.Contracts;
using AuthService.Domain.Models;
using AuthService.Infrastructure.Entities;
using AutoMapper;

namespace AuthService.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserEntity>();
            CreateMap<UserEntity, User>();
            CreateMap<User, LoginUserRequest>();
            CreateMap<LoginUserRequest, User>();
            CreateMap<User,RegisterUserRequest>();
            CreateMap<RegisterUserRequest, User>();
        }
    }
}
