using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class DashboardCountVM
    {
        public long ManagerCount { get; set; }
        public long SuperVisorCount { get; set; }
        public long EngineerCount { get; set; }
    }
}