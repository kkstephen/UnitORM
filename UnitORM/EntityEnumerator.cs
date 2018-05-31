using System;
using System.Data;

namespace UnitORM.Data
{
    public class EntityEnumerator<T> : GenericEnumerator<T> where T : IEntity 
    {  
        public IDiagram Diagram { get; private set; }

        public EntityEnumerator(IDataReader rdr, IDiagram diagram) : base(rdr)
        {  
            this.Diagram = diagram;
        } 

        public override object GetEntity(Type type)
        { 
            object instance = Activator.CreateInstance(type);
          
            OdbTable table = this.Diagram.GetTable(type);

            if (table == null)
                throw new OdbException("No table.");

            foreach (OdbColumn col in table.Columns)
            {
                //Get column value
                object value = this.GetValue(table.Alias + "." + col.Name);

                if (!col.Attribute.IsMapped)
                {
                    col.SetValue(instance, value);
                }
                else if (value != null)
                {
                    //Get FK value
                    Type tp = col.GetMapType();

                    IEntity entity = this.GetEntity(tp) as IEntity;

                    col.SetValue(instance, entity);
                }
            }

            return instance;
        }
    }
}
