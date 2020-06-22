using Admin.Core.Model.Admin;

namespace Web.core.Db
{
    public class Data
    {
        public ApiEntity[] Apis { get; set; }
        public ViewEntity[] Views { get; set; }
        public PermissionEntity[] Permissions { get; set; }
        public UserEntity[] Users { get; set; }
        public RoleEntity[] Roles { get; set; }
        public UserRoleEntity[] UserRoles { get; set; }
        public RolePermissionEntity[] RolePermissions { get; set; }
    }
}