using RP.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.AccountVM
{
    public class LoggedInUserData
    {
        public AccessPermissionVM AccessPermissions { get; set; }

       
        public string UserId { get; set; }
        public string UserType { get; set; }
      
        public Contact Contacts { get; set; }
    }
}