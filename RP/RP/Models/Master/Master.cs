using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Master
{
    public class ItemMaster:CommonEnitity
    {
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string Manager_ID { get; set; }
       

    }
}