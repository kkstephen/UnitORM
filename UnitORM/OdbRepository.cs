using System;
using System.Collections;
using System.Collections.Generic;
using UnitORM;

namespace UnitORM.Data
{
    public class OdbRepository<T> : IRepository<T> where T : IEntity
    {
        public IUnitOfWork Unit { get; set; }

        private IDiagram _diagram;
        public IDiagram Diagram
        {
            get
            {
                if (this._diagram == null)
                {
                    this._diagram = this.Unit.Context.Provider.CreateDiagram();

                    this._diagram.FetchTable(typeof(T));
                }

                return this._diagram;
            }
        }

        private IQuery _entities;
        public IQuery Entities
        {
            get
            {
                if (_entities == null)
                {
                    Type type = typeof(T);

                    OdbTable ot = this.Diagram.GetTable(type);

                    this._entities = this.Unit.Query<T>(this.Diagram.GetColumns()).As(ot.Alias);

                    foreach (var kv in this.Diagram.Tables)
                    {
                        this._entities.LeftJoin(kv.Key).On(kv.Value);
                    }
                }

                return this.Unit.Context.CreateQuery<T>().Append(this._entities.ToString());
            }
        }

        public OdbRepository(IUnitOfWork unit)
        {
            if (unit == null)
                throw new ArgumentNullException("UnitOfWork");

            this.Unit = unit;            
        }  
        #region CURD     
        
        public virtual T Get(int id)
        { 
            var table = this.Diagram.GetTable(typeof(T)); 

            var q = this.Entities.Where(table.Alias + "." + table.PK).Eq(id).Take(1);

            using (var et = new EntityEnumerator<T>(q.Read(), this.Diagram))
            {
                var list = OdbContext.Collection(et);

                return list.Count > 0 ? list[0] : default(T);
            } 
        } 

        public void Add(T t)
        {
            this.Unit.RegisterAdd(t);
        }

        public int Store(T t)
        {
            return this.Unit.Context.ExecuteReturnId(t);
        }

        public void Update(T t)
        {
            this.Unit.RegisterUpdate(t);
        }
        
        public virtual void Delete(int id)
        {
            this.Unit.RegisterDelete<T>(id);
        }
        
        public virtual int Count()
        {
            return this.Unit.Count<T>().Scalar<int>();
        }

        public virtual IList<T> ToList()
        {
            var table = this.Diagram.GetTable(typeof(T));

            var q = this.Entities;

            using (var et = new EntityEnumerator<T>(q.Read(), this.Diagram))
            {
                return OdbContext.Collection(et); 
            } 
        }
        #endregion

        #region Enumerator      
        //public IEnumerator<T> GetEnumerator()
        //{ 
        //    var et = new EntityEnumerator<T>(this.Entities.Read(), this.Diagram);

        //    return et.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}
        #endregion
    }
}
