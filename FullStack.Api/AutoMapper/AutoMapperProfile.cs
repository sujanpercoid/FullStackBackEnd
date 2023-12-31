﻿using AutoMapper;
using FullStack.Api.Models;

namespace FullStack.Api.AutoMapper
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
            CreateMap<User, UserDto>();
            CreateMap<LoginDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
            CreateMap<ProductDto, Product>();
            CreateMap<ProductDto,UserProduct>();
            CreateMap<Cart,CartDto>();

        }
    }
}
