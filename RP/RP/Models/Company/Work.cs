using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Company
{
    public class Work : CommonEnitity
    {
        public long CaseNo { get; set; }
        public string WorkId { get; set; }
        public string UserId { get; set; }
        public long City { get; set; }
        public string CaseSite { get; set; }
        public string Landmark { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public string Ladditude { get; set; }
        public string Longitude { get; set; }
        public DateTime? BeforeDate { get; set; }
        public DateTime? AfterDate { get; set; }
        public string AfterImage { get; set; }
        public string BeforeImage { get; set; }
        public long WorkStatus { get; set; }
        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
        public string CompletedBy { get; set; }
        public string RegenrateBy { get; set; }
        public DateTime? RegenrateDate { get; set; }




    }
}