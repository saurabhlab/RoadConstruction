using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RP.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        [Display(Name = "Student Id")]
        public int StId { get; set; }
        [Required]
        [Display(Name = "Student Name")]
        public string StName { get; set; }
        [Display(Name = "Address")]
        public string StAddress { get; set; }
        [Required]
        [Display(Name = "Mobile No.")]
        public string MobileNo { get; set; }


        public int test { get; set; }

    }
}