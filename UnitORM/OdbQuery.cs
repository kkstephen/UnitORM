using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UnitORM.Data
{
    public abstract class OdbQuery<T> : IQuery<T>   
    {
        public IOdbProvider Provider { get; set; }
        protected StringBuilder _sb;

        public string Key { get; set; }

        private string _table;
        public string Table
        {
            get
            {
                if (string.IsNullOrEmpty(this._table))
                {
                    this._table = OdbMapping.GetTableName(typeof(T));
                }

                return this._table;
            }

            set
            {
                this._table = value;
            }
        }  

        private List<IDbDataParameter> parameters;

        public OdbQuery(IOdbProvider provider)
        { 
            this.Provider = provider;

            this._sb = new StringBuilder();
            this.parameters = new List<IDbDataParameter>();
        }             
 
        public virtual IQuery Select(string[] cols)
        {
            this._sb.Append("SELECT ");
            this._sb.Append(string.Join(",", cols));

            return this;
        }    

        public virtual IQuery From() 
        {  
            return this.From(this.Table);
        }

        public virtual IQuery From(string table)
        {
            this._sb.Append(" FROM ");
            this._sb.Append(Enclosed(table));
          
            return this;
        }

        public virtual IQuery As(string str)
        {
            this._sb.Append(" AS ");
            this._sb.Append(Enclosed(str));

            return this;
        }

        public virtual IQuery Insert(string[] cols) 
        {
            this._sb.Append("INSERT INTO "); 
            this._sb.Append(Enclosed(this.Table));

            this._sb.Append(" (");    
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");                

            return this;
        } 

        public virtual IQuery Values(string[] cols)
        {
            this._sb.Append(" VALUES (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");

            return this;
        }

        public virtual IQuery Update()
        { 
            return this.Update(this.Table);
        }

        public virtual IQuery Update(string table)
        {
            this._sb.Append("UPDATE ");
            this._sb.Append(Enclosed(table));

            return this;
        }

        public virtual IQuery Set(string[] cols)
        {
            this._sb.Append(" SET ");
            this._sb.Append(string.Join(", ", cols));

            return this;
        }
 
        public virtual IQuery Delete()
        {
            this._sb.Append("DELETE");

            return this.From();
        }
          
        public virtual IQuery Where(string str)
        {
            this._sb.Append(" WHERE ");
            this._sb.Append(str);

            return this;
        }
                
        public virtual IQuery And(string str)
        {
            this._sb.Append(" AND ");
            this._sb.Append(str); 

            return this;
        }

        public virtual IQuery Or(string str)
        {
            this._sb.Append(" OR ");
            this._sb.Append(str);  

            return this;
        }

        public virtual IQuery Equal(string str)
        {
            this._sb.Append(" = ");
            this._sb.Append(str);

            return this;
        } 

        public virtual IQuery Eq(object val)
        {
            this._sb.Append(" = ");
        
            return this.AddParams(val);
        }

        public virtual IQuery NotEq(object val)
        {
            this._sb.Append(" <> ");
       
            return this.AddParams(val);
        }

        public virtual IQuery Gt(object val)
        {
            this._sb.Append(" > ");
        
            return this.AddParams(val);
        }

        public virtual IQuery Lt(object val)
        {
            this._sb.Append(" < ");         

            return this.AddParams(val);
        }

        public virtual IQuery Gte(object val)
        {
            this._sb.Append(" >= ");
   
            return this.AddParams(val);
        }

        public virtual IQuery Lte(object val)
        {
            this._sb.Append(" <= ");
     
            return this.AddParams(val);
        }

        public IQuery Is(string str)
        {
            this._sb.Append(" IS " + str);
          
            return this;
        }
 
        public virtual IQuery Like(string str)
        {
            this._sb.Append(" LIKE ");
       
            return this.AddParams("%" + str + "%");
        }

        public virtual IQuery OrderBy(string str)
        {
            this._sb.Append(" ORDER BY ");
            this._sb.Append(str);
           
            return this;
        }

        public IQuery Group(string str)
        {
            this._sb.Append(" GROUP BY " + str);

            return this;
        }

        public IQuery Having(string str)
        {
            this._sb.Append(" HAVING ");

            return this;
        }

        public virtual IQuery SortAsc()
        {
            this._sb.Append(" ASC");

            return this;
        }

        public virtual IQuery SortDesc()
        {
            this._sb.Append(" DESC");            
      
            return this;
        }

        public virtual IQuery Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.Join(OdbMapping.GetTableName(type));
        }

        public virtual IQuery Join(string table)
        {
            this._sb.Append(" JOIN ");
            this._sb.Append(table);

            return this;
        }

        public virtual IQuery LeftJoin<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.LeftJoin(OdbMapping.GetTableName(type));
        }

        public virtual IQuery LeftJoin(string table)
        {
            this._sb.Append(" LEFT");

            return this.Join(table);
        } 

        public virtual IQuery On(string str)
        {
            this._sb.Append(" ON ");
            this._sb.Append(str);

            return this;
        }

        public IQuery Not()
        {
            this._sb.Append(" NOT ");

            return this;
        }

        public virtual IQuery Count(string str)
        {
            string[] cols = { "COUNT(" + str + ")" };

            return this.Select(cols); 
        }

        public abstract IQuery Skip(int start);

        public abstract IQuery Take(int count);      

        public virtual IQuery Append(string str)
        {
            this._sb.Append(str);

            return this;
        }

        protected string Enclosed(string str)
        { 
            return "[" + str + "]";
        }
 
        public virtual string SetParams(object b)
        {
            if (b == null)
            {
                return "NULL";
            }
            else
            {
                int i = this.parameters.Count;

                IDbDataParameter pa = this.Provider.CreateParameter(i, b);

                this.parameters.Add(pa);

                return pa.ParameterName;
            }
        }
        
        public virtual IQuery<T> AddParams(object b)
        {
            string name = this.SetParams(b);

            this._sb.Append(name);

            return this;
        }

        public IDbDataParameter[] GetParams()
        {
            return this.parameters.ToArray();
        }

        public virtual int Execute()
        {
            return this.Provider.Db.ExecuteNonQuery(this);
        }         

        public IDataReader Read()
        {
            return this.Provider.Db.ExecuteReader(this);
        }
        
        public DataTable GetTable()
        {
            return this.Provider.Db.ExecuteTable(this.ToString(), this.GetParams());
        }

        public virtual T1 First<T1>()
        {
            return this.Provider.Db.ExecuteSingle<T1>(this); 
        }

        public virtual T1 Scalar<T1>()
        {
            return (T1)this.Provider.Db.ExecuteScalar(this.ToString(), this.GetParams());
        } 

        public virtual IList<T> ToList()
        {
            return this.ToList<T>();
        }

        public virtual IList<T1> ToList<T1>()
        {
            return this.Provider.Db.ExecuteList<T1>(this);
        }      
    }
}
