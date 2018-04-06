using System;
using System.Data;

namespace UnitORM.Data
{
    public interface IProvider
    {
        IDbContext Db { get; }

        string CreateTable(string name, string[] cols);
        string DropTable(string name);
        string CreateColumn(OdbColumn col);

        IDbDataParameter CreateParameter(int index, object value);
    }
}
