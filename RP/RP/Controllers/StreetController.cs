using RP.IRepository;
using RP.IServices;
using RP.Models.Street;
using RP.ViewModel.Common;
using RP.ViewModel.Street;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RP.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/street")]
    public class StreetController : ApiController
    {
        #region--Inject Dependency--
        ResponseViewModel responseViewModel = new ResponseViewModel();

        private readonly IUnitOfWork unitOfWork;

        private IStreetService iStreetServiceService;

        public StreetController(
            IStreetService IStreetServiceService,

        IUnitOfWork UnitOfWork
            )
        {
            this.iStreetServiceService = IStreetServiceService;

            this.unitOfWork = UnitOfWork;

        }

        #endregion



        #region--SaveUpdateStreetData
        [HttpPost, Route("SaveUpdateStreetData")]
        public HttpResponseMessage SaveUpdateStreetData(StreetVM streetVM)
        {
            var response = iStreetServiceService.SaveUpdateStreetData(streetVM);
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

        #region--GetStreetList
        [HttpPost, Route("GetStreetList")]
        public HttpResponseMessage GetStreetList(IdViewModel idViewModel)
        {
            var response = iStreetServiceService.GetStreetList(idViewModel);
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

        #region--AssignStreet
        [HttpPost, Route("AssignStreet")]
        public HttpResponseMessage AssignStreet(StreetAssignVM model)
        {
            var response = iStreetServiceService.AssignStreet(model);
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

        #region--GetAssignStreetList
        [HttpPost, Route("GetAssignStreetList")]
        public HttpResponseMessage GetAssignStreetList(IdViewModel model)
        {
            var response = iStreetServiceService.GetAssignStreetList(model);
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
