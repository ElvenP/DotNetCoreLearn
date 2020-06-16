using System;

namespace Admin.Core.Common.BaseModel
{
    public class EntityAdd<TKye>:Entity<TKye>,IEntityAdd<TKye> where TKye:struct
    {
        public DateTime? CreatedTime { get; set; }

        public TKye? CreatedUserId { get; set; }

        public string CreatedUserName { get; set; }
    }

    public class EntityAdd : EntityAdd<long>
    {
    }
}