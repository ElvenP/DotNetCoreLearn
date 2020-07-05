using System.Threading.Tasks;
using Admin.Core.Common.Auth;
using Admin.Core.Common.Output;
using Admin.Core.Service.Admin.User;
using Admin.Core.Service.Admin.User.Output;
using Microsoft.AspNetCore.Mvc;
using Web.core.Attributes;

namespace Web.core.Controllers.Admin
{

    public class UserController : AreaController
    {

        private readonly IUserService _userServices;
       
        public UserController(IUserService userServices, IUser user)
        {
            _userServices = userServices;
          
        }

        /// <summary>
        /// 查询单条用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseOutput<UserGetOutput>> Get(long id)
        {
            return await _userServices.GetAsync(id);
        }

    }
}
