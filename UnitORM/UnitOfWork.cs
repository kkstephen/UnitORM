using System;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbContext Context { get; set; }

        public bool AutoCommit { get; set; }
        private List<Action> actionRegisters; 

        private bool disposed = false;

        public UnitOfWork(IDbContext context)
        {
            this.Context = context;

            this.AutoCommit = true;
            this.actionRegisters = new List<Action>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.Context != null)
                    {
                        this.Context.Dispose();                        
                    }

                    this.Cleanup();
                }

                this.Context = null;

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } 
 
        public virtual IQuery Query<T>(string[] cols)  
        {
            return this.Context.Select<T>(cols);
        }
    
        public virtual void Create<T>()
        {
            this.RegisterTask(() =>
            {
                this.Context.ExecuteCreate<T>();
            });
        } 
         
        public virtual void Remove<T>()
        {
            this.RegisterTask(() =>
            {
                this.Context.ExecuteDrop<T>();
            }); 
        }

        public virtual void Clear<T>()
        { 
            this.RegisterTask(() =>
            {
                this.Context.CreateQuery<T>().Delete().Execute();
            }); 
        } 

        public virtual IQuery Count<T>()
        {
            return this.Context.Count<T>();
        }
 
        #region Register 
        public void RegisterAdd<T>(T t)
        { 
            this.RegisterTask(() =>
            {
                this.Context.ExecuteInsert(t);
            });            
        } 

        public void RegisterUpdate<T>(T t) 
        {            
            this.RegisterTask(() =>
            {
                this.Context.ExecuteUpdate(t);
            });             
        }
         
        public void RegisterDelete<T>(int id) 
        { 
            this.RegisterTask(() =>
            {
                this.Context.ExecuteDelete<T>(id);
            });
        }

        public void RegisterTask(Action action)
        {
            this.actionRegisters.Add(action);

            if (AutoCommit)
                this.Commit();
        }

        public virtual void Commit()
        {
            try
            {
                this.Context.BeginTrans();

                foreach (var action in this.actionRegisters)
                {
                    action();
                }       
                
                this.Context.SaveChanges();                          
            }
            catch (Exception ex)
            {
                this.Context.Cancel();

                throw ex;
            }
            finally
            { 
                this.Cleanup();  

                this.AutoCommit = true;
            } 
        }

        private void Cleanup()
        {
            if (this.actionRegisters.Count > 0)
                this.actionRegisters.Clear();  
        }

        #endregion
    }
}
