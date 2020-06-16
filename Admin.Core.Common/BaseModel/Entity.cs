using System.ComponentModel;
using FreeSql.DataAnnotations;

namespace Admin.Core.Common.BaseModel
{
    public class Entity<TKey> : IEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Description("编号")]
        [Column(Position = 1, IsIdentity = true)]
        public virtual TKey Id { get; set; }
    }

    public class Entity : Entity<long>
    {
    }
}