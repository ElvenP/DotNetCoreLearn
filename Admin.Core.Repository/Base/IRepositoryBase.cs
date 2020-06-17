using FreeSql;

namespace Admin.Core.Repository.Base
{
    public interface IRepositoryBase<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {

    }


    public interface IRepositoryBase<TEntity> : IBaseRepository<TEntity, long> where TEntity : class
    {

    } 
}