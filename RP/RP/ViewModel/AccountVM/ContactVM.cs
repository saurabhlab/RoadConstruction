using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.AccountVM
{
    public class ContactVM
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public long City { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public long AccessPermissionId { get; set; }
        public string PhotoPath { get; set; }
        
    }
}