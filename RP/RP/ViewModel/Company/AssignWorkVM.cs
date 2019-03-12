using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class AssignWorkVM
    {

        public string UserId { get; set; }

        public List<WorkReportsVM> WorkReportsVMList { get; set; }
    }
}