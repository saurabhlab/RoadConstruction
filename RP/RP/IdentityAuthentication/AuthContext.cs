using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RP.IdentityAuthentication
{
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext() : base("RPConnection") { }
    }
    public class RPCntxt : DbContext
    {
        public RPCntxt() : base("RPConnection") { }
    }
}