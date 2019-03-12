using RP.ViewModel.Common;
using RP.ViewModel.Company;
using RP.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.IServices
{
    public interface ICommonService
    {
        ResponseViewModel GetCityList();
ResponseViewModel GetManagerList();
        ResponseViewModel GeItemMasterList();
        ResponseViewModel GeMasterLookupList();


        ResponseViewModel GetSupervisorsByMgrId(IdViewModel idViewModel);
        ResponseViewModel GetNextId(IdViewModel idViewModel);

          ResponseViewModel GetOldQuantity(InwardOutwardVM inwardOutwardVM);
 ResponseViewModel GetUserList(IdViewModel idViewModel);
 ResponseViewModel GetDashboardCount(IdViewModel idViewModel);

    }
}
