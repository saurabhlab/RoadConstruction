using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Master
{
    public class DropdownVm
    {
        public List<DropDownFieldVM> Items { get; set; }
        public List<DropDownFieldVM> Designations { get; set; }
        public List<DropDownFieldVM> Units { get; set; }
    }

    public class DropDownFieldVM
    {
        public long Id { get; set; }
        //public DateTime CreatedDate { get; set; } 

        //public bool IsDeleted { get; set; }

        public string GroupName { get; set; }
        public string FieldName { get; set; }
    }
}