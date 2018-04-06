using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UnitORM.Linq
{ 
    public class EntityQuery<T> : IEntityQuery<T>
    {
        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public EntityQuery(IEntityProvider provider)  
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this.Provider = provider;
            this.Expression = Expression.Constant(this); 
        }

        /// <summary> 
        /// This constructor is called by Provider.CreateQuery(). 
        /// </summary> 
        /// <param name="expression"></param>
        public EntityQuery(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            this.Provider = provider;
            this.Expression = expression;
        }

        #region Enumerators
        public IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        public virtual string GetSQL()
        {
            return (this.Provider as IEntityProvider).Translate(this.Expression);
        }
    }
}
