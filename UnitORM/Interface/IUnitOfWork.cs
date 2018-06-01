using System;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public interface IUnitOfWork : IDisposable
    {
        bool AutoCommit { get; set; }
        IDbContext Context { get; }   

        void Create<T>();
        void Remove<T>();
        void Clear<T>();

        IQuery Query<T>(string[] cols);
        IQuery Count<T>();
 
        void RegisterAdd<T>(T t);
        void RegisterUpdate<T>(T t);  
        void RegisterDelete<T>(int id);

        void RegisterStore<T>(T t) where T : IEntity;

        void RegisterTask(Action action);

        void Commit();
    }
}
