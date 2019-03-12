using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RP.Models.Account
{
    public class Contact : CommonEnitity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Designation { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public long City { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public long Gender { get; set; }
        

        //public string PhotoPath { get; set; }

    }

}