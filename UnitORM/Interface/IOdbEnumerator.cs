using System;
using System.Collections.Generic;

namespace UnitORM
{
    public interface IOdbEnumerator<T> : IDisposable
    {
        object GetObject(Type type);        
    }
}
