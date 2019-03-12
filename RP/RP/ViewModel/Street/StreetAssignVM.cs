using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Street
{
    public class StreetAssignVM
    {
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public List<StreetVM> StreetListVM { get; set; }
    }
}