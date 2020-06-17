using System.Threading.Tasks;
using Admin.Core.Common.Output;
using Admin.Core.Model.Admin;

namespace Admin.Core.Service.Admin.User
{
    public interface IUserService
    {
        Task<UserEntity> GetAsync(long id);
    }
}