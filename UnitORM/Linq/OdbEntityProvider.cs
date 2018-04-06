using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using UnitORM.Data;

namespace UnitORM.Linq
{
    public abstract class OdbEntityProvider : OdbProvider, IEntityProvider
    {  
        public IOdbVisitor Visitor { get; set; }

        protected OdbEntityProvider(IDbContext context)
        {
            this.Db = context; 
        } 

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(EntityQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        // Queryable's collection-returning standard query operators call this method. 
        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new EntityQuery<T>(this, expression);
        }

        public virtual IEntityQuery<T> CreateQuery<T>() where T : IEntity
        {
            return new EntityQuery<T>(this);
        }

        public virtual object Execute(Expression expression)
        {
            Type type = TypeSystem.GetElementType(expression.Type);

            if (type.Name.Contains("Anonymous"))
            {
                throw new NotSupportedException("Anonymous Expression");
            }

            string sql = this.Translate(expression);

            IDataReader rdr = this.Db.ExecuteReader(sql, this.Visitor.GetParamters());
            
            //return OdbEntity
            if (OdbType.OdbEntity.IsAssignableFrom(type))             
                return Activator.CreateInstance(typeof(EntityEnumerator<>).MakeGenericType(type), new object[] { rdr, this.Visitor.Diagram });
                           
            //return any colums
            return Activator.CreateInstance(typeof(GenericEnumerator<>).MakeGenericType(type), new object[] { rdr });           
        }
      
        public virtual T Execute<T>(Expression expression)
        {
            Type type = typeof(T);

            bool isEnumerable = type.Name == "IEnumerable`1";

            var result = this.Execute(expression);

            if (isEnumerable)             
                return (T)result;
            
            var l = (result as IEnumerable).OfType<T>().ToList();

            if (l.Count > 0)
                return l[0];

            return default(T);
        } 
  
        public abstract string Translate(Expression expression); 
    }
}
