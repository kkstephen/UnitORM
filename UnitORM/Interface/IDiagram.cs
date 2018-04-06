using System;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public interface IDiagram
    {
        Dictionary<string, string> Tables { get; }

        void FetchTable(Type type);

        OdbTable GetTable(Type type);
        string[] GetColumns();
        string[] GetColumns(Type type);

        void Clear();
    }
}
