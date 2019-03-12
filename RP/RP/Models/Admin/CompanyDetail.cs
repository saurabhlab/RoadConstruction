using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Admin
{
    public class CompanyDetail: CommonEnitity
    {
        public string CompanyName { get; set; }
        public string CompanyType { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string FaxNo { get; set; }
        public string IsoNo { get; set; }
        public string BusinessTag { get; set; }
        public string LandMark { get; set; }
        public string LatLog { get; set; }
        public string Name { get; set; }
    }
}