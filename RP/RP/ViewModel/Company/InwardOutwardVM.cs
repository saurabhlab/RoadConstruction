using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Company
{
    public class InwardOutwardVM
    {

        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsInward { get; set; }
        public long City { get; set; }
        public long InvoiceNo { get; set; }
        public long ItemId { get; set; }
        public long Unit { get; set; }
        public long Quantity { get; set; }
        public long OldQuantity { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string SiteName { get; set; }

    }
}