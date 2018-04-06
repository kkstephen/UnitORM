using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitORM.Data
{
    public class OdbException : Exception
    {
        public OdbException(string message) : base(message)
        {             
        }         
    }
}
