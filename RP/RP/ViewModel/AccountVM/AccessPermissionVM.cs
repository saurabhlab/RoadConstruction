using RP.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.AccountVM
{
    public class AccessPermissionVM
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
        public bool IsMaterialInward { get; set; }
        public bool IsMaterialOutward { get; set; }
        public bool IsWorkGenrate { get; set; }
        public bool IsWorkComplete { get; set; }
        public bool IsWorkReports { get; set; }
        public bool IsMaterialReports { get; set; }
        public Contact Contacts { get; set; }
    }
}