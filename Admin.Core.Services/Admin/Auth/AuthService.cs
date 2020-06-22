using System;
using System.Threading.Tasks;
using AutoMapper;
using Admin.Core.Common.Output;
using Admin.Core.Repository.Admin;
using Admin.Core.Common.Cache;
using Admin.Core.Common.Extensions;
using Admin.Core.Common.Helpers;
using Admin.Core.Service.Admin.Auth.Input;
using Admin.Core.Service.Admin.Auth.Output;

namespace Admin.Core.Service.Admin.Auth
{
    public class AuthService : IAuthService
    {
    
        private readonly ICache _cache;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public AuthService(
            ICache cache,
            IMapper mapper,
            IUserRepository userRepository

        )
        {
            _cache = cache;
            _mapper = mapper;
            _userRepository = userRepository;
         
        }

        public async Task<IResponseOutput> LoginAsync(AuthLoginInput input)
        {
            

            var user = (await _userRepository.GetAsync(a => a.UserName == input.UserName));
            if (!(user?.Id > 0))
            {
                return ResponseOutput.NotOk("账号输入有误!", 3);
            }

            #region 解密
            if (input.PasswordKey.NotNull())
            {
                var passwordEncryptKey = string.Format(CacheKey.PassWordEncryptKey, input.PasswordKey);
                var existsPasswordKey = await _cache.ExistsAsync(passwordEncryptKey);
                if (existsPasswordKey)
                {
                    var secretKey = await _cache.GetAsync(passwordEncryptKey);
                    if (secretKey.IsNull())
                    {
                        return ResponseOutput.NotOk("解密失败！", 1);
                    }
                    input.Password = DesEncrypt.Decrypt(input.Password, secretKey);
                    await _cache.DelAsync(passwordEncryptKey);
                }
                else
                {
                    return ResponseOutput.NotOk("解密失败！", 1);
                }
            }
            #endregion

            var password = Md5Encrypt.Encrypt32(input.Password);
            if (user.Password != password)
            {
                return ResponseOutput.NotOk("密码输入有误！", 4);
            }

            var authLoginOutput = _mapper.Map<AuthLoginOutput>(user);

            return ResponseOutput.Ok(authLoginOutput);
        }


        public async Task<IResponseOutput> GetPassWordEncryptKeyAsync()
        {
            //写入Redis
            var guid = Guid.NewGuid().ToString("N");
            var key = string.Format(CacheKey.PassWordEncryptKey, guid);
            var encyptKey = StringHelper.GenerateRandom(8);
            await _cache.SetAsync(key, encyptKey, TimeSpan.FromMinutes(5));
            var data = new { key = guid, encyptKey };

            return ResponseOutput.Ok(data);
        }


    }
}
