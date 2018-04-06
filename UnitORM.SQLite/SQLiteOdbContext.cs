using System;
using System.Data;
using System.Data.SQLite;

namespace UnitORM.Data.SQLite
{
    public class SQLiteOdbContext : OdbContext
    {
        public SQLiteOdbContext(string connstr) : this(new SQLiteConnection(connstr))
        {
        }

        public SQLiteOdbContext(IDbConnection conn) : base(conn)
        {
        }

        public override IOdbProvider CreateProvider()
        {
            return new SQLiteOdbProvider(this);
        }

        public override int ExecuteReturnId<T>(T t)
        {
            IQuery q = this.createInsert(t);
                    
            this.ExecuteNonQuery(q);

            return (int)(this.Connection as SQLiteConnection).LastInsertRowId;
        }
    }
}
