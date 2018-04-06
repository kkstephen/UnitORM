using System;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public interface IRepository
    {
        IUnitOfWork Unit { get; }
    }

    public interface IRepository<T> : IRepository
    { 
        T Get(int id);
    
        void Add(T t);
        void Update(T t);
        void Delete(int id);

        IList<T> ToList();

        int Count();     
    }
}
