using RP.IRepository;
using RP.IServices;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Common;
using RP.ViewModel.Company;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RP.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/common")]
    public class CommonController : ApiController
    {
        #region--Inject Dependency--
        ResponseViewModel responseViewModel = new ResponseViewModel();

        private readonly IUnitOfWork unitOfWork;

        private ICommonService commonService;
        private IAccountService accountService;
        public CommonController(
            ICommonService CommonService,
            IAccountService AccountService,
        IUnitOfWork UnitOfWork
            )
        {
            this.commonService = CommonService;
            this.accountService = AccountService;
            this.unitOfWork = UnitOfWork;

        }

        #endregion



        #region--GetCityList
        [HttpGet, Route("GetCityList")]
        public HttpResponseMessage GetCityList()
        {
            var response = commonService.GetCityList();
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion

        #region--GeMasterLookupList
        [HttpGet, Route("GeMasterLookupList")]
        public HttpResponseMessage GeMasterLookupList()
        {
            //var response = commonService.GeItemMasterList();
            var response = commonService.GeMasterLookupList();
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion
        #region--GetManagerList
        [HttpGet, Route("GetManagerList")]
        public HttpResponseMessage GetManagerList()
        {
            var response = commonService.GetManagerList();
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion

        #region--GetNextId
        [HttpPost, Route("GetNextId")]
        public HttpResponseMessage GetNextId(IdViewModel idViewModel)
        {
            var response = commonService.GetNextId(idViewModel);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion



        #region--GetOldQuantity
        [HttpPost, Route("GetOldQuantity")]
        public HttpResponseMessage GetOldQuantity(InwardOutwardVM inwardOutwardVM)
        {
            var response = commonService.GetOldQuantity(inwardOutwardVM);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion


        [HttpPost, Route("UploadImage"), AllowAnonymous]
        public ResponseViewModel UploadImage()
        {
            try
            {
                var info = new ResponseViewModel();
                string sPath = "~/UploadFile/WorkPhotos/";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(sPath);
                Guid guid = Guid.NewGuid();
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FolderName = System.Web.HttpContext.Current.Request.Form;
                //import.Id = Convert.ToInt64(FolderName["ContactID"]);
                var PhotoPath = "";
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        //var savedFilePath = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"UploadFile\WorkPhotos\");
                        var savedFilePath = sPath;
                        if (!Directory.Exists(savedFilePath))
                            Directory.CreateDirectory(savedFilePath);
                       var extn= Path.GetExtension(hpf.FileName);

                        //PhotoPath = sPath + "\\" + guid + ".jpg";
                        PhotoPath = sPath + "\\" + guid + extn;
                        if (!File.Exists(PhotoPath))
                        {
                            hpf.SaveAs(PhotoPath);
                            //PhotoPath = hpf.FileName;
                            PhotoPath = "/UploadFile/WorkPhotos/" + guid + extn;
                          
                        }
                    }
                }

                responseViewModel.Content = PhotoPath;
                responseViewModel.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                if (responseViewModel.ReturnMessage != null && responseViewModel.ReturnMessage.Count > 0)
                {
                    responseViewModel.ReturnMessage.Add(ex.Message);
                }
                else
                {
                    responseViewModel.ReturnMessage = new List<string>();
                    responseViewModel.ReturnMessage.Add(ex.Message);

                }
            }
            return responseViewModel;
        }



        #region--UpdatePermissions
        [HttpPost, Route("UpdatePermissions")]
        public HttpResponseMessage UpdatePermissions(AccessPermissionVM accessPermissionVM)
        {
            var response = accountService.UpdatePermissions(accessPermissionVM);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion


        #region--GetSupervisorsByMgrId
        [HttpPost, Route("GetSupervisorsByMgrId")]
        public HttpResponseMessage GetSupervisorsByMgrId(IdViewModel idViewModel)
        {
            var response = commonService.GetSupervisorsByMgrId(idViewModel);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion

        #region--GetUserList
        [HttpPost, Route("GetUserList")]
        public HttpResponseMessage GetUserList(IdViewModel idViewModel)
        {
            var response = commonService.GetUserList(idViewModel);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion

        #region--GetDashboardCount
        [HttpPost, Route("GetDashboardCount")]
        public HttpResponseMessage GetDashboardCount(IdViewModel idViewModel)
        {
            var response = commonService.GetDashboardCount(idViewModel);
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, response, "application/json");
            }

        }
        #endregion

    }
}
