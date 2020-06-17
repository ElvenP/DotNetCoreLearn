using System.Linq;
using System.Threading.Tasks;
using Admin.Core.Model.Admin;
using Admin.Core.Repository.Admin;

namespace Admin.Core.Service.Admin.User
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserEntity> GetAsync(long id)
        {
            var entity = await _userRepository.Select
                .WhereDynamic(id)
                .IncludeMany(a => a.Roles.Select(b => new RoleEntity { Id = b.Id }))
                .ToOneAsync();

            return entity;

        }
    }
}