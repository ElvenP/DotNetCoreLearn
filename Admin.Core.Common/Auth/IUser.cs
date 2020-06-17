namespace Admin.Core.Common.Auth
{
    public interface IUser
    {
        /// <summary>
        /// 主键
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        string NickName { get; }
    }
}