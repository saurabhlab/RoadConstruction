using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.AccountVM
{
    public class UserListVM
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }

    }
}