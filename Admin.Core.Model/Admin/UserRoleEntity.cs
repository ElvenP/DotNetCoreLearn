using Admin.Core.Common.BaseModel;
using FreeSql.DataAnnotations;

namespace Admin.Core.Model.Admin
{

    [Table(Name = "ab_user_role")]
    public class UserRoleEntity:EntityAdd

    {
        /// <summary>
        /// 用户Id
        /// </summary>


        public UserEntity User { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>

        public long RoleId { get; set; }


        public RoleEntity Role { get; set; }
    }
}