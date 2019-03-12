using RP.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RP.Models
{
    public class UserTypeMapping:CommonEnitity
    {
        public string UserId { get; set; }

        [ForeignKey("Contacts")]
        public long ContactId { get; set; }
        public virtual Contact Contacts { get; set; }

        public long UserType { get; set; }
    }
}