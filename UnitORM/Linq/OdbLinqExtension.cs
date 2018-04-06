using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnitORM.Data;

namespace UnitORM.Linq
{
    public static class OdbLinqExtension
    {
        public static IEntityQuery<T> AsQueryable<T>(this IRepository<T> repo) where T : IEntity
        {
            if (repo.Unit.Context.Provider == null)
            {
                throw new NullReferenceException("provider");
            }

            return (repo.Unit.Context.Provider as IEntityProvider).CreateQuery<T>();
        }

        public static IList<T> Gets<T>(this IRepository<T> repo, Expression<Func<T, bool>> func) where T : IEntity
        {
            return repo.AsQueryable().Where(func).ToList();
        }

        public static T Get<T>(this IRepository<T> repo, Expression<Func<T, bool>> func) where T : IEntity
        {
            return repo.Gets(func).FirstOrDefault();
        }

        public static int Count<T>(this IRepository<T> repo, Expression<Func<T, bool>> func) where T : IEntity
        {
            return repo.AsQueryable().Where(func).Count();
        }
    }
}
