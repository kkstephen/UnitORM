using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;

namespace UnitORM.Data
{
    public class GenericEnumerator<T> : IOdbEnumerator<T>, IEnumerable<T>
    {
        protected IDataReader dr; 

        public IList<OdbColumn> Columns { get; private set; }
       
        private bool disposed = false;

        public GenericEnumerator(IDataReader Reader)
        {
            this.dr = Reader; 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.dr != null)
                    {
                        if (!this.dr.IsClosed)
                            this.dr.Close();

                        this.dr.Dispose();
                    }
                }

                this.dr = null;

                this.disposed = true;
            } 
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        public IEnumerator<T> GetEnumerator()
        {
            Type type = typeof(T);

            this.Columns = new List<OdbColumn>();

            foreach (var col in OdbMapping.GetColumns(type))
            {
                this.Columns.Add(col);
            }

            while (this.dr.Read())
            {
                object b = this.GetObject(type);

                yield return (T)b; 
            }

            //if get first object [close] will not call
            this.dr.Close();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region Get Object        
        public virtual object GetObject(Type type)
        { 
            if (OdbType.IsNullable(type))
            {
                type = Nullable.GetUnderlyingType(type);                 
            }

            if (type.IsClass && type != OdbType.String)
                return this.GetEntity(type);

            return this.GetValue(0);
        }

        public virtual object GetEntity(Type type)
        {
            object instance = FormatterServices.GetUninitializedObject(type);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                var col = Columns[i];
              
                if (!col.Attribute.IsMapped)
                {
                    var value = this.GetValue(this.Columns[i].Name);

                    this.Columns[i].SetValue(instance, value);
                }
            }

            return instance;
        } 

        public virtual object GetValue(string name)
        {
            return this.dr[name] == DBNull.Value ? null : this.dr[name];
        }

        public virtual object GetValue(int i)
        {
            return this.dr[i] == DBNull.Value ? null : this.dr[i];
        }  
        #endregion    
    }
}
