﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;

namespace Admin.Core.Repository.Base
{
    public abstract class RepositoryBase<TEntity, TKey> : BaseRepository<TEntity, TKey> where TEntity : class, new()
    {
        protected RepositoryBase(UnitOfWorkManager uowm) : base(uowm.Orm, null
        )
        {
            uowm.Binding(this);
        }

        public virtual Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> exp)
        {
            return Select.Where(exp).ToOneAsync();
        }
    }

    public abstract class RepositoryBase<TEntity> : RepositoryBase<TEntity, long> where TEntity : class, new()
    {
        protected RepositoryBase(UnitOfWorkManager uowm) : base(uowm)
        {
        }
    }
}