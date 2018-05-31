using System;
using System.Data;
using System.Linq.Expressions;
using System.Data.SQLite;
using UnitORM.Linq;

namespace UnitORM.Data.SQLite
{
    public class SQLiteOdbProvider : OdbEntityProvider, IOdbProvider
    {
        public SQLiteOdbProvider(IDbContext db) : base(db)
        {
        }

        #region Odb
        public virtual IQuery<T> CreateOdbQuery<T>()
        {
            return new SQLiteQuery<T>(this);
        }  

        public override string CreateColumn(OdbColumn col)
        {
            string dbtype = this.SqlTypeStr(col.GetMapType());
            string sql = "[" + col.Name + "] ";

            ColumnAttribute attr = col.Attribute;

            if (attr.IsKey)
            {
                sql += "INTEGER PRIMARY KEY";
            }
            else
            {
                sql += dbtype;
            }

            if (attr.IsAuto)
            {
                sql += " AUTOINCREMENT";
            }

            if (attr.IsNullable && !attr.IsKey)
            {
                sql += " NULL";
            }
            else
            {
                sql += " NOT NULL";
            }

            return sql;
        }

        public override string SqlTypeStr(Type type)
        {
            DbType dtp = OdbSqlType.Convert(type);

            if (dtp == DbType.String)
            {
                return "TEXT";
            }
            else if (dtp == DbType.StringFixedLength)
            {
                return "CHAR(1)";
            }
            else if (dtp == DbType.SByte)
            {
                return "TINYINT";
            }
            else if (dtp == DbType.Int16 || dtp == DbType.Byte)
            {
                return "SMALLINT";
            }
            else if (dtp == DbType.Int32 || dtp == DbType.UInt16)
            {
                return "INT";
            }
            else if (dtp == DbType.Int64 || dtp == DbType.UInt32)
            {
                return "INTEGER";
            }
            else if (dtp == DbType.Double)
            {
                return "REAL";
            }
            else if (dtp == DbType.Single)
            {
                return "FLOAT";
            }
            else if (dtp == DbType.Decimal)
            {
                return "NUMERIC(20,10)";
            }
            else if (dtp == DbType.Boolean)
            {
                return "BOOLEAN";
            }
            else if (dtp == DbType.DateTime)
            {
                return "TIMESTAMP";
            }
            else if (dtp == DbType.Binary)
            {
                return "BLOB";
            }
            else if (dtp == DbType.Guid)
            {
                return "GUID";
            }

            return "TEXT";
        } 

        public override IDbDataParameter CreateParameter(int index, object b)
        {
            IDbDataParameter pa = new SQLiteParameter()
            {
                ParameterName = "@p" + index,
                Value = b ?? DBNull.Value,
                DbType = OdbSqlType.Get(b)
            };
             
            return pa;
        }

        #endregion

        #region Linq         

        public override string Translate(Expression expression)
        {
            if (this.Visitor == null)
                this.Visitor = new SQLiteVisitor(this);            
 
            return this.Visitor.Translate(expression); 
        }
 
        #endregion
    }
}
