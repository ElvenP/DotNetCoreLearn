﻿using System.Linq;
using Admin.Core.Model.Admin;
using Admin.Core.Service.Admin.User.Input;
using Admin.Core.Service.Admin.User.Output;
using AutoMapper;

namespace Admin.Core.Service.Admin.User
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            //新增
            CreateMap<UserAddInput, UserEntity>();
            CreateMap<UserUpdateInput, UserEntity>();

            //修改
            CreateMap<UserChangePasswordInput, UserEntity>();
            CreateMap<UserUpdateBasicInput, UserEntity>();

            //查询
            CreateMap<UserEntity, UserGetOutput>().ForMember(
                d => d.RoleIds,
                m => m.MapFrom(s => s.Roles.Select(a => a.Id))
            );

            CreateMap<UserEntity, UserListOutput>().ForMember(
                d => d.RoleNames,
                m => m.MapFrom(s => s.Roles.Select(a => a.Name))
            );
        }
    }
}