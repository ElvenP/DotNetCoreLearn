using System.Collections.Generic;
using Admin.Core.Common.BaseModel;
using FreeSql.DataAnnotations;

namespace Admin.Core.Model.Admin
{
    /// <summary>
    /// 角色 
    /// </summary>
    [Table(Name = "ad_role")]
    [Index("uk_role_name",nameof(Name),true)]
    public class RoleEntity:EntityBase
    {

        /// <summary>
        /// 名称
        /// </summary>
        [Column(StringLength = 50)]
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [Column(StringLength = 100)]
        public string Description { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public bool Enabled { get; set; }

        [Navigate(ManyToMany = typeof(UserRoleEntity))]
        public ICollection<UserEntity> Users { get; set; }


        [Navigate(ManyToMany = typeof(RolePermissionEntity))]
        public ICollection<PermissionEntity> Permissions { get; set; }
    }
}