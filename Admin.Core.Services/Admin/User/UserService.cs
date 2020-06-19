using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.Core.Common.Attributes;
using Admin.Core.Common.Auth;
using Admin.Core.Common.Cache;
using Admin.Core.Common.Output;
using Admin.Core.Model.Admin;
using Admin.Core.Repository.Admin;
using Admin.Core.Service.Admin.User.Output;
using AutoMapper;
using Castle.DynamicProxy.Generators;
using CacheKey = Admin.Core.Common.Cache.CacheKey;

namespace Admin.Core.Service.Admin.User
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICache _cache;
        private readonly IUser _user;
        private readonly IRolePermissionRepository RolePermissionRepository;


        public UserService(IUserRepository userRepository, IMapper mapper, ICache cache, IRolePermissionRepository rolePermissionRepository, IUser user)
        {
            _mapper = mapper;
            _cache = cache;
            RolePermissionRepository = rolePermissionRepository;
            _user = user;
            _userRepository = userRepository;
           
        }
        [Transaction]
        public async Task<ResponseOutput<UserGetOutput>> GetAsync(long id)
        {
            var res =new ResponseOutput<UserGetOutput>();

            var entity = await _userRepository.Select
                .WhereDynamic(id)
                .IncludeMany(a => a.Roles.Select(b => new RoleEntity { Id = b.Id }))
                .ToOneAsync();
            var entityDto = _mapper.Map<UserGetOutput>(entity);
            return res.Ok(entityDto);

        }

        public async Task<IList<string>> GetPermissionsAsync()
        {
            var key = string.Format(CacheKey.UserPermissions, _user.Id);
            if (await _cache.ExistsAsync(key))
            {
                return await _cache.GetAsync<IList<string>>(key);
            }
            else
            {
                var userPermissoins = await RolePermissionRepository.Select
                    .InnerJoin<UserRoleEntity>((a, b) => a.RoleId == b.RoleId && b.UserId == _user.Id && a.Permission.Type == PermissionType.Api)
                    .Include(a => a.Permission.Api)
                    .Distinct()
                    .ToListAsync(a => a.Permission.Api.Path);

                await _cache.SetAsync(key, userPermissoins);

                return userPermissoins;
            }
        }
    }
}