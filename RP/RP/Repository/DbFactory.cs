using RP.Context;
using RP.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Repository
{
    public class DbFactory : Disposable, IDbFactory
    {
        DataContext dbContext;

        public DataContext Init()
        {
            return dbContext ?? (dbContext = new DataContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
