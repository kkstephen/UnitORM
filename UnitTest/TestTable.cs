using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitORM.Data;

namespace UnitTest
{
    [TestClass]
    public class TestTable
    {
        private IUnitOfWork unit = UnitDB.Build();

        [TestMethod]
        public void TestCreate()
        {
            this.unit.Create<Student>();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestDrop()
        { 
            this.unit.Remove<Student>();

            Assert.IsTrue(true);
        }
    }
}
