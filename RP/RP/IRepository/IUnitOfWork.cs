using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.IRepository
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
