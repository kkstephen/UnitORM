using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitORM.Data;

namespace UnitTest
{
    [TestClass]
    public class TestCURD
    {
        private MyUnit unit = UnitDB.Build() as MyUnit;

        [TestMethod]
        public void TestInsert()
        {
            Student student = new Student();

            student.Name = "Peter Chan";
            student.Birthday = DateTime.Parse("1987-1-7");

            unit.RegisterAdd(student);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestGet()
        {
            Student student = unit.Students.Get(1);

            Assert.IsNotNull(student);
        }

        [TestMethod]
        public void TestStore()
        {
            var addr = new Address() { Flat = "10", Region = "TKO", Street = "Tam Street" };
 
            unit.RegisterStore(addr);
   
            Assert.IsTrue(addr.Id > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            Student student = unit.Students.Get(2);

            student.Birthday = DateTime.Now;

            unit.RegisterUpdate(student);

            Assert.IsNotNull(student);
        }

        [TestMethod]
        public void TestDelete()
        {
            unit.RegisterDelete<Address>(2);

            Assert.IsTrue(1 > 0);
        }
    }
}
