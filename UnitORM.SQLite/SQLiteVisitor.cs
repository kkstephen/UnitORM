using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using UnitORM.Linq;

namespace UnitORM.Data.SQLite
{
    public class SQLiteVisitor : OdbVisitor
    {
        private string sql = "";

        public SQLiteVisitor(IOdbProvider provider) : base(provider)
        {             
        } 
         
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(DateTime) && m.Member.Name == "Now")
            {
                this._sb.Append("datetime()");
            }
            else if (m.Member.DeclaringType == typeof(string) && m.Member.Name == "Length")
            {
                this._sb.Append("LENGTH(");
                this.Visit(m.Expression);
                this._sb.Append(")");
            }                      
            else if (OdbType.OdbEntity.IsAssignableFrom(m.Type))
            {                 
                this._sb.Append(this.GetColumns(m.Type));
            }
            else 
            {
                this.VisitMemberValue(m);
            }        

            return m;
        }

        public override void SetLimit()
        {
            if (this._limit == "")
            {
                this._limit = " LIMIT ";
                this._sb.Append(this._limit);
            }
        }

        public override void SetSkip()
        {
            this.SetLimit();
            this._sb.Append(" OFFSET ");
        }

        public override void SetTake()
        {
            this.SetLimit();
        }

        public override string Translate(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            this._expression = expression;

            this.Clear(); 

            this.Visit(this._expression);

            this.sql = this._sb.ToString(); 

            if (!this.sql.StartsWith("SELECT"))
            {
                if (!this.HasCount)
                {
                    Type type = TypeSystem.GetElementType(this._expression.Type);

                    this.sql = this.GetColumns(type) + this.sql;
                }

                return "SELECT " + this.sql;  
            }                

            return this.sql; 
        }
    } 
}
