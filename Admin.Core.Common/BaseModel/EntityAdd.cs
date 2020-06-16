using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace Admin.Core.Common.BaseModel
{
    /// <summary>
    ///     实体创建审计
    /// </summary>
    /// <typeparam name="TKye"></typeparam>
    public class EntityAdd<TKye> : Entity<TKye>, IEntityAdd<TKye> where TKye : struct
    {
        /// <summary>
        ///     创建者Id
        /// </summary>
        [Description("创建者Id")]
        [Column(Position = -3, CanUpdate = false)]
        public TKye? CreatedUserId { get; set; }


        /// <summary>
        ///     创建者
        /// </summary>
        [Description("创建者")]
        [Column(Position = -2, CanUpdate = false)]
        [MaxLength(50)]
        public string CreatedUserName { get; set; }

        /// <summary>
        ///     建仓时间
        /// </summary>
        [Description("创建时间")]
        [Column(Position = -1, CanUpdate = false, ServerTime = DateTimeKind.Local)]
        public DateTime? CreatedTime { get; set; }
    }

    public class EntityAdd : EntityAdd<long>
    {
    }
}