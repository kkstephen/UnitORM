using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using UnitORM.Linq;

namespace UnitORM.Data.MSSQL
{
    public class SqlOdbProvider : OdbEntityProvider, IOdbProvider
    { 
        public SqlOdbProvider(IDbContext db) : base(db)
        {
        }

        public override string SqlTypeStr(Type type)
        {
            DbType dtp = OdbSqlType.Convert(type);
 
            if (dtp == DbType.String)
            {
                return "NVARCHAR";
            }
            if (dtp == DbType.AnsiStringFixedLength)
            {
                return "NCHAR(1)";
            }
            if (dtp == DbType.Byte)
            {
                return "TINYINT";
            }
            if (dtp == DbType.Int16 || dtp == DbType.SByte)
            {
                return "SMALLINT";
            }
            if (dtp == DbType.Int32 || dtp == DbType.UInt16)
            {
                return "INT";
            }
            if (dtp == DbType.UInt32 || dtp == DbType.Int64)
            {
                return "BIGINT";
            }
            if (dtp == DbType.UInt64)
            {
                return "BIGINT";
            }
            if (dtp == DbType.Double)
            {
                return "FLOAT";
            }
            if (dtp == DbType.Single)
            {
                return "REAL";
            }
            if (dtp == DbType.Decimal)
            {
                return "NUMERIC(20,10)";
            }
            if (dtp == DbType.Boolean)
            {
                return "BIT";
            }
            if (dtp == DbType.DateTime)
            {
                return "DATETIME";
            }
            if (dtp == DbType.Binary)
            {
                return "VARBINARY";
            }
            if (dtp == DbType.Guid)
            {
                return "UNIQUEIDENTIFIER";
            }
            return "NVARCHAR";
        }

        public override string CreateColumn(OdbColumn column)
        {
            string dbtype = this.SqlTypeStr(column.GetMapType());
           
            ColumnAttribute attr = column.Attribute; 
 
            if (attr.Length != 0)
                dbtype += "(" + attr.Length + ")";

            string col = "[" + column.Name + "] ";

            if (attr.IsPrimaryKey)
            {
                col += "INT PRIMARY KEY";
            }
            else
            {
                col += dbtype;
            }

            if (attr.IsAuto)
            {
                col += " IDENTITY(1,1)";
            }

            if (attr.IsNullable)
            {
                col += " NULL";
            }
            else
            {
                col += " NOT NULL";
            }

            return col;
        } 

        public override string CreateTable(string name, string[] cols)
        {
            string sql = "IF OBJECT_ID('[" + name + "]', 'U') IS NULL CREATE TABLE [" + name + "] ";

            sql += "(\r\n" + string.Join(",\r\n", cols) + "\r\n);";
            
            return sql;
        }  
         
        public override IDbDataParameter CreateParameter(int i, object b)
        {
            IDbDataParameter pa = new SqlParameter()
            {
                ParameterName = "@p" + i,
                Value = b ?? DBNull.Value,
                DbType = OdbSqlType.Get(b)
            };

            return pa;           
        }

        public IQuery<T> CreateOdbQuery<T>()  
        {
            return new SqlOdbQuery<T>(this);
        }

        public override string Translate(Expression expression)
        {
            if (this.Visitor == null)
                this.Visitor = new SqlOdbVisitor(this);

            return this.Visitor.Translate(expression);
        }
    }
}
