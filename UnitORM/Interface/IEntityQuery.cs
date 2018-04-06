using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitORM
{
    public interface IEntityQuery<T> : IOrderedQueryable<T>
    {
        string GetSQL();        
    }
}
