using System;
using System.Data; 
using System.Collections.Generic;

namespace UnitORM.Data
{ 
    public interface IQuery
    {
        IOdbProvider Provider { get; set; }

        string Key { get; set; }
        string Table { get; set; }

        IQuery Insert(string[] cols);
        IQuery Values(string[] cols);
        IQuery Update();
        IQuery Update(string table);
        IQuery Delete();
        IQuery Select(string[] cols);
        IQuery From();
        IQuery From(string table);
        IQuery Where(string str);
        IQuery And(string str);
        IQuery Or(string str);
        IQuery OrderBy(string str);
        IQuery Eq(object val);
        IQuery NotEq(object val);
        IQuery Gt(object val);
        IQuery Lt(object val);
        IQuery Gte(object val);
        IQuery Lte(object val);
        IQuery Not();
        IQuery Is(string str);
        IQuery Like(string str);
        IQuery Set(string[] cols);
        IQuery Count(string str);
        IQuery Group(string str);
        IQuery Having(string str);
        IQuery Skip(int start);
        IQuery Take(int count);
        IQuery Join<T1>() where T1 : IEntity;
        IQuery Join(string table);
        IQuery LeftJoin<T1>() where T1 : IEntity;
        IQuery LeftJoin(string table);
        IQuery As(string str);
        IQuery On(string str);
        IQuery Equal(string str);
        IQuery SortAsc();
        IQuery SortDesc();

        IQuery Append(string str);
 
        string SetParams(object b);

        IDbDataParameter[] GetParams();

        int Execute();

        T Scalar<T>();
        T1 First<T1>();
        IList<T> ToList<T>();
        DataTable GetTable();

        IDataReader Read();
      
        string ToString();
    }

    public interface IQuery<T> : IQuery
    {        
        IList<T> ToList(); 
    }
}
