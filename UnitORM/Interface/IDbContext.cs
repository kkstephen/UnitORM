using System;
using System.Collections.Generic;
using System.Data;

namespace UnitORM.Data
{
    public interface IDbContext : IDisposable
    { 
        IOdbProvider Provider { get; }
                
        void BeginTrans();
        void SaveChanges();
        void Cancel();
    
        IQuery CreateQuery<T>();

        IQuery Select<T>(string[] cols);       
        IQuery Delete<T>();
        IQuery Update<T>();
        IQuery Count<T>();
 
        void ExecuteCreate<T>();
        void ExecuteDrop<T>();

        int ExecuteInsert<T>(T t);
        int ExecuteUpdate<T>(T t);
        int ExecuteDelete<T>(int id);
        int ExecuteReturnId<T>(T t);

        int ExecuteNonQuery(IQuery q);        
        T ExecuteSingle<T>(IQuery q);     
        IList<T> ExecuteList<T>(IQuery q); 
        IDataReader ExecuteReader(IQuery q);

        int ExecuteNonQuery(string sql, params IDbDataParameter[] cmdParms);
        object ExecuteScalar(string sql, params IDbDataParameter[] cmdParms); 
        IDataReader ExecuteReader(string sql, params IDbDataParameter[] cmdParms);
        DataTable ExecuteTable(string sql, params IDbDataParameter[] cmdParms); 
    }
}
