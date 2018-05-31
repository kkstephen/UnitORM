using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using UnitORM.Data;
using UnitORM.Data.SQLite;

namespace UnitTest
{
    public class UnitDB 
    {
        private static readonly string _conn = "Data Source={0};Version=3;Pooling=True;Max Pool Size=10;";

        public static IUnitOfWork Build()
        { 
            string conn = string.Format(_conn, "test.db3");

            IDbContext context = new SQLiteOdbContext(conn);

            return new MyUnit(context);
        }
    }
}
