using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class ReportFilterVM
    {
        public string ManagerUserId { get; set; }
        public string SupervisorUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long Worktype { get; set; }
        public string FormType { get; set; }
        public long UserType { get; set; }
        
    }
}