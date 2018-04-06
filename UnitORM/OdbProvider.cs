using System;
using System.Data;

namespace UnitORM.Data
{
    public abstract class OdbProvider : IProvider
    { 
        public IDbContext Db { get; set; }

        public abstract string CreateColumn(OdbColumn col);
       
        public virtual IDiagram CreateDiagram()
        {
            return new OdbDiagram();
        }

        public abstract IDbDataParameter CreateParameter(int index, object value);        

        public virtual string CreateTable(string name, string[] cols)
        {
            string sqlTable = "CREATE TABLE IF NOT EXISTS [" + name + "] ";

            sqlTable += "(\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            return sqlTable;
        }

        public virtual string DropTable(string name)
        {
            return "DROP TABLE IF EXISTS [" + name + "]";
        }

        public abstract string SqlTypeStr(Type type);
    }
}
