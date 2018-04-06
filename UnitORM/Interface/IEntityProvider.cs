using System;
using System.Linq;
using System.Linq.Expressions;
using UnitORM.Data;

namespace UnitORM
{
    public interface IEntityProvider : IQueryProvider, IProvider
    {
        string Translate(Expression expression);

        IEntityQuery<T> CreateQuery<T>() where T : IEntity;
    }
}
