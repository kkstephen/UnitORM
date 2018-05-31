using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnitORM;
using UnitORM.Data;

namespace UnitORM.Linq
{
    public abstract class OdbVisitor : ExpressionVisitor, IOdbVisitor
    { 
        public IOdbProvider Provider { get; set; }

        protected StringBuilder _sb;
        protected Expression _expression;
        public List<IDbDataParameter> Parameters;

        protected int _index = 0; 
        protected string _limit = "";

        private IDiagram diagram;
        public IDiagram Diagram
        {
            get
            {
                if (this.diagram == null)
                {
                    this.diagram = this.Provider.CreateDiagram();
                }

                return this.diagram;
            }
        }
       
        protected bool _is_count;
        public bool HasCount
        {
            get
            {
                return this._is_count;
            }
        }

        public OdbVisitor(IOdbProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.Provider = provider; 

            this.Parameters = new List<IDbDataParameter>();
            this._sb = new StringBuilder();
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                if (lambda.Body.NodeType != ExpressionType.Parameter)
                {
                    string sql_from = this._sb.ToString();

                    this._sb.Length = 0;
                    this._sb.Append("SELECT ");

                    if (!this.HasCount)
                    {
                        this.Visit(lambda.Body);
                    }

                    this._sb.Append(sql_from);
                }
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                //visit From
                this.Visit(m.Arguments[0]);
                this._sb.Append(" WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                //visit Where
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Skip")
            {
                this.Visit(m.Arguments[0]);

                this.SetSkip();
            
                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Take")
            {
                this.Visit(m.Arguments[0]);

                this.SetTake();

                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderBy")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderByDescending")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this._sb.Append(" DESC");
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenBy")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenByDescending")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this._sb.Append(" DESC");
            }
            else if (m.Method.Name == "Contains")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(" || '%')");
            }
            else if (m.Method.Name == "StartsWith")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE (");
                this.Visit(m.Arguments[0]);
                this._sb.Append(" || '%')");
            }
            else if (m.Method.Name == "EndsWith")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "Equals")
            {
                this.Visit(m.Object);
                this._sb.Append(" = ");
                this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == "Trim")
            {
                this._sb.Append("TRIM(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "ToLower")
            {
                this._sb.Append("LOWER(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "ToUpper")
            {
                this._sb.Append("UPPER(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "IndexOf")
            {
                this._sb.Append("INSTR(");
                this.Visit(m.Object);
                this._sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "Substring")
            {
                this._sb.Append("SUBSTR(");
                this.Visit(m.Object);
                this._sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                this.Visit(m.Arguments[1]);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "FirstOrDefault")
            {
                this.Visit(m.Arguments[0]);
                this.SetLimit();
                this._sb.Append(" 1");
            }
            else if (m.Method.Name == "Count")
            {
                this.SetCount();
                this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == "LongCount")
            {
                this.SetCount();
                this.Visit(m.Arguments[0]);
            }
            else
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));

            return m;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                    this._sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    this._sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                        this._sb.Append(" IS ");
                    else
                        this._sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                        this._sb.Append(" IS NOT ");
                    else
                        this._sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this._sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this._sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this._sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this._sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            this.Visit(b.Right);

            return b;
        } 

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this._sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;

                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                this.Diagram.FetchTable(q.ElementType);

                OdbTable ot = this.Diagram.GetTable(q.ElementType);

                this._sb.Append(" FROM ");
                this._sb.Append(Enclosed(ot.Name));
                this._sb.Append(" AS ");
                this._sb.Append(ot.Alias);

                foreach (var kv in this.Diagram.Tables)
                {
                    this._sb.Append(" LEFT JOIN ");
                    this._sb.Append(kv.Key);
                    this._sb.Append(" ON ");
                    this._sb.Append(kv.Value);
                }                               
            }
            else
            {
                this.AddParamter(c);
            }

            return c;
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            List<string> cols = new List<string>();

            int n = 0;

            foreach (string c in this.VisitMemberList(nex.Arguments))
            {
                cols.Add(c + " AS " + nex.Members[n++].Name);
            }

            if (cols.Count > 0)
                this._sb.Append(string.Join(",", cols.ToArray()));

            return nex;
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);

            this.VisitBindingList(init.Bindings);            

            return init;
        }

        protected virtual IEnumerable<string> VisitMemberList(ReadOnlyCollection<Expression> original)
        {
            int i = 0;

            while (i < original.Count)           
            {
                yield return this.GetColumnName(original[i++] as MemberExpression);
            }
        }

        protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        { 
            int i = 0;

            while (i < original.Count)
            {  
                MemberBinding b = this.VisitBinding(original[i++]);

                this._sb.Append(" AS " + b.Member.Name);

                if (i < original.Count)
                    this._sb.Append(",");              
            }

            return original;
        }

        public virtual string GetColumnName(MemberExpression m)
        {
            ColumnAttribute attr = OdbMapping.GetColAttribute(m.Member as PropertyInfo);

            if (OdbType.OdbEntity.IsAssignableFrom((m.Expression.Type)))
            {
                OdbTable table = this.Diagram.GetTable(m.Expression.Type);

                if (table != null)
                {
                    return Enclosed(table.Alias) + "." + Enclosed(attr.Name);
                }               
            }

            return attr.Name;         
        }

        public Expression VisitMemberValue(MemberExpression m)
        {
            if (m.Member is FieldInfo)
            {
                var fieldInfo = m.Member as FieldInfo;
                var constant = m.Expression as ConstantExpression;

                if (fieldInfo != null & constant != null)
                {
                    object value = fieldInfo.GetValue(constant.Value);

                    this.Visit(Expression.Constant(value));
                }
            }
            else if (m.Member is PropertyInfo)
            {
                MemberExpression mx = m;
                 
                string name = m.Member.Name;

                while (mx.Expression is MemberExpression)
                {
                    mx = mx.Expression as MemberExpression;
                    name = mx.Member.Name + "." + name;
                }

                //check the root expression type
                if (mx.Expression.NodeType == ExpressionType.Parameter)
                {
                    //find the prop name
                    this._sb.Append(this.GetColumnName(m));
                }
                else if (mx.Expression.NodeType == ExpressionType.Constant)
                {
                    var constant = (ConstantExpression)mx.Expression;

                    var obj = ((FieldInfo)mx.Member).GetValue(constant.Value);

                    //object value = ((PropertyInfo)m.Member).GetValue(fieldInfoValue, null);

                    var value = GetValue(obj, name);

                    this.Visit(Expression.Constant(value));
                }
            }

            return m;
        }

        public static object GetValue(object obj, string propertyName)
        {
            var propertys = propertyName.Split('.');

            //skip first
            int i = 1;

            while (obj != null && i < propertys.Length)
            {
                var pi = obj.GetType().GetProperty(propertys[i++]);

                if (pi != null)
                    obj = pi.GetValue(obj, null);
                else
                    obj = null;
            }

            return obj;
        }

        public virtual string GetColumns(Type type)
        {  
            return string.Join(",", this.Diagram.GetColumns(type));
        }

        public virtual void AddParamter(ConstantExpression c)
        {
            if (c.Value == null)
            {
                this._sb.Append("NULL");
            }
            else
            { 
                IDbDataParameter pa = this.Provider.CreateParameter(this._index++, c.Value);
                                
                this.Parameters.Add(pa);
                this._sb.Append(pa.ParameterName);
            }
        }
         
        public IDbDataParameter[] GetParamters()
        {
            return this.Parameters.ToArray();
        }

        protected static string Enclosed(string str)
        {
            return "[" + str + "]";
        }

        public abstract void SetSkip();
        public abstract void SetTake();

        public abstract void SetLimit();         

        public virtual void SetCount()
        {
            if (!this._is_count)
            {
                this._is_count = true;

                this._sb.Append("COUNT(*)");
            }
        }

        public void Clear()
        {
            this.Parameters.Clear();
            this._sb.Clear();
            
            this._index = 0;
            this._limit = "";
            this._is_count = false;
        }

        public abstract string Translate(Expression expression);      
    }
}
