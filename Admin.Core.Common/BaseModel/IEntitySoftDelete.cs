namespace Admin.Core.Common.BaseModel
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public interface IEntitySoftDelete
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        bool IsDeleted { get; set; }
    }
}