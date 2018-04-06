using System;
using System.Collections.Generic;
 
namespace UnitORM
{
    public interface ICache<T>
    {
        T Get(string key);

        IEnumerable<T> Gets(Func<T, bool> pred);

        List<T> ToList();

        void Set(string key, T value);

        bool Remove(string key);

        void Clear();

        int Count();
    }
}
