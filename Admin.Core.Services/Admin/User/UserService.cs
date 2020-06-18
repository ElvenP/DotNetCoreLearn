using System.Linq;
using System.Threading.Tasks;
using Admin.Core.Common.Attributes;
using Admin.Core.Common.Output;
using Admin.Core.Model.Admin;
using Admin.Core.Repository.Admin;
using Admin.Core.Service.Admin.User.Output;
using AutoMapper;

namespace Admin.Core.Service.Admin.User
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
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

       
    }
}