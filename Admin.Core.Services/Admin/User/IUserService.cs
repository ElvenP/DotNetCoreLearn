using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Core.Common.Output;
using Admin.Core.Service.Admin.User.Output;

namespace Admin.Core.Service.Admin.User
{
    public interface IUserService
    {
        Task<ResponseOutput<UserGetOutput>> GetAsync(long id);

        Task<IList<string>> GetPermissionsAsync();
    }
}