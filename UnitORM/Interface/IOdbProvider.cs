using System;
using System.Data;

namespace UnitORM.Data
{
    public interface IOdbProvider : IProvider
    {
        IQuery<T> CreateOdbQuery<T>(); 

        IDiagram CreateDiagram();
    }
}
