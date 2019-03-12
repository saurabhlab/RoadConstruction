using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class PdfReportVM
    {
        public string UserId { get; set; }
        public long UserType { get; set; }

        public List<InwardOutwardReportsVM> MaterialReportsList { get; set; }

        public List<WorkReportsVM> WorkReportsList { get; set; }
    }
}