using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RP.Models.Account
{
    public class AccessPermission : CommonEnitity
    {
        public string UserId { get; set; }
        public long UserType { get; set; }
        public bool Views { get; set; }
        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool IsMaterialInward { get; set; }
        public bool IsMaterialOutward { get; set; }
        public bool IsWorkGenrate { get; set; }
        public bool IsWorkComplete { get; set; }
        public bool IsWorkReports { get; set; }
        public bool IsMaterialReports { get; set; }

        [ForeignKey("Contacts")]
        public long ContactId { get; set; }
        public virtual Contact Contacts { get; set; }

    }
}