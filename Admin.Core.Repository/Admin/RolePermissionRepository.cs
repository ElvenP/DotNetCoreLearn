using Admin.Core.Model.Admin;
using Admin.Core.Repository.Base;
using FreeSql;

namespace Admin.Core.Repository.Admin
{
    public class RolePermissionRepository:RepositoryBase<RolePermissionEntity>,IRolePermissionRepository
    {
        public RolePermissionRepository(UnitOfWorkManager uowm) : base(uowm)
        {
        }
    }
}