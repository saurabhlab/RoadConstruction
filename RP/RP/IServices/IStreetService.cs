using RP.ViewModel.Common;
using RP.ViewModel.Street;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.IServices
{
    public interface IStreetService
    {
           ResponseViewModel GetStreetList(IdViewModel idViewModel);
 
ResponseViewModel AssignStreet(StreetAssignVM model);
ResponseViewModel GetAssignStreetList(IdViewModel model);
      ResponseViewModel SaveUpdateStreetData(StreetVM streetVM);
    }
}
