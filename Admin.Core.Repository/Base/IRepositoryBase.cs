using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;

namespace Admin.Core.Repository.Base
{
    public interface IRepositoryBase<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> exp);


    }


    public interface IRepositoryBase<TEntity> : IRepositoryBase<TEntity, long> where TEntity : class
    {

    } 
}