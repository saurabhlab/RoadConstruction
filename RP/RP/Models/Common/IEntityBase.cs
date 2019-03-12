using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Models.Common
{
    public interface IEntityBase
    {
        long Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
