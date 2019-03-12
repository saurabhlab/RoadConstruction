using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class WorkReportsVM
    {
        public long Id { get; set; }
        public string WorkId { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string CreatedBy { get; set; }       
        public long CaseNo { get; set; }        
        public string City { get; set; }
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
        public string WorkStatus { get; set; }
        public bool IsAssign { get; set; }
        public string Date { get; set; }
        public decimal Area { get; set; }
        public string RegenrateBy { get; set; }
        public DateTime? RegenrateDate { get; set; }
        //public string GenrateBy { get; set; }
        //public string CompleteBy { get; set; }
        //public string RegenerateBy { get; set; }
        //public string GenrateDate { get; set; }
        //public string CompleteDate { get; set; }
        //public string RegenerateDate { get; set; }

    }
}