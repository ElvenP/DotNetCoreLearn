using Admin.Core.Common.Extensions;
using Admin.Core.Common.Helpers;
using Microsoft.AspNetCore.Http;
namespace Admin.Core.Common.Auth
{/// <summary>
 /// 用户信息
 /// </summary>
    public class User : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public User(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long Id
        {
            get
            {
                var id = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
                if (id != null && id.Value.NotNull())
                {
                    return id.Value.ToLong();
                }
                return 0;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);

                if (name != null && name.Value.NotNull())
                {
                    return name.Value;
                }

                return "";
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserNickName);

                if (name != null && name.Value.NotNull())
                {
                    return name.Value;
                }

                return "";
            }
        }
    }

    /// <summary>
    /// Claim属性
    /// </summary>
    public static class ClaimAttributes
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string UserId = "id";

        /// <summary>
        /// 用户名
        /// </summary>
        public const string UserName = "na";

        /// <summary>
        /// 姓名
        /// </summary>
        public const string UserNickName = "nn";

        /// <summary>
        /// 刷新有效期
        /// </summary>
        public const string RefreshExpires = "re";
    }
}