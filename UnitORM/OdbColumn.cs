using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace UnitORM.Data
{
    public class OdbColumn
    {
        public string Name { get; set; }
        public PropertyInfo Property { get; private set; }
        public ColumnAttribute Attribute { get; private set; }

        public OdbColumn(PropertyInfo property, ColumnAttribute attr)
        {
            this.Property = property;

            this.Attribute = attr;

            this.Name = string.IsNullOrEmpty(attr.Name) ? property.Name : attr.Name;
        } 

        public Type GetMapType()
        {
            return this.Property.PropertyType;
        }
 
        public virtual object GetValue(object b)
        {
            var val = this.Property.GetValue(b, null);
         
            if (Property.PropertyType.IsEnum)             
                return (int)val;            
            
            return val;             
        } 

        public virtual void SetValue(object instance, object value)
        {
            if (this.Attribute.IsKey)
                value = Convert.ToInt32(value);
 
            if (this.Property.PropertyType.IsEnum)
                value = Enum.ToObject(this.Property.PropertyType, value);

            this.Property.SetValue(instance, value, null);
        }
    }
}
