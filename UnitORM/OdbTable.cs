using System;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public class OdbTable
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public int Parent { get; set; }
        public string FK { get; set; }
        public string PK { get; set; }
         
        public IList<OdbColumn> Columns { get; set; }        
                   
        public string Alias
        {
            get
            {
                return "T" + this.Id;
            }
        }         
 
        public OdbTable(Type type)
        {
            this.Id = 0;
            this.Parent = -1;
          
            this.Name = OdbMapping.GetTableName(type);              

            this.Columns = new List<OdbColumn>();             
        }         
    }
}
