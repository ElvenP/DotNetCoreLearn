using System;
using System.Linq.Expressions;
using Admin.Core.Common.Auth;
using FreeSql;

namespace Admin.Core.Repository.Base
{
    public class RepositoryBase<TEntity, TKey> : BaseRepository<TEntity, TKey> where TEntity : class, new()
    {
        private readonly IUser _user;
        public RepositoryBase(UnitOfWorkManager uowm, IUser user) : base(uowm.Orm, null, null
        )
        {
            _user = user;
            uowm.Binding(this);
        }
    }

    public class RepositoryBase<TEntity> : RepositoryBase<TEntity, long> where TEntity : class, new()
    {
        public RepositoryBase(UnitOfWorkManager uowm, IUser user) : base(uowm, user)
        {
        }
    }
}