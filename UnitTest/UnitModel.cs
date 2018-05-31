using System;
using System.Collections.Generic;
using System.Linq;
using UnitORM;

namespace UnitTest
{
    public abstract class UnitEntity : IEntity
    {
        public int Id { get; set; }
    }

    public class Student : UnitEntity
    { 
        public string Name { get; set; }
        public DateTime Birthday { get; set; }  
        public Address Address { get; set; }
        public DateTime DateCreate { get; set; }

        public IList<string> ISDN { get; set; }
    }

    public class Address : UnitEntity
    { 
        public string Street { get; set; }
        public string Region { get; set; }
        public string Flat { get; set; }
    }

    public class School : UnitEntity
    {
        public string Name { get; set; }

    }
}
