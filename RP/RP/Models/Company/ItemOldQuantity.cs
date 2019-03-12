using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Company
{
    public class ItemOldQuantity:CommonEnitity
    {

        public long ItemId { get; set; }
        public decimal TotalQuantity { get; set; }
        public string CreatedBy { get; set; }
    }
}