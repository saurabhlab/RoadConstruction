using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RP.Models.Street
{
    public class AssignStreet :CommonEnitity
    {
        public string SupervisorId { get; set; }
        public string CreatedBy { get; set; }

        [ForeignKey("Streets")]
        public long? StreetId { get; set; }
        public virtual Street Streets { get; set; }
    }
}