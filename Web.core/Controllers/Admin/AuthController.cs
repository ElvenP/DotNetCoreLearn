using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Admin.Core.Common.Auth;
using Admin.Core.Common.Output;
using Admin.Core.Service.Admin.Auth;
using Admin.Core.Service.Admin.Auth.Input;
using Admin.Core.Service.Admin.Auth.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Core.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserToken _userToken;

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IUserToken userToken)
        {
            _authService = authService;
            _userToken = userToken;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]

        public async Task<IResponseOutput> GetPassWordEncryptKey()
        {
            return await _authService.GetPassWordEncryptKeyAsync();
        }

        /// <summary>
        /// 用户登录
        /// 根据登录信息生成Token
        /// </summary>
        /// <param name="input">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
       
        public async Task<IResponseOutput> Login(AuthLoginInput input)
        {
            var sw = new Stopwatch();
            sw.Start();
            var res = await _authService.LoginAsync(input);
            sw.Stop();

          

            ResponseOutput<AuthLoginOutput> output = null;
            if (!res.Success) return !res.Success ? res : GetToken(output: null);
            output = (res as ResponseOutput<AuthLoginOutput>);
            var user = output?.Data;

            return !res.Success ? res : GetToken(output);
        }


        /// <summary>
        /// 获得token
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private IResponseOutput GetToken(ResponseOutput<AuthLoginOutput> output)
        {
            if (!output.Success)
            {
                return ResponseOutput.NotOk(output.Msg);
            }

            var user = output.Data;
            var token = _userToken.Build(new[]
            {
                new Claim(ClaimAttributes.UserId, user.Id.ToString()),
                new Claim(ClaimAttributes.UserName, user.UserName),
                new Claim(ClaimAttributes.UserNickName, user.NickName)
            });

            return ResponseOutput.Ok(new { token });
        }
    }
}
