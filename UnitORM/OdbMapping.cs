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
                if (col.Attribute.IsKey)
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
            
            if (ptyInfo.PropertyType == typeof(string))
            { 
                if (colAttr.Length == 0)
                    colAttr.Length = 255;
            }
            else
            {
                if (OdbType.OdbEntity.IsAssignableFrom(ptyInfo.PropertyType))
                {
                    colAttr.IsMapped = true;
                }

                if (IsList(ptyInfo.PropertyType))
                {
                    colAttr.IsIgnore = true;
                }
            }                
       
            if (ptyInfo.Name == "Id")
            {
                colAttr.IsKey = true;
                colAttr.IsAuto = true;
                colAttr.IsNullable = false;
            }

            return colAttr;
        } 

        public static IEnumerable<OdbColumn> GetColumns(Type type)
        {
            PropertyInfo[] propes = type.GetProperties();

            for (int i = 0; i < propes.Length; i++)
            { 
                ColumnAttribute colAttr = GetColAttribute(propes[i]);

                if (!colAttr.IsIgnore)
                {
                    yield return new OdbColumn(propes[i], colAttr); 
                }
            }
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
        }
    }
}
