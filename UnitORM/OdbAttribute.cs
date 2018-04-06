using System;

namespace UnitORM
{ 
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    { 
        public string Name { get; set; }
        public int Length { get; set; }
        public bool IsAuto { get; set; }
        public bool IsNullable { get; set; }
        public bool NotMapped { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsList { get; set; }
     
        public ColumnAttribute() : this("")
        {
        }

        public ColumnAttribute(string name) : this(name, 0)
        {
        }

        public ColumnAttribute(string name, int size)
        {
            Name = name;
            Length = size;

            IsAuto = false;
            IsNullable = true;
            NotMapped = false;
            IsPrimaryKey = false;
            IsForeignKey = false;
            IsList = false;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                   Inherited = false, AllowMultiple = false)]
    public class KeyAttribute : ColumnAttribute
    { 
        public KeyAttribute(string name = "") : base(name)
        {           
            IsAuto = true;
            IsNullable = false;     
            IsPrimaryKey = true;           
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MappedAttribute : ColumnAttribute
    {
        public MappedAttribute()
        {
            this.IsForeignKey = true;
            this.IsNullable = false;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class LengthAttribute : ColumnAttribute
    {
        public LengthAttribute(int size = 255)
        {
            this.Length = size;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property,
                    Inherited = false, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public string Schema { get; set; }

        public TableAttribute() : this("")
        {
        }

        public TableAttribute(string name) : this(name, "")
        {
        }

        public TableAttribute(string name, string schema)
        {
            this.Name = name;
            this.Schema = schema;
        }
    }
}
