using System;
using System.Text;
using System.Collections.Generic;

namespace UnitORM.Data
{
    public class OdbDiagram : IDiagram
    {
        private Dictionary<string, OdbTable> _nodes { get; set; }

        public OdbTable Root { get; private set; } 
         
        private List<string> _cols;

        private Dictionary<string, string> _tables;
        public Dictionary<string, string> Tables
        {
            get
            {
                if (this._tables.Count == 0)
                {
                    this.getNodes(this.Root);
                }

                return this._tables;
            }
        }

        public OdbDiagram()
        {
            this._nodes = new Dictionary<string, OdbTable>();            
            this._tables = new Dictionary<string, string>();

            this._cols = new List<string>();
        }
 
        public void FetchTable(Type type) 
        {
            var t = OdbMapping.CreateTable(type);

            if (this.Root != null && this.Root == t)
            {
                return;
            }

            this.Clear();

            this.Root = t; 

            this._nodes.Add(this.Root.Name, this.Root);

            this.findNodes(this.Root);
        }
        
        private void findNodes(OdbTable node)
        { 
            foreach (OdbColumn col in node.Columns)
            {
                if (col.Attribute.IsMapped)
                {
                    OdbTable tableNode = OdbMapping.CreateTable(col.GetMapType());

                    tableNode.Id = this._nodes.Count;
                    tableNode.Parent = node.Id;
                    tableNode.FK = col.Name;

                    this._nodes.Add(tableNode.Name, tableNode);

                    this.findNodes(tableNode);
                }
            }
        }
                      
        public OdbTable GetTable(string name)
        {
            if (this._nodes.ContainsKey(name))
                return this._nodes[name];

            return null;         
        }

        public OdbTable GetTable(Type type)
        {
            string name = OdbMapping.GetTableName(type);

            return this.GetTable(name);
        }
        
        private void getNodes(OdbTable root)
        {
            foreach (OdbTable child in this._nodes.Values)
            {
                if (child.Parent == root.Id)
                {
                    string table = Enclosed(child.Name) + " AS " + child.Alias;

                    string key = Enclosed(child.Alias) + "." + Enclosed(child.PK);
                    string val = Enclosed(root.Alias) + "." + Enclosed(child.FK);

                    this._tables.Add(table, key + " = " + val);

                    this.getNodes(child);
                }
            }
        }

        public string[] GetColumns()
        { 
            return this.GetColumns(this.Root);
        }

        public string[] GetColumns(Type type)
        {
            var t = GetTable(type);

            return this.GetColumns(t);
        }

        public string[] GetColumns(OdbTable table)
        {
            if (this._cols.Count == 0)
            {
                this.getColumns(table);
            }

            return this._cols.ToArray();
        }

        private void getColumns(OdbTable node)
        {
            foreach (OdbColumn col in node.Columns)
            { 
                string name = Enclosed(node.Alias) + "." + Enclosed(col.Name);
                string alias = Enclosed(node.Alias + "." + col.Name);

                this._cols.Add(name + " AS " + alias);                 
            }

            foreach (OdbTable child in this._nodes.Values)
            {
                if (child.Parent == node.Id)
                {
                    this.getColumns(child);
                }
            } 
        }

        public static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }

        public void Clear()
        {
            if (this._nodes != null)
                this._nodes.Clear();

            if (this._cols != null)
                this._cols.Clear();

            if (this._tables != null)
                this._tables.Clear();
        }
    }
}
