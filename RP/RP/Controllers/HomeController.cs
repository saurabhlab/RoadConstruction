using AutoMapper;
using RP.IRepository;
using RP.IServices;
using RP.Models;
using RP.Models.Master;
using RP.Repository;
using RP.ViewModel.Common;
using RP.ViewModel.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RP.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IEntityBaseRepository<ItemMaster> itemMasterRepository;
        //private IRepository<Student> studentRepository = null;
        //private IRepository<ItemMaster> itemMasterRepository = null;
        public HomeController()
        {
        }

        #region--Inject Dependency--
        ResponseViewModel responseViewModel = new ResponseViewModel();

        private readonly IUnitOfWork unitOfWork;
        private IInwardOutwardService iInwardOutwardService;
        public HomeController(
            IInwardOutwardService IInwardOutwardService,
        IUnitOfWork UnitOfWork
            )
        {
            this.iInwardOutwardService = IInwardOutwardService;
            this.unitOfWork = UnitOfWork;

        }

        #endregion

        //public HomeController(IEntityBaseRepository<ItemMaster> ItemMasterRepository)
        //{
        //    itemMasterRepository = ItemMasterRepository;
        //    //    // StudentRepository studentRepository = new StudentRepository();
        //    //    this.studentRepository = new Repository<Student>();
        //    //    this.itemMasterRepository = new Repository<ItemMaster>();
        //}

        //protected readonly StudentRepository studentRepository;

        public ActionResult Index()
        {
           // var t = itemMasterRepository.GetAll();

            //var tt = studentRepository.GetAll();
            //var y = 11;

            //var a = itemMasterRepository.GetById(3);
            //var b = itemMasterRepository.GetAll();

            //var std = new Student();

            //std.MobileNo = "684684";
            //std.StAddress = "ngp";
            //std.StName = "Devanand";
            //studentRepository.Insert(std);
            //studentRepository.Save();
            //var stdid = std.StId;

            var tt = 12;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        public ActionResult GetWorkReportPdf(string UserId)
        {
            List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
            //object WorkReportsList;
            try
            {
                           IdViewModel IDVM = new IdViewModel();
                IDVM.UserId = UserId;
                IDVM.FormType = "workreport";
                var list = iInwardOutwardService.GetWorkReportList(IDVM);
                if (list != null & list.IsSuccess && list.Content != null)
                {
                    //WorkReportsList =  list.Content;
                    WorkReportsList = (List<WorkReportsVM>)list.Content;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View(WorkReportsList);
        }
    }
}