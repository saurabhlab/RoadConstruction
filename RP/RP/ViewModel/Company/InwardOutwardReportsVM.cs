using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class InwardOutwardReportsVM
    {
        public long Id { get; set; }
       // public DateTime CreatedDate { get; set; } 
       // public string CreatedBy { get; set; }       
       // public bool IsInward { get; set; }
        public string City { get; set; }
        public string InvoiceNo { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public long Quantity { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        //public long OldQuantity { get; set; }
        //  public DateTime? ReturnDate { get; set; }
        public string SiteName { get; set; }
    }
}