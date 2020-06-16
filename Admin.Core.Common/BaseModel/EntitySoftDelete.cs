using FreeSql.DataAnnotations;

namespace Admin.Core.Common.BaseModel
{
    public class EntitySoftDelete<TKey> : Entity<TKey>, IEntitySoftDelete
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        [Column(Position = -1)]
        public bool IsDeleted { get; set; }
    }

    public class EntitySoftDelete : EntitySoftDelete<long>
    {

    }
}