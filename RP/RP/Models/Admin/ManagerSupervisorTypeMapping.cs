using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Admin
{
    public class ManagerSupervisorTypeMapping : CommonEnitity
    {
        public string ManagerId { get; set; }
        public string SupervisorId { get; set; }


    }
}