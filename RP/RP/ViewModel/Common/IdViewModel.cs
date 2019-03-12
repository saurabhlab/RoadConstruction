using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Common
{
    public class IdViewModel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long UserType { get; set; }
        public long CityId { get; set; }
        public string FormType{ get; set; }
    }
}