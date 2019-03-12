using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Company
{
    public class InwardOutward:CommonEnitity
    {
        public bool IsInward { get; set; }
        public long  City { get; set; }
        public long InvoiceNo { get; set; }
        public long ItemId { get; set; }
        public long Unit { get; set; }
        public long Quantity { get; set; }
       // public long OldQuantity { get; set; }
        //public DateTime? InwardDate { get; set; }
        //public DateTime? OutwardDate { get; set; }

        //public string OutwardNo { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string SiteName { get; set; }

        public string CreatedBy { get; set; }
    }
}