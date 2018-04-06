using System;
using System.Data;
using System.Linq.Expressions;
using UnitORM.Data;

namespace UnitORM
{
    public interface IOdbVisitor
    {
        IDiagram Diagram { get; }
     
        bool HasCount { get; }

        void Clear();

        IDbDataParameter[] GetParamters();
      
        string Translate(Expression expression);
    }
}
