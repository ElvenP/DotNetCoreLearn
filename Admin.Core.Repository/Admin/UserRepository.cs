using Admin.Core.Common.Auth;
using Admin.Core.Model.Admin;
using Admin.Core.Repository.Base;
using FreeSql;

namespace Admin.Core.Repository.Admin
{
    public class UserRepository:RepositoryBase<UserEntity>,IUserRepository
    {
        public UserRepository(UnitOfWorkManager uowm) : base(uowm)
        {
        }
    }
}