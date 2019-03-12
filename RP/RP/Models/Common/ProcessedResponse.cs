using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Models.Common
{
    public class ProcessedResponse
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }    
        public string Message { get; set; }      
        public string ErrorOrAddMsg { get; set; }
        public string Status { get; set; }      
        public object Content { get; set; }
    }
}