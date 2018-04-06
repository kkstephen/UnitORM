using System;
using System.Data;
using System.Reflection;

namespace UnitORM.Data
{
    public class OdbSqlType
    {
        public static DbType Get(object b)
        {
            if (b == null)
                throw new ArgumentNullException("Null Parameter");

            Type type = b.GetType();
             
            return Convert(type);
        }

        public static DbType Convert(Type t)
        { 
            if (t == OdbType.String)
            {
                return DbType.String;
            }

            if (t == OdbType.Char)
            {
                return DbType.AnsiStringFixedLength;
            }

            if (t == OdbType.Byte) 
            {
                return DbType.Byte;
            }

            if (t == OdbType.Bytes) 
            {
                return DbType.Binary;
            }

            if (t == OdbType.SByte) 
            {
                return DbType.SByte;
            }

            if (t == OdbType.Int32) 
            {
                return DbType.Int32;
            }

            if (t == OdbType.UInt32) 
            {
                return DbType.UInt32;
            }

            if (t == OdbType.Short) 
            {
                return DbType.Int16;
            }

            if (t == OdbType.UShort) 
            {
                return DbType.UInt16;
            }

            if (t == OdbType.Int64) 
            {
                return DbType.Int64;
            }

            if (t == OdbType.UInt64) 
            {
                return DbType.UInt64;
            }

            if (t == OdbType.Double) 
            {
                return DbType.Double;
            }

            if (t == OdbType.Single)  
            {
                return DbType.Single;
            }

            if (t == OdbType.Decimal) 
            {
                return DbType.Decimal;
            }

            if (t == OdbType.Bool) 
            {
                return DbType.Boolean;
            }

            if (t == OdbType.DateTime || t == OdbType.NullableDateTime) 
            {
                return DbType.DateTime;
            }

            if (t == OdbType.Guid) 
            {
                return DbType.Guid;
            }

            if (t.IsEnum)
            {
                return DbType.UInt32;
            }

            if (OdbType.OdbEntity.IsAssignableFrom(t))
            {
                return DbType.Int64;
            }
            
            return DbType.String;
        } 
    }
}
