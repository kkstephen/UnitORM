using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using UnitORM.Linq;

namespace UnitORM.Data.MSSQL
{
    public class SqlOdbVisitor : OdbVisitor
    {
        private string sql = "";

        public SqlOdbVisitor(IOdbProvider provider) : base(provider)
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
            throw new NotImplementedException();
        }

        public override void SetSkip()
        {
            throw new NotImplementedException();
        }

        public override void SetTake()
        {
            throw new NotImplementedException();
        }

        public override string Translate(Expression expression)
        {
            if (expression == null)
                throw new Exception("expression");

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

                return "SELECT " + this._limit + this.sql;
            }

            return this.sql;
        }
    }
}
