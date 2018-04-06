using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitORM.Data
{
    public interface IFactory
    {
        IUnitOfWork CreateUnit();
    }
}
