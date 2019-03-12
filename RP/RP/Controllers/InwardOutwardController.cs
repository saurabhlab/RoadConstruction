using RP.IRepository;
using RP.IServices;
using RP.ViewModel.Common;
using RP.ViewModel.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RP.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/inwardoutward")]
    //[EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class InwardOutwardController : ApiController
    {
            #region--Inject Dependency--
            ResponseViewModel responseViewModel = new ResponseViewModel();

            private readonly IUnitOfWork unitOfWork;

            private IInwardOutwardService inwardOutwardService;

            public InwardOutwardController(
                IInwardOutwardService InwardOutwardService,

                IUnitOfWork UnitOfWork
                )
            {
                this.inwardOutwardService = InwardOutwardService;

                this.unitOfWork = UnitOfWork;
                
            }

        #endregion

        #region--SaveInwardOutwardData
        [HttpPost, Route("SaveInwardOutwardData")]
        public HttpResponseMessage SaveInwardOutwardData(InwardOutwardVM inwardOutwardVM)
        {
            var response = inwardOutwardService.SaveInwardOutwardData(inwardOutwardVM);
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

        #region--SaveWorkGenrateData
        [HttpPost, Route("SaveWorkGenerateData")]
        public HttpResponseMessage SaveWorkGenerateData(WorkVM workVM)
        {
            var response = inwardOutwardService.SaveWorkGenerateData(workVM);
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
        #region--UpdateWorkCompleteData
        [HttpPost, Route("UpdateWorkCompleteData")]
        public HttpResponseMessage UpdateWorkCompleteData(WorkVM workVM)
        {
            var response = inwardOutwardService.UpdateWorkCompleteData(workVM);
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

        #region--GetMaterialReportsList
        [HttpPost, Route("GetMaterialReportsList")]
        public HttpResponseMessage GetMaterialReportsList(ReportFilterVM model)
        {
            var response = inwardOutwardService.GetMaterialReportsList(model);
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


        #region--GetWorkReportList
        [HttpPost, Route("GetWorkReportList")]
        public HttpResponseMessage GetWorkReportList(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.GetWorkReportList(idViewModel);
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

        #region--GetWorkReportListForManager
        [HttpPost, Route("GetWorkReportListForManager")]
        public HttpResponseMessage GetWorkReportListForManager(ReportFilterVM reportFilterVM)
        {
            var response = inwardOutwardService.GetWorkReportListForManager(reportFilterVM);
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

        

        #region--GetWorkDetailsById
        [HttpPost, Route("GetWorkDetailsById")]
        public HttpResponseMessage GetWorkDetailsById(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.GetWorkDetailsById(idViewModel);
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



        #region--AssignWork
        [HttpPost, Route("AssignWork")]
        public HttpResponseMessage AssignWork(AssignWorkVM assignWorkVM)
        {
            var response = inwardOutwardService.AssignWork(assignWorkVM);
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


       


 #region--GenrateWorkReportPdf
        [HttpPost, Route("GenrateWorkReportPdf")]
        public HttpResponseMessage GenrateWorkReportPdf(PdfReportVM pdfReportVM)
        {
            var response = inwardOutwardService.GenrateWorkReportPdf(pdfReportVM);
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



        #region--DownloadMaterialReportPdf
        [HttpPost, Route("DownloadMaterialReportPdf")]
        public HttpResponseMessage DownloadMaterialReportPdf(PdfReportVM pdfReportVM)
        {
            var response = inwardOutwardService.DownloadMaterialReportPdf(pdfReportVM);
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
        
        #region--DeleteExistingPdf
        [HttpPost, Route("DeleteExistingPdf")]
        public HttpResponseMessage DeleteExistingPdf(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.DeleteExistingPdf(idViewModel);
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



        #region--GetAdminWorkReportList
        [HttpPost, Route("GetAdminWorkReportList")]
        public HttpResponseMessage GetAdminWorkReportList(ReportFilterVM model)
        {
            var response = inwardOutwardService.GetAdminWorkReportList(model);
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

        #region--GetAdminMaterialReportsList
        [HttpPost, Route("GetAdminMaterialReportsList")]
        public HttpResponseMessage GetAdminMaterialReportsList(ReportFilterVM model)
        {
            var response = inwardOutwardService.GetAdminMaterialReportsList(model);
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

        #region--DeleteMaterialById
        [HttpPost, Route("DeleteMaterialById")]
        public HttpResponseMessage DeleteMaterialById(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.DeleteMaterialById(idViewModel);
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

        #region--WorkRegenrate
        [HttpPost, Route("WorkRegenrate")]
        public HttpResponseMessage WorkRegenrate(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.WorkRegenrate(idViewModel);
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

        #region--DeleteWorkById
        [HttpPost, Route("DeleteWorkById")]
        public HttpResponseMessage DeleteWorkById(IdViewModel idViewModel)
        {
            var response = inwardOutwardService.DeleteWorkById(idViewModel);
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
