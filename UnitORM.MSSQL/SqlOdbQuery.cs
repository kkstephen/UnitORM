using System;
using System.Collections.Generic;

namespace UnitORM.Data.MSSQL
{
    public class SqlOdbQuery<T> : OdbQuery<T> 
    {
        private int _skip = 0;
        private int _take = 0;

        private string _order;

        public SqlOdbQuery(IOdbProvider provider) : base(provider)
        { 
        }

        public override IQuery Insert(string[] cols)
        { 
            this._sb.Append("INSERT INTO ");
            this._sb.Append(Enclosed(this.Table));

            this._sb.Append(" (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");
 
            return this;
        } 

        public override IQuery OrderBy(string str)
        {
            this._order = str;

            return base.OrderBy(str); 
        }

        public override IQuery Skip(int start)
        { 
            if (string.IsNullOrEmpty(this._order))
            {
                this.OrderBy("Id").SortAsc();
            }

            this._skip = start;
             
            return this;
        }

        public override IQuery Take(int count)
        {
            this._take = count;
             
            return this;
        }

        public override string ToString()
        {
            string sql = this._sb.ToString();

            if (this._skip > 0)
            {
                sql = "SELECT * FROM (" + sql.Insert(7, "ROW_NUMBER() OVER(ORDER BY " + this._order + ") AS [ROWNO],") + ") as P WHERE P.[ROWNO] > " + this._skip;
            }

            if (this._take > 0)
            {
                return sql.Insert(7, "TOP(" + this._take + ") ");
            }

            return sql; 
        }      
    }
}
