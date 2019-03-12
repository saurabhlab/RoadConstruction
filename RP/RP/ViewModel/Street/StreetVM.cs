using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RP.Common.Enums;

namespace RP.ViewModel.Street
{
    public class StreetVM
    {
        public long Id { get; set; }
        public string StreetName { get; set; }

        public string CreatedBy { get; set; }
        public bool IsSelect { get; set; }

        public UserType UserType { get; set; }
    }
}