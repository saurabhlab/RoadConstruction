using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.IdentityAuthentication
{
    public class ApplicationUserRole : IdentityRole
    {
        public ApplicationUserRole() : base() { }
        public ApplicationUserRole(string name) : base(name) { }
        public string Description { get; set; }
    }
}