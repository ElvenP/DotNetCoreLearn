using System;

namespace Admin.Core.Common.BaseModel
{
    public class EntityUpdate<TKey> : Entity<TKey>, IEntityUpdate<TKey> where TKey : struct
    {
        public DateTime? ModifiedTime { get; set; }

        public TKey? ModifiedUserId { get; set; }

        public string ModifiedUserName { get; set; }
    }

    public class EntityUpdate : EntityUpdate<long>
    {
    }
}