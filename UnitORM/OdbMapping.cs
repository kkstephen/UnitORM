using System;
using System.Reflection;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public class OdbMapping
    {
        public static OdbTable CreateTable(Type type)
        {
            OdbTable table = new OdbTable(type);

            foreach (OdbColumn col in GetColumns(type))
            {
                if (col.Attribute.IsPrimaryKey)
                    table.PK = col.Name;

                table.Columns.Add(col);
            }

            return table;
        }

        public static string GetTableName(Type type)
        {
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), false);
 
            if (tableAttributes.Length > 0)
            {
                TableAttribute attribute = tableAttributes[0] as TableAttribute;

                return attribute.Name;
            }

            return type.Name;
        }

        public static ColumnAttribute GetColAttribute(PropertyInfo ptyInfo)
        {
            object[] objAttrs = ptyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);

            ColumnAttribute colAttr;

            if (objAttrs.Length > 0)
            {
                colAttr = objAttrs[0] as ColumnAttribute; 
            }
            else
            {
                colAttr = new ColumnAttribute();
            }

            if (!IsGenericList(ptyInfo.PropertyType))
            {
                if (ptyInfo.PropertyType.IsClass)
                {
                    if (!colAttr.IsForeignKey)
                    {
                        //ignore all class
                        if (ptyInfo.PropertyType != typeof(string))
                        {
                            colAttr.NotMapped = true;
                        }
                        else
                        {
                            if (colAttr.Length == 0)
                                colAttr.Length = 255;
                        }
                    }
                }               
            }
            else
            {
                colAttr.IsList = true;
            }
       
            if (ptyInfo.Name == "Id")
            {
                colAttr.IsPrimaryKey = true;
                colAttr.IsAuto = true;
                colAttr.IsNullable = false;
            }

            return colAttr;
        }

        public static bool IsGenericList(Type type)
        {             
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>)));
        }

        public static IEnumerable<OdbColumn> GetColumns(Type type)
        {
            PropertyInfo[] propes = type.GetProperties();

            for (int i = 0; i < propes.Length; i++)
            { 
                ColumnAttribute colAttr = GetColAttribute(propes[i]);

                if (!colAttr.NotMapped)
                {
                    yield return new OdbColumn(propes[i], colAttr); 
                }
            }
        } 
    }
}
