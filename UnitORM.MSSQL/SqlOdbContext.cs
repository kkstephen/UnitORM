using System;
using System.Data;
using System.Data.SqlClient;

namespace UnitORM.Data.MSSQL
{
    public class SqlOdbContext : OdbContext
    {
        public SqlOdbContext(string conn) : base(new SqlConnection(conn))
        {
        }

        public SqlOdbContext(IDbConnection conn) : base(conn)
        {
        }

        public override IOdbProvider CreateProvider()
        {
            return new SqlOdbProvider(this);
        }
        
        public override int ExecuteReturnId<T>(T t)
        {
            IQuery q = this.createInsert(t);

            string sql = q.ToString().Replace("VALUES", "OUTPUT INSERTED." + q.Key + " VALUES");

            object id = this.ExecuteScalar(sql, q.GetParams());

            return Convert.ToInt32(id);
        }
    }
}
