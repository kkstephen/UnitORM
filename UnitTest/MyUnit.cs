using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitORM;
using UnitORM.Data;

namespace UnitTest
{
    public class MyUnit : UnitOfWork
    {
        private IRepository<Student> students;
        public IRepository<Student> Students
        {
            get
            {
                if (this.students == null)
                {
                    this.students = new OdbRepository<Student>(this);
                }

                return this.students;
            }
        }

        private IRepository<Address> address;
        public IRepository<Address> Address
        {
            get
            {
                if (this.address == null)
                {
                    this.address = new OdbRepository<Address>(this);
                }

                return this.address;
            }
        }

        public MyUnit(IDbContext context)
            : base(context)
        {
        }
    }
}
