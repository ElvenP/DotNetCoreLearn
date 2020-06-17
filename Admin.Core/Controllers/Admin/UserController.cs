using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.Core.Model.Admin;
using Admin.Core.Service.Admin.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Core.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userServices;

        public UserController(IUserService userServices)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// 查询单条用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserEntity> Get(long id)
        {
            return await _userServices.GetAsync(id);
        }

    }
}
