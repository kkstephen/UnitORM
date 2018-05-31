using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitORM.Data;

namespace UnitTest
{
    [TestClass]
    public class TestList
    {
        private MyUnit unit = UnitDB.Build() as MyUnit;

        [TestMethod]
        public void TestCollect()
        {
            var students = unit.Students.ToList();

            Assert.IsTrue(students.Count > 0);
        }
    }
}
