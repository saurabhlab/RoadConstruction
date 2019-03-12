using AutoMapper;
using RP.Common;
using RP.IRepository;
using RP.IServices;
using RP.Models;
using RP.Models.Admin;
using RP.Models.Common;
using RP.Models.Company;
using RP.Models.Master;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Common;
using RP.ViewModel.Company;
using RP.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;
using System.Security.Cryptography;
using System.Text;

namespace RP.Services
{
    public class CommonService : ICommonService
    {
        #region--Inject Dependency--
        private readonly IEntityBaseRepository<ItemMaster> itemMasterRepository;
        private readonly IEntityBaseRepository<MasterLookup> masterLookupRepository;
        private readonly IEntityBaseRepository<City> cityRepository;
        private readonly IEntityBaseRepository<Work> workRepository;
        private readonly IEntityBaseRepository<InwardOutward> inwardOutwardRepository;
        private readonly IEntityBaseRepository<UserTypeMapping> userTypeMappingRepository;
        private readonly IEntityBaseRepository<ItemOldQuantity> itemOldQuantityRepository;
        private readonly IEntityBaseRepository<ManagerSupervisorTypeMapping> managerSupervisorTypeMappingRepository;

        private readonly IUnitOfWork unitOfWork;
        ProcessedResponse processedResponse = new ProcessedResponse();
        ResponseViewModel response = new ResponseViewModel();
        public CommonService(IEntityBaseRepository<ItemMaster> ItemMasterRepository,
            IEntityBaseRepository<City> CityRepository,
            IEntityBaseRepository<MasterLookup> MasterLookupRepository,
            IEntityBaseRepository<Work> WorkRepository,
            IEntityBaseRepository<InwardOutward> InwardOutwardRepository,
             IEntityBaseRepository<ItemOldQuantity> ItemOldQuantityRepository,
            IEntityBaseRepository<UserTypeMapping> UserTypeMappingRepository,
            IEntityBaseRepository<ManagerSupervisorTypeMapping> ManagerSupervisorTypeMappingRepository,

        IUnitOfWork UnitOfWork)
        {
            itemMasterRepository = ItemMasterRepository;
            masterLookupRepository = MasterLookupRepository;
            cityRepository = CityRepository;
            unitOfWork = UnitOfWork;
            workRepository = WorkRepository;
            inwardOutwardRepository = InwardOutwardRepository;
            userTypeMappingRepository = UserTypeMappingRepository;
            itemOldQuantityRepository = ItemOldQuantityRepository;
            managerSupervisorTypeMappingRepository = ManagerSupervisorTypeMappingRepository;
        }

        #endregion

       
        public ResponseViewModel GetCityList()
        {
            try
            {
                List<CityVM> CityList = new List<CityVM>();
                var list = cityRepository.GetAll().ToList();

                if (list != null && list.Count > 0)
                {
                    CityList = Mapper.Map<List<City>, List<CityVM>>(list);
                }
                response.IsSuccess = true;
                response.Status = "SUCCESS";
                response.Message = "Cities Get successfully...";
                response.Content = CityList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }
        public ResponseViewModel GetManagerList()
        {
            try
            {
                long managerId = 2;
                List<ManagersListVM> ManagersList = new List<ManagersListVM>();
                var data = userTypeMappingRepository.GetAll().Where(x => x.UserType == managerId).ToList();
                if (data != null && data.Count > 0)
                {
                    data.ForEach(x =>
                    {
                        ManagersList.Add(new ManagersListVM()
                        {
                            UserId = x.UserId,
                            Name = x.Contacts != null ? x.Contacts.FirstName + " " + x.Contacts.LastName : "",
                            City = x.Contacts != null ? x.Contacts.City: 0
                        });
                    });
                }
                response.IsSuccess = true;
                response.Status = "SUCCESS";
                response.Message = "Managers List Get successfully...";
                response.Content = ManagersList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }
        public ResponseViewModel GeItemMasterList()
        {
            try
            {
                List<ItemMasterViewModel> MasterList = new List<ItemMasterViewModel>();
                var list = itemMasterRepository.GetAll().ToList();
                if (list != null && list.Count > 0)
                {
                    MasterList = Mapper.Map<List<ItemMaster>, List<ItemMasterViewModel>>(list);
                }
                response.IsSuccess = true;
                response.Status = "SUCCESS";
                response.Message = "Cities Get successfully...";
                response.Content = MasterList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }

        public ResponseViewModel GeMasterLookupList()
        {
            try
            {
                DropdownVm listDropDown = new DropdownVm();


                var list = masterLookupRepository.GetAll().ToList();
                if (list != null && list.Count > 0)
                {
                    List<DropDownFieldVM> MasterLookupList = new List<DropDownFieldVM>();
                    MasterLookupList = Mapper.Map<List<MasterLookup>, List<DropDownFieldVM>>(list);

                    listDropDown.Items = MasterLookupList.Where(x => x.GroupName == "Item").ToList();
                    listDropDown.Designations = MasterLookupList.Where(x => x.GroupName == "Designations"  && x.FieldName.ToLower() != "admin").ToList();
                    listDropDown.Units = MasterLookupList.Where(x => x.GroupName == "Unit").ToList();


                }

                response.IsSuccess = true;
                response.Status = "SUCCESS";
                response.Message = " Get dropdowndata successfully...";
                response.Content = listDropDown;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }


        public ResponseViewModel GetNextId(IdViewModel idViewModel)
        {
            try
            {
                long NextId = 1;
                if (!string.IsNullOrEmpty(idViewModel.FormType) && !string.IsNullOrEmpty(idViewModel.UserId))
                {
                    if (idViewModel.FormType.ToLower() == "workgenrate")
                    {
                        var data = workRepository.GetAll().Where(x => x.CreatedBy == idViewModel.UserId).ToList();
                        if (data != null && data.Count > 0)
                        {
                            NextId = Convert.ToInt64(data.Max(x => x.CaseNo)) + 1;
                        }
                    }
                    else if (idViewModel.FormType.ToLower() == "materialinward")
                    {
                        var data = inwardOutwardRepository.GetAll().Where(x => x.CreatedBy == idViewModel.UserId && x.IsInward).ToList();
                        if (data != null && data.Count > 0)
                        {
                            NextId = Convert.ToInt64(data.Max(x => x.InvoiceNo)) + 1;
                        }
                    }
                    else if (idViewModel.FormType.ToLower() == "materialoutward")
                    {
                        var data = inwardOutwardRepository.GetAll().Where(x => x.CreatedBy == idViewModel.UserId && !x.IsInward).ToList();
                        if (data != null && data.Count > 0)
                        {
                            NextId = Convert.ToInt64(data.Max(x => x.InvoiceNo)) + 1;
                        }
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get NextId successfully...";
                    //response.Content = NextId;
                    response.Id = NextId;
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }

        public ResponseViewModel GetOldQuantity(InwardOutwardVM inwardOutwardVM)
        {
            try
            {
                decimal OldQty = 0;
                if (!string.IsNullOrEmpty(inwardOutwardVM.CreatedBy) && inwardOutwardVM.ItemId > 0)
                {
                    var Data = itemOldQuantityRepository.FindBy(x => x.IsDeleted == false && x.CreatedBy == inwardOutwardVM.CreatedBy && x.ItemId == inwardOutwardVM.ItemId).FirstOrDefault();
                    //inwardOutwardRepository.FindBy(x => x.IsDeleted == false && x.CreatedBy == inwardOutwardVM.CreatedBy && x.ItemId == inwardOutwardVM.ItemId).FirstOrDefault();
                    if (Data != null && Data.Id > 0)
                    {
                        OldQty = Data.TotalQuantity;
                    }
                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Get Old Quantity successfully...";
                    response.Content = OldQty;
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }

        public ResponseViewModel GetSupervisorsByMgrId(IdViewModel idViewModel)
        {
            try
            {
                List<ManagersListVM> list = new List<ManagersListVM>();
                if (!string.IsNullOrEmpty(idViewModel.UserId))
                {
                    var data = managerSupervisorTypeMappingRepository.GetAll().Where(x => x.ManagerId == idViewModel.UserId).ToList();
                    if (data != null && data.Count > 0)
                    {
                        data.ForEach(x =>
                        {
                            var usertypedata = userTypeMappingRepository.FindBy(y => y.UserId == x.SupervisorId).FirstOrDefault();

                            list.Add(new ManagersListVM()
                            {
                                Name = (usertypedata != null && usertypedata.Contacts != null) ? usertypedata.Contacts.FirstName + " " + usertypedata.Contacts.LastName : "",
                                UserId = x.SupervisorId
                            });
                        });
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get Data successfully...";
                    response.Content = list;
                    //response.Id = NextId;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }

        public ResponseViewModel GetUserList(IdViewModel idViewModel)
        {
            try
            {
                Helper helper = new Helper();
                //******

              
                //    if (result != null)
                //    {
                //        //genrate pdf from html string
                //        //  var pathEmailpdf = ConfigurationManager.AppSettings["MainURL"];
                //        Guid guid = Guid.NewGuid();
                //        var downloadName = "WeeklyReport-" + guid;
                //        byte[] downloadBytes = null;
                //        downloadName += ".pdf";
                //        var pdfConverter = helper.GetPdfConverter();
                //        downloadBytes = pdfConverter.GetPdfBytesFromHtmlString(result);
                //        var memorystream = new MemoryStream(downloadBytes);

                //    }
                //}
                //***************************************



                ////Guid guid = Guid.NewGuid();
                ////var pathEmailpdf = ConfigurationManager.AppSettings["BaseURL"];
                ////var urlToConvert = pathEmailpdf + "Home/GetWorkReportPdf?UserId=" + idViewModel.UserId;
                //////var urlToConvert = pathEmailpdf + "SendBrochure/GetMarketAgencyContractData?ID=" + model.PropertyInfoId;
                ////var downloadName = "WorkReport-" + idViewModel.UserId;
                ////string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/Documents/");
                ////string fullpath = sPath + "\\" + guid + ".pdf";
                ////byte[] downloadBytes = null;
                ////downloadName += ".pdf";
                ////var pdfConverter = helper.GetPdfConverter();
                ////downloadBytes = pdfConverter.GetPdfBytesFromUrl(urlToConvert);
                //////string savepath = ConfigurationManager.AppSettings["PropertyDocumentsPath"];
                ////var savedFilePath = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"UploadFile\Documents") + @"\" + guid + ".pdf";
                ////var path = Path.GetDirectoryName(savedFilePath);
                ////if (path != null && !Directory.Exists(path))
                ////    Directory.CreateDirectory(path);
                ////using (FileStream filestream = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write))
                ////{
                ////    filestream.Write(downloadBytes, 0, downloadBytes.Length);
                ////    filestream.Close();
                ////}

                //Helper hpl = new Helper();
                //hpl.GeneratePDF(null);
                //***



                List<UserListVM> list = new List<UserListVM>();
                if (!string.IsNullOrEmpty(idViewModel.UserId))
                {
                    var data = userTypeMappingRepository.GetAll().Where(x=>x.UserType != 1).OrderByDescending(x=>x.Id).ToList();

                    if (data != null && data.Count > 0)
                    {

                        var CityList = cityRepository.GetAll().ToList();
                        var masterLookpData = masterLookupRepository.GetAll().ToList();

                        var DesignationList = masterLookpData.Where(x => x.GroupName == "Designations").ToList();

                        for (int i = 0; i < data.Count; i++)
                        {
                            list.Add(new UserListVM() {
                                Id = data[i].Id,
                                City = data[i].Contacts!= null && data[i].Contacts.City > 0 ? CityList.Where(x=>x.Id == data[i].Contacts.City).FirstOrDefault().CityName :"",
                                ContactNo = data[i].Contacts != null ? data[i].Contacts.ContactNo : "",
                                Designation = data[i].Contacts != null && data[i].Contacts.Designation > 0 ? DesignationList.Where(x=>x.Id == data[i].Contacts.Designation).FirstOrDefault().FieldName :"",
                                Email= data[i].Contacts != null ? data[i].Contacts.Email: "",
                                Name= data[i].Contacts != null ? data[i].Contacts.FirstName + " " + data[i].Contacts.LastName : "",
                                UserId =data[i].UserId

                            });
                        }
                        
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get Data successfully...";
                    response.Content = list;
                   
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }
        public ResponseViewModel GetDashboardCount(IdViewModel idViewModel)
        {
            try
            {
                DashboardCountVM CountVM = new DashboardCountVM();

                CountVM.ManagerCount = userTypeMappingRepository.FindBy(x => !x.IsDeleted && x.UserType == 2).ToList().Count();
                CountVM.SuperVisorCount = userTypeMappingRepository.FindBy(x => !x.IsDeleted && x.UserType == 3).ToList().Count();
                CountVM.EngineerCount = userTypeMappingRepository.FindBy(x => !x.IsDeleted && x.UserType == 4).ToList().Count();

                response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get Count successfully...";
                    response.Content = CountVM;
                   
               
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Message = "Something went wrong";
            }
            return response;
        }
    }
}