using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.Core.Attributes;
using Admin.Core.Common.Attributes;
using Admin.Core.Common.Auth;
using Admin.Core.Common.Output;
using Admin.Core.Model.Admin;
using Admin.Core.Service.Admin.User;
using Admin.Core.Service.Admin.User.Output;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Core.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Permission]
    public class UserController : ControllerBase
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
