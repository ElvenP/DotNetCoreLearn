using System.Threading.Tasks;
using Admin.Core.Common.Output;
using Admin.Core.Model.Admin;
using Admin.Core.Service.Admin.User.Output;

namespace Admin.Core.Service.Admin.User
{
    public interface IUserService
    {
        Task<ResponseOutput<UserGetOutput>> GetAsync(long id);
    }
}