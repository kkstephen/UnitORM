using System;
using System.Data;

namespace UnitORM.Data.SQLite
{
    public class SQLiteQuery<T> : OdbQuery<T>  
    {
        private bool _limit;

        public SQLiteQuery(IOdbProvider provider) : base(provider)
        {
            this._limit = false;
        }
          
        public override IQuery Skip(int start)
        {
            this.SetLimit();
            this._sb.Append(start.ToString());

            return this;
        }

        public override IQuery Take(int n)
        {
            if (!this._limit)
            {
                this.Skip(0);
            }

            this._sb.Append(" , " + n.ToString());

            return this;
        }

        private void SetLimit()
        {
            if (!this._limit)
            {
                this._sb.Append(" LIMIT ");

                this._limit = true;
            }
        }

        public override string ToString()
        {
            return this._sb.ToString(); 
        }         
    }
}
