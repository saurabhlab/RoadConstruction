using RP.Models.Common;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Common;
using RP.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.IServices
{
    public interface IAccountService
    {
        //ProcessedResponse GetNewOrderPrincipleList(OrderBuilderViewModel orderBuilderViewModel);
        ResponseViewModel GetUserPermissionData(IdViewModel idViewModel);
        ResponseViewModel UpdatePermissions(AccessPermissionVM accessPermissionVM);
        ResponseViewModel GetLogedInUserDetails(IdViewModel idViewModel);
    }
}
