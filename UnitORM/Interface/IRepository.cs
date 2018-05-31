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
        int Store(T t);

        void Add(T t);
        void Update(T t);
        void Delete(T t); 

        IList<T> ToList();

        int Count();     
    }
}
