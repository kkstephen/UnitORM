using System;
using System.Data;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public abstract class OdbContext : IDbContext
    { 
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
  
        private IOdbProvider provider;
        public IOdbProvider Provider
        {
            get
            {
                if (this.provider == null)
                {
                    this.provider = this.CreateProvider();
                }

                return this.provider;
            }
        }       
         
        private bool disposed = false;
     
        public OdbContext(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            this.Connection = connection;  
        }
 
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.Transaction != null)
                    {
                        this.Transaction.Dispose();                        
                    }

                    if (this.Connection != null)
                    {
                        if (this.Connection.State == ConnectionState.Open)
                            this.Connection.Close();

                        this.Connection.Dispose();                        
                    }
                }

                this.Transaction = null;
                this.Connection = null;

                this.disposed = true;
            }           
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        } 

        #region Transaction 
        public virtual void BeginTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this.Transaction = this.Connection.BeginTransaction(IsolationLevel.Serializable); 
        }

        public virtual void SaveChanges()
        {
            if (this.Transaction == null)             
                throw new NullReferenceException("Transaction doesn't open.");

            this.Transaction.Commit();

            this.Abort();            
        }

        public virtual void Cancel()
        {
            if (this.Transaction == null)
                throw new NullReferenceException("Transaction doesn't open.");

            this.Transaction.Rollback();

            this.Abort();             
        }

        private void Abort()
        {
            this.Transaction.Dispose();
            this.Transaction = null;
        }
        #endregion

        public abstract IOdbProvider CreateProvider();       

        public virtual IQuery CreateQuery<T>() 
        {
            return this.Provider.CreateOdbQuery<T>();
        }  

        #region Syntactic sugar
        
        public virtual IQuery Select<T>(string[] cols)
        { 
            return this.CreateQuery<T>().Select(cols).From();
        } 

        public virtual IQuery Update<T>() 
        { 
            return this.CreateQuery<T>().Update();
        }

        public virtual IQuery Delete<T>()
        {
            return this.CreateQuery<T>().Delete();
        }

        public virtual IQuery Count<T>()  
        {
            return this.CreateQuery<T>().Count("*").From();
        }

        #endregion

        /// <summary>
        /// Create Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void ExecuteCreate<T>() 
        {
            this.Create(typeof(T));
        }

        public virtual void Create(string table, string[] cols)
        {
            string sql = this.Provider.CreateTable(table, cols);

            this.ExecuteNonQuery(sql);
        }

        private void Create(Type type)
        { 
            List<string> cols = new List<string>();
 
            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                if (col.Attribute.IsMapped)
                    this.Create(col.GetMapType());
                              
                cols.Add(this.Provider.CreateColumn(col));                
            }
            
            string table = OdbMapping.GetTableName(type);

            this.Create(table, cols.ToArray());           
        }
    
        /// <summary>
        /// Drop Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void ExecuteDrop<T>()
        { 
            this.Drop(typeof(T));
        }

        public virtual void Drop(string table)
        {
            string sqlDrop = this.Provider.DropTable(table);

            this.ExecuteNonQuery(sqlDrop);
        }

        private void Drop(Type type)
        {
            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                if (col.Attribute.IsMapped)
                {
                    this.Drop(col.GetMapType());
                }
            }

            string table = OdbMapping.GetTableName(type);

            this.Drop(table);
        } 

        /// <summary>
        /// Get Object List
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// 
        public IList<T> ExecuteList<T>(IQuery q)
        {
            using (var et = new GenericEnumerator<T>(q.Read()))
            {
                return Collection(et);
            }
        }
 
        public virtual T ExecuteSingle<T>(IQuery q)
        {
            var list = this.ExecuteList<T>(q);

            return list.Count > 0 ? list[0] : default(T);               
        }         
         
        public IDataReader ExecuteReader(IQuery q)
        {
            return this.ExecuteReader(q.ToString(), q.GetParams());
        }
 
        public static IList<T> Collection<T>(IEnumerable<T> enumerator) 
        {
            IList<T> list = new List<T>();

            foreach (T t in enumerator)
            {
                list.Add(t);
            }

            return list;
        }        

        protected IQuery createInsert<T>(T t)
        {
            Type type = t.GetType();

            IQuery query = this.CreateQuery<T>();

            query.Table = OdbMapping.GetTableName(type);

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();

            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                ColumnAttribute attr = col.Attribute;

                if (!attr.IsAuto)               
                {
                    object b = col.GetValue(t);

                    if (attr.IsMapped && b != null)
                        b = (b as IEntity).Id;

                    string pa = query.SetParams(b);

                    ps.Add(pa);
                    cols.Add("[" + col.Name + "]");                    
                }

                if (attr.IsKey)
                {
                    query.Key = col.Name;                    
                }
            }

            query.Insert(cols.ToArray()).Values(ps.ToArray());

            return query;
        }

        /// <summary>
        /// Insert object
        /// </summary>     
        public virtual int ExecuteInsert<T>(T t)
        {
            IQuery q = this.createInsert(t);

            return q.Execute();
        }

        /// <summary>
        /// Update object
        /// </summary>       
        public virtual int ExecuteUpdate<T>(T t)
        {
            Type type = t.GetType();

            IQuery query = this.CreateQuery<T>();

            query.Table = OdbMapping.GetTableName(type);
 
            List<string> cols = new List<string>();

            object value = 0;
 
            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                ColumnAttribute attr = col.Attribute; 

                object b = col.GetValue(t); 

                if (!attr.IsAuto)
                {
                    if (attr.IsMapped && b != null)
                        b = (b as IEntity).Id;

                    string pa = query.SetParams(b);

                    cols.Add("[" + col.Name + "]" + "=" + pa);                   
                }      
                
                if (attr.IsKey)
                {
                    query.Key = col.Name;                   
                    value = b;
                }
            }

            if (!string.IsNullOrEmpty(query.Key))
            {
                query.Update().Set(cols.ToArray()).Where(query.Key).Eq(value);

                return query.Execute();
            }
            else
            {
                throw new Exception("No key column");
            }
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>         
        public virtual int ExecuteDelete<T>(int id)
        {
            var table = OdbMapping.CreateTable(typeof(T));

            IQuery q = this.Delete<T>().Where(table.PK).Eq(id);
 
            return q.Execute();
        }

        public virtual int ExecuteNonQuery(IQuery q)
        {
            return this.ExecuteNonQuery(q.ToString(), q.GetParams());
        }

        /// <summary>
        /// Get Insert object Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>  
        public abstract int ExecuteReturnId<T>(T t);

        #region Data Access
        protected static IDbCommand SetCommand(IDbConnection conn, IDbTransaction trans, string cmdText, IDbDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            IDbCommand cmd = conn.CreateCommand();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = CommandType.Text;

            if (cmdParms != null)
            {
                foreach (IDbDataParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }

            return cmd;
        }
     
        public IDataReader ExecuteReader(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            using (IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms))
            {
                IDataReader rdr = cmd.ExecuteReader();

                cmd.Parameters.Clear();

                return rdr;
            }             
        } 

        public object ExecuteScalar(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            using (IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms))
            {
                object val = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return val;
            }             
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command
            using (IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms))
            {
                int n = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                return n;
            } 
        }

         /// <summary>
        /// Get Datatable
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// 
        public virtual DataTable ExecuteTable(string sql, params IDbDataParameter[] cmdParms)
        {
            using (var rdr = this.ExecuteReader(sql, cmdParms))
            {
                DataTable dt = new DataTable();

                dt.Load(rdr);

                return dt;
            }
        } 
        #endregion        
    }
}
