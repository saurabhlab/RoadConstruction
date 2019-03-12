using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using RP.Common;
using RP.IRepository;
using RP.IServices;
using RP.Models;
using RP.Models.Account;
using RP.Models.Admin;
using RP.Models.Company;
using RP.Models.Master;
using RP.Models.Street;
using RP.ViewModel.Common;
using RP.ViewModel.Company;
using RP.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static RP.Common.Enums;
using static RP.Common.Helper;
using Image = iTextSharp.text.Image;

namespace RP.Services
{

    public class InwardOutwardService : IInwardOutwardService
    {
        #region--Inject Dependency--
        ResponseViewModel response = new ResponseViewModel();
        private readonly IEntityBaseRepository<InwardOutward> inwardOutwardRepository;
        private readonly IEntityBaseRepository<Work> workRepository;
        private readonly IEntityBaseRepository<MasterLookup> masterLookupRepository;
        private readonly IEntityBaseRepository<City> cityRepository;
        private readonly IEntityBaseRepository<ItemOldQuantity> itemOldQuantityRepository;
        private readonly IEntityBaseRepository<UserTypeMapping> userTypeMappingRepository;
        private readonly IEntityBaseRepository<ManagerSupervisorTypeMapping> managerSupervisorTypeMappingRepository;
        private readonly IEntityBaseRepository<Contact> contactRepository;
        private readonly IEntityBaseRepository<Street> streetRepository;
        private readonly IUnitOfWork unitOfWork;
        private ICommonService iCommonService;

        public InwardOutwardService(
            IEntityBaseRepository<InwardOutward> InwardOutwardRepository,
            IEntityBaseRepository<Street> StreetRepository,
        IEntityBaseRepository<Work> WorkRepository,
            IEntityBaseRepository<MasterLookup> MasterLookupRepository,
            IEntityBaseRepository<City> CityRepository,
            IEntityBaseRepository<Contact> ContactRepository,
            IEntityBaseRepository<UserTypeMapping> UserTypeMappingRepository,
        IEntityBaseRepository<ManagerSupervisorTypeMapping> ManagerSupervisorTypeMappingRepository,

        IEntityBaseRepository<ItemOldQuantity> ItemOldQuantityRepository,
        IUnitOfWork UnitOfWork,
            ICommonService ICommonService
        )
        {
            inwardOutwardRepository = InwardOutwardRepository;
            workRepository = WorkRepository;
            streetRepository = StreetRepository;
            masterLookupRepository = MasterLookupRepository;
            cityRepository = CityRepository;
            contactRepository = ContactRepository;
            itemOldQuantityRepository = ItemOldQuantityRepository;
            userTypeMappingRepository = UserTypeMappingRepository;
            managerSupervisorTypeMappingRepository = ManagerSupervisorTypeMappingRepository;
            unitOfWork = UnitOfWork;
            iCommonService = ICommonService;

        }

        #endregion


        public ResponseViewModel SaveInwardOutwardData(InwardOutwardVM inwardOutwardVM)

        {
            try
            {
                if (inwardOutwardVM != null)
                {
                    InwardOutward data = Mapper.Map<InwardOutwardVM, InwardOutward>(inwardOutwardVM);
                    inwardOutwardRepository.Add(data);
                    unitOfWork.Commit();

                    if (data.Id > 0 && !string.IsNullOrEmpty(inwardOutwardVM.CreatedBy) && inwardOutwardVM.ItemId > 0)
                    {
                        var OldQtyData = itemOldQuantityRepository.FindBy(x => x.ItemId == inwardOutwardVM.ItemId && x.IsDeleted == false && x.CreatedBy == inwardOutwardVM.CreatedBy).FirstOrDefault();
                        if (OldQtyData != null && OldQtyData.Id > 0)
                        {
                            if (inwardOutwardVM.IsInward)
                            {
                                OldQtyData.TotalQuantity = OldQtyData.TotalQuantity + inwardOutwardVM.Quantity;
                            }
                            else if (!inwardOutwardVM.IsInward)
                            {
                                OldQtyData.TotalQuantity = OldQtyData.TotalQuantity - inwardOutwardVM.Quantity;
                            }

                            itemOldQuantityRepository.Edit(OldQtyData, OldQtyData);
                            unitOfWork.Commit();

                        }
                        else
                        {
                            var ItemOldQty = new ItemOldQuantity();
                            ItemOldQty.TotalQuantity = inwardOutwardVM.Quantity;

                            ItemOldQty.ItemId = inwardOutwardVM.ItemId;
                            ItemOldQty.CreatedBy = inwardOutwardVM.CreatedBy;
                            itemOldQuantityRepository.Add(ItemOldQty);
                            unitOfWork.Commit();
                        }

                    }



                    response.Status = "SUCCESS";
                    response.IsSuccess = true;
                    response.Message = "Record Saved successfully...";
                    response.Content = null;

                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;


        }

        public ResponseViewModel SaveWorkGenerateData(WorkVM workVM)
        {
            try
            {
                if (workVM != null)
                {
                    workVM.BeforeDate = DateTime.Now;
                    var WorkIdData = this.GetWorkCaseNo(workVM);
                    if (WorkIdData.Content != null && WorkIdData.IsSuccess)
                    {
                        workVM.WorkId = WorkIdData.Content.ToString();
                    }

                    Work data = Mapper.Map<WorkVM, Work>(workVM);
                    workRepository.Add(data);
                    unitOfWork.Commit();

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = null;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel UpdateWorkCompleteData(WorkVM workVM)
        {
            try
            {
                if (workVM != null)
                {
                    var workdata = workRepository.GetSingle(workVM.Id);
                    if (workdata != null && workdata.Id > 0)
                    {
                        var newWorkData = workdata;

                        newWorkData.AfterDate = DateTime.Now;
                        newWorkData.WorkStatus = 12;
                        newWorkData.AfterImage = workVM.AfterImage;
                        newWorkData.Length = workVM.Length;
                        newWorkData.Width = workVM.Width;
                        newWorkData.Ladditude = workVM.Ladditude;
                        newWorkData.Longitude = workVM.Longitude;
                        newWorkData.CompletedBy = workVM.CompletedBy;


                        workRepository.Edit(workdata, newWorkData);
                        unitOfWork.Commit();
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Updated successfully...";
                    response.Content = null;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel GetMaterialReportsList(ReportFilterVM model)
        {
            try
            {
                //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
                List<InwardOutwardReportsVM> MaterialReportsList = new List<InwardOutwardReportsVM>();
                if (!string.IsNullOrEmpty(model.ManagerUserId))
                {
                    var data = new List<InwardOutward>();
                    var Dropdowndata = masterLookupRepository.GetAll().ToList();
                    var CityList = cityRepository.GetAll().ToList();
                    if (model.FormType.ToLower() == "inwardreport")
                    {
                        //data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true && x.CreatedBy == idViewModel.UserId).ToList();
                        data = inwardOutwardRepository.FindBy(x => x.IsInward && x.CreatedBy == model.ManagerUserId && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();

                        if (data != null && data.Count > 0 && model.FromDate != null)
                        {
                            data = data.Where(x => x.CreatedDate >= model.FromDate).ToList();
                        }
                        if (data != null && data.Count > 0 && model.ToDate != null)
                        {
                            data = data.Where(x => x.CreatedDate <= model.ToDate).ToList();
                        }
                    }
                    else if (model.FormType.ToLower() == "outwardreport")
                    {
                        //data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == false && x.CreatedBy == idViewModel.UserId).ToList();
                        data = inwardOutwardRepository.FindBy(x => !x.IsInward && x.CreatedBy == model.ManagerUserId && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();

                        if (data != null && data.Count > 0 && model.FromDate != null)
                        {
                            data = data.Where(x => x.CreatedDate >= model.FromDate).ToList();
                        }
                        if (data != null && data.Count > 0 && model.ToDate != null)
                        {
                            data = data.Where(x => x.CreatedDate <= model.ToDate).ToList();
                        }
                    }


                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {


                            MaterialReportsList.Add(new InwardOutwardReportsVM
                            {
                                Id = data[i].Id,
                                // CreatedDate = data[i].CreatedDate,
                                // CreatedBy = data[i].CreatedBy,
                                // IsInward = data[i].IsInward,
                                City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName,
                                InvoiceNo = "Invoice No " + data[i].InvoiceNo,
                                Item = Dropdowndata.Where(x => x.Id == data[i].ItemId).FirstOrDefault().FieldName,
                                Unit = Dropdowndata.Where(x => x.Id == data[i].Unit).FirstOrDefault().FieldName,
                                Quantity = data[i].Quantity,

                                CreatedDate = data[i].CreatedDate.ToShortDateString(),
                                //OldQuantity = data[i].,
                                // ReturnDate = data[i].ReturnDate,
                                SiteName = data[i].SiteName
                            });
                        }

                        // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    }
                    //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
                    //if (data != null && data.Count > 0)
                    //{
                    //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    //}


                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = MaterialReportsList;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = MaterialReportsList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        //public ResponseViewModel GetMaterialReportsList(IdViewModel idViewModel)
        //{
        //    try
        //    {
        //        //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
        //        List<InwardOutwardReportsVM> MaterialReportsList = new List<InwardOutwardReportsVM>();
        //        if (idViewModel.UserId != null)
        //        {
        //            var data = new List<InwardOutward>();
        //            var Dropdowndata = masterLookupRepository.GetAll().ToList();
        //            var CityList = cityRepository.GetAll().ToList();
        //            if (idViewModel.FormType.ToLower() == "inwardreport")
        //            {
        //                data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true && x.CreatedBy == idViewModel.UserId).ToList();
        //            }
        //            else if (idViewModel.FormType.ToLower() == "outwardreport")
        //            {
        //                data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == false && x.CreatedBy == idViewModel.UserId).ToList();
        //            }


        //            if (data != null && data.Count > 0)
        //            {
        //                for (int i = 0; i < data.Count; i++)
        //                {


        //                    MaterialReportsList.Add(new InwardOutwardReportsVM
        //                    {
        //                        //  Id = data[i].Id,
        //                        // CreatedDate = data[i].CreatedDate,
        //                        // CreatedBy = data[i].CreatedBy,
        //                        // IsInward = data[i].IsInward,
        //                        City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName,
        //                        InvoiceNo = "Invoice No " + data[i].InvoiceNo,
        //                        Item = Dropdowndata.Where(x => x.Id == data[i].ItemId).FirstOrDefault().FieldName,
        //                        Unit = Dropdowndata.Where(x => x.Id == data[i].Unit).FirstOrDefault().FieldName,
        //                        Quantity = data[i].Quantity,

        //                        CreatedDate = data[i].CreatedDate.ToShortDateString(),
        //                        //OldQuantity = data[i].,
        //                        // ReturnDate = data[i].ReturnDate,
        //                        SiteName = data[i].SiteName
        //                    });
        //                }

        //                // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
        //            }
        //            //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
        //            //if (data != null && data.Count > 0)
        //            //{
        //            //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
        //            //}


        //            response.IsSuccess = true;
        //            response.Status = "SUCCESS";
        //            response.Message = "Record Saved successfully...";
        //            response.Content = MaterialReportsList;
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Status = "FAILED";
        //            response.Message = "Someting Went Wrong!";
        //            response.Content = MaterialReportsList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Status = "FAILED";
        //        response.Content = null;
        //        if (response.ReturnMessage == null)
        //            response.ReturnMessage = new List<string>();
        //        response.ReturnMessage.Add(ex.Message);
        //    }
        //    return response;
        //}

        public ResponseViewModel GetAdminMaterialReportsList(ReportFilterVM model)
        {
            try
            {
                //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
                List<InwardOutwardReportsVM> MaterialReportsList = new List<InwardOutwardReportsVM>();
                if (model != null && !string.IsNullOrEmpty(model.FormType))
                {
                    var data = new List<InwardOutward>();
                    var Dropdowndata = masterLookupRepository.GetAll().ToList();
                    var CityList = cityRepository.GetAll().ToList();

                    if (model.FormType.ToLower() == "inwardreport")
                    {
                        data = inwardOutwardRepository.FindBy(x => x.IsInward == true && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();
                        if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                        {
                            data = data.Where(x => x.CreatedBy == model.ManagerUserId).ToList();
                        }
                        if (data != null && data.Count > 0 && model.FromDate != null)
                        {
                            data = data.Where(x => x.CreatedDate >= model.FromDate).ToList();
                        }
                        if (data != null && data.Count > 0 && model.ToDate != null)
                        {
                            data = data.Where(x => x.CreatedDate <= model.ToDate).ToList();
                        }


                        //if (idViewModel.CityId > 0)
                        //{
                        //    data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true && x.City == idViewModel.CityId).OrderByDescending(x => x.Id).ToList();
                        //}
                        //else
                        //{
                        //    data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).OrderByDescending(x => x.Id).ToList();
                        //}
                    }
                    if (model.FormType.ToLower() == "outwardreport")
                    {
                        data = inwardOutwardRepository.FindBy(x => x.IsInward == false && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();
                        if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                        {
                            data = data.Where(x => x.CreatedBy == model.ManagerUserId).ToList();
                        }
                        if (data != null && data.Count > 0 && model.FromDate != null)
                        {
                            data = data.Where(x => x.CreatedDate >= model.FromDate).ToList();
                        }
                        if (data != null && data.Count > 0 && model.ToDate != null)
                        {
                            data = data.Where(x => x.CreatedDate <= model.ToDate).ToList();
                        }

                        //if (idViewModel.CityId > 0)
                        //{
                        //    data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == false && x.City == idViewModel.CityId).OrderByDescending(x => x.Id).ToList();
                        //}
                        //else
                        //{
                        //    data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == false).OrderByDescending(x => x.Id).ToList();
                        //}
                    }

                    //if (idViewModel.FormType.ToLower() == "inwardreport")
                    //{
                    // data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true ).ToList();
                    //}
                    //else if (idViewModel.FormType.ToLower() == "outwardreport")
                    //{
                    //    data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == false ).ToList();
                    //}


                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            var createdByUserId = data[i].CreatedBy;
                            var contactData = contactRepository.FindBy(x => x.UserId == createdByUserId && !x.IsDeleted).FirstOrDefault();
                            MaterialReportsList.Add(new InwardOutwardReportsVM
                            {
                                Id = data[i].Id,
                                // CreatedDate = data[i].CreatedDate,
                                // CreatedBy = data[i].CreatedBy,
                                // IsInward = data[i].IsInward,
                                City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName,
                                InvoiceNo = "Invoice No " + data[i].InvoiceNo,
                                Item = Dropdowndata.Where(x => x.Id == data[i].ItemId).FirstOrDefault().FieldName,
                                Unit = Dropdowndata.Where(x => x.Id == data[i].Unit).FirstOrDefault().FieldName,
                                Quantity = data[i].Quantity,
                                CreatedBy = contactData != null ? (contactData.FirstName + " " + contactData.LastName) : "",
                                CreatedDate = data[i].CreatedDate.ToShortDateString(),
                                //OldQuantity = data[i].,
                                // ReturnDate = data[i].ReturnDate,
                                SiteName = data[i].SiteName
                            });
                        }

                        // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    }
                    //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
                    //if (data != null && data.Count > 0)
                    //{
                    //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    //}


                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = MaterialReportsList;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = MaterialReportsList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel GetWorkReportList(IdViewModel idViewModel)
        {
            try
            {
                //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
                List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
                if (!string.IsNullOrEmpty(idViewModel.UserId) && !string.IsNullOrEmpty(idViewModel.FormType))
                {
                    var data = new List<Work>();
                    //var WorkStstusList = masterLookupRepository.GetAll().Where(x=>x.GroupName.ToLower() == "workstatus").ToList();
                    var CityList = cityRepository.GetAll().ToList();

                    if (idViewModel.FormType.ToLower() == "workreport")
                    {
                        //data = workRepository.GetAll().Where(x => (x.CreatedBy == idViewModel.UserId || x.AssignTo == idViewModel.UserId) && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();

                        //if (idViewModel.Worktype == 11)
                        //{

                        //    // var predicate = PredicateBuilder.New<Work>();
                        //    data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                        //    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                        //    {
                        //        data = data.Where(x => x.CreatedBy == model.ManagerUserId).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.FromDate != null)
                        //    {
                        //        data = data.Where(x => x.BeforeDate >= model.FromDate).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.ToDate != null)
                        //    {
                        //        data = data.Where(x => x.BeforeDate <= model.ToDate).ToList();
                        //    }

                        //    //data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype && (x.BeforeDate >= model.FromDate && x.BeforeDate <= model.ToDate )|| x.CreatedBy == model.ManagerUserId).OrderByDescending(x => x.Id).ToList();
                        //}
                        //else if (model.Worktype == 12)
                        //{
                        //    data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                        //    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                        //    {
                        //        data = data.Where(x => x.CompletedBy == model.ManagerUserId).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.FromDate != null)
                        //    {
                        //        data = data.Where(x => x.AfterDate >= model.FromDate).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.ToDate != null)
                        //    {
                        //        data = data.Where(x => x.AfterDate <= model.ToDate).ToList();
                        //    }

                        //}
                        //else if (model.Worktype == 13)
                        //{
                        //    data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                        //    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                        //    {
                        //        data = data.Where(x => x.RegenrateBy == model.ManagerUserId).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.FromDate != null)
                        //    {
                        //        data = data.Where(x => x.RegenrateDate >= model.FromDate).ToList();
                        //    }
                        //    if (data != null && data.Count > 0 && model.ToDate != null)
                        //    {
                        //        data = data.Where(x => x.RegenrateDate <= model.ToDate).ToList();
                        //    }
                        //}

                    }
                    else if (idViewModel.FormType.ToLower() == "workassign")
                    {
                        data = workRepository.GetAll().Where(x => x.CreatedBy == idViewModel.UserId && (x.AssignTo == "" || x.AssignTo == null) && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
                    }
                    else if (idViewModel.FormType.ToLower() == "workpdf")
                    {
                        data = workRepository.GetAll().Where(x => (x.CreatedBy == idViewModel.UserId || x.AssignTo == idViewModel.UserId) && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
                    }

                    //var tt = workRepository.GetAll().ToList();

                    //***

                    //***
                    Helper help = new Helper();
                    //var data = help.ImageToBase64();

                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            long streetid = Convert.ToInt64(data[i].CaseSite);
                            var streetData = streetRepository.FindBy(x => x.Id == streetid && !x.IsDeleted).FirstOrDefault();

                            WorkReportsList.Add(new WorkReportsVM
                            {
                                Id = data[i].Id,
                                WorkId = data[i].WorkId,
                                CaseSite = streetData != null ? streetData.StreetName : "",
                                AfterDate = data[i].AfterDate,
                                BeforeDate = data[i].BeforeDate,
                                CaseNo = data[i].CaseNo,
                                AfterImage = data[i].AfterImage,
                                BeforeImage = data[i].BeforeImage,
                                Area = data[i].Length * data[i].Width,
                                RegenrateBy = data[i].RegenrateBy,
                                RegenrateDate = data[i].RegenrateDate,
                                //  AfterImage = "data:image/jpg;base64,"+ help.ImageToBase64(data[i].AfterImage),
                                // BeforeImage = "data:image/jpg;base64," + help.ImageToBase64(data[i].BeforeImage),
                                CreatedBy = data[i].CreatedBy,
                                Date = data[i].CreatedDate.ToShortDateString(),
                                Ladditude = data[i].Ladditude,
                                Landmark = data[i].Landmark,
                                Length = data[i].Length,
                                Longitude = data[i].Longitude,
                                Width = data[i].Width,
                                //WorkStatus = data[i].WorkStatus,
                                // WorkStatus = (WorkStstusList !=null || WorkStstusList.Count > 0) ? WorkStstusList.Where(x=>x.Id == data[i].WorkStatus).FirstOrDefault().FieldName : "",
                                City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName

                            });
                        }

                        // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    }
                    //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
                    //if (data != null && data.Count > 0)
                    //{
                    //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    //}

                    response.Id = (long)WorkReportsList.Sum(x => x.Area);
                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = WorkReportsList;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = WorkReportsList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public ResponseViewModel GetWorkReportListForManager(ReportFilterVM model)
        {
            try
            {
                //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
                List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
                if (model != null && !string.IsNullOrEmpty(model.ManagerUserId) && !string.IsNullOrEmpty(model.FormType))
                {
                    var data = new List<Work>();
                    //var WorkStstusList = masterLookupRepository.GetAll().Where(x=>x.GroupName.ToLower() == "workstatus").ToList();
                    var CityList = cityRepository.GetAll().ToList();

                    if (model.FormType.ToLower() == "workreport")
                    {
                        //data = workRepository.GetAll().Where(x => (x.CreatedBy == idViewModel.UserId || x.AssignTo == idViewModel.UserId) && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();

                        //get city for Engineer
                        if (model.UserType == (long)UserType.Engineer)
                        {
                            var engData = contactRepository.FindBy(x => !x.IsDeleted && x.UserId == model.ManagerUserId).FirstOrDefault();
                            if (engData != null && engData.Id > 0 && engData.City > 0)
                            {
                                if (model.Worktype == 11)
                                {
                                    data = workRepository.FindBy(x => !x.IsDeleted && x.WorkStatus == model.Worktype && x.City == engData.City).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate <= model.ToDate).ToList();
                                    }

                                    //data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype && (x.BeforeDate >= model.FromDate && x.BeforeDate <= model.ToDate )|| x.CreatedBy == model.ManagerUserId).OrderByDescending(x => x.Id).ToList();
                                }
                                else if (model.Worktype == 12)
                                {
                                    data = workRepository.FindBy(x => x.City == engData.City && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate <= model.ToDate).ToList();
                                    }

                                }
                                else if (model.Worktype == 13)
                                {
                                    data = workRepository.FindBy(x => x.City == engData.City && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate <= model.ToDate).ToList();
                                    }
                                }
                            }
                        }

                        //get work list for manager or supervisor
                        else if (model.UserType == (long)UserType.Manager)
                        {
                            IdViewModel IDVM = new IdViewModel();
                            IDVM.UserId = model.ManagerUserId;
                            // var predicate = PredicateBuilder.New<Work>();
                            List<ManagersListVM> supervisorList = (List<ManagersListVM>)iCommonService.GetSupervisorsByMgrId(IDVM).Content;

                            if (model.Worktype == 11)
                            {

                                if (supervisorList != null && supervisorList.Count() > 0)
                                {
                                    supervisorList.ForEach(y =>
                                    {
                                        var supervisorWorkList = workRepository.FindBy(x => !x.IsDeleted && x.WorkStatus == model.Worktype && (x.CreatedBy == y.UserId || x.AssignTo == y.UserId)).OrderByDescending(x => x.Id).ToList();
                                        if (supervisorWorkList != null && supervisorWorkList.Count > 0)
                                        {
                                            data.AddRange(supervisorWorkList);
                                        }
                                    });

                                    //data =
                                    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.SupervisorUserId))
                                    {
                                        data = data.Where(x => x.CreatedBy == model.SupervisorUserId || x.AssignTo == model.SupervisorUserId).OrderByDescending(x => x.Id).ToList();
                                    }

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate <= model.ToDate).ToList();
                                    }
                                }

                                //data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype && (x.BeforeDate >= model.FromDate && x.BeforeDate <= model.ToDate )|| x.CreatedBy == model.ManagerUserId).OrderByDescending(x => x.Id).ToList();
                            }
                            else if (model.Worktype == 12)
                            {
                                if (supervisorList != null && supervisorList.Count() > 0)
                                {
                                    supervisorList.ForEach(y =>
                                    {
                                        var supervisorWorkList = workRepository.FindBy(x => !x.IsDeleted && x.WorkStatus == model.Worktype && x.CompletedBy == y.UserId).OrderByDescending(x => x.Id).ToList();
                                        if (supervisorWorkList != null && supervisorWorkList.Count > 0)
                                        {
                                            data.AddRange(supervisorWorkList);
                                        }
                                    });

                                    //data =
                                    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.SupervisorUserId))
                                    {
                                        data = data.Where(x => x.CompletedBy == model.SupervisorUserId).OrderByDescending(x => x.Id).ToList();
                                    }
                                    // data = workRepository.FindBy(x => x.CompletedBy == model.SupervisorUserId && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate <= model.ToDate).ToList();
                                    }

                                }
                            }
                            else if (model.Worktype == 13)
                            {
                                if (supervisorList != null && supervisorList.Count() > 0)
                                {
                                    // data = workRepository.FindBy(x => x.RegenrateBy == model.SupervisorUserId && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                                    supervisorList.ForEach(y =>
                                {
                                    var supervisorWorkList = workRepository.FindBy(x => !x.IsDeleted && x.WorkStatus == model.Worktype && x.RegenrateBy == y.UserId).OrderByDescending(x => x.Id).ToList();
                                    if (supervisorWorkList != null && supervisorWorkList.Count > 0)
                                    {
                                        data.AddRange(supervisorWorkList);
                                    }
                                });

                                    //data =
                                    if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.SupervisorUserId))
                                    {
                                        data = data.Where(x => x.RegenrateBy == model.SupervisorUserId).OrderByDescending(x => x.Id).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate <= model.ToDate).ToList();
                                    }
                                }
                            }

                            }

                            else if (model.UserType == (long)UserType.Supervisor)
                            {
                                if (model.Worktype == 11)
                                {

                                    // var predicate = PredicateBuilder.New<Work>();


                                    data = workRepository.FindBy(x => !x.IsDeleted && x.WorkStatus == model.Worktype && (x.CreatedBy == model.ManagerUserId || x.AssignTo == model.ManagerUserId)).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.BeforeDate <= model.ToDate).ToList();
                                    }

                                    //data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype && (x.BeforeDate >= model.FromDate && x.BeforeDate <= model.ToDate )|| x.CreatedBy == model.ManagerUserId).OrderByDescending(x => x.Id).ToList();
                                }
                                else if (model.Worktype == 12)
                                {
                                    data = workRepository.FindBy(x => x.CompletedBy == model.ManagerUserId && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.AfterDate <= model.ToDate).ToList();
                                    }

                                }
                                else if (model.Worktype == 13)
                                {
                                    data = workRepository.FindBy(x => x.RegenrateBy == model.ManagerUserId && x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();

                                    if (data != null && data.Count > 0 && model.FromDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate >= model.FromDate).ToList();
                                    }
                                    if (data != null && data.Count > 0 && model.ToDate != null)
                                    {
                                        data = data.Where(x => x.RegenrateDate <= model.ToDate).ToList();
                                    }
                                }

                            }


                        }


                        //var tt = workRepository.GetAll().ToList();

                        //***

                        //***
                        Helper help = new Helper();
                        //var data = help.ImageToBase64();
                        var WorkstatusList = masterLookupRepository.GetAll().Where(x => x.GroupName == "WorkStatus").ToList();

                        if (data != null && data.Count > 0)
                        {
                            for (int i = 0; i < data.Count; i++)
                            {
                                long streetid = Convert.ToInt64(data[i].CaseSite);
                                var streetData = streetRepository.FindBy(x => x.Id == streetid && !x.IsDeleted).FirstOrDefault();


                                WorkReportsList.Add(new WorkReportsVM
                                {
                                    Id = data[i].Id,
                                    // CaseSite = data[i].CaseSite,
                                    WorkId = data[i].WorkId,
                                    CaseSite = streetData != null ? streetData.StreetName : "",
                                    AfterDate = data[i].AfterDate,
                                    BeforeDate = data[i].BeforeDate,
                                    CaseNo = data[i].CaseNo,
                                    AfterImage = data[i].AfterImage,
                                    BeforeImage = data[i].BeforeImage,
                                    Area = data[i].Length * data[i].Width,
                                    RegenrateBy = data[i].RegenrateBy,
                                    RegenrateDate = data[i].RegenrateDate,
                                    //  AfterImage = "data:image/jpg;base64,"+ help.ImageToBase64(data[i].AfterImage),
                                    // BeforeImage = "data:image/jpg;base64," + help.ImageToBase64(data[i].BeforeImage),
                                    CreatedBy = data[i].CreatedBy,
                                    Date = data[i].CreatedDate.ToShortDateString(),
                                    Ladditude = data[i].Ladditude,
                                    Landmark = data[i].Landmark,
                                    Length = data[i].Length,
                                    Longitude = data[i].Longitude,
                                    Width = data[i].Width,
                                    WorkStatus = WorkstatusList.Where(x => x.Id == data[i].WorkStatus).FirstOrDefault() != null ? WorkstatusList.Where(x => x.Id == data[i].WorkStatus).FirstOrDefault().FieldName.ToString() : "",
                                    // WorkStatus = (WorkStstusList !=null || WorkStstusList.Count > 0) ? WorkStstusList.Where(x=>x.Id == data[i].WorkStatus).FirstOrDefault().FieldName : "",
                                    City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName

                                });
                            }

                            // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                        }
                        //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
                        //if (data != null && data.Count > 0)
                        //{
                        //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                        //}



                        response.Id = (long)WorkReportsList.Sum(x => x.Area);
                        response.IsSuccess = true;
                        response.Status = "SUCCESS";
                        response.Message = "Record Saved successfully...";
                        response.Content = WorkReportsList;

                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Status = "FAILED";
                        response.Message = "Someting Went Wrong!";
                        response.Content = WorkReportsList;
                    }
                }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel GetAdminWorkReportList(ReportFilterVM model)
        {
            try
            {
                //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
                List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
                if (!string.IsNullOrEmpty(model.FormType))
                {
                    var data = new List<Work>();
                    //var WorkStstusList = masterLookupRepository.GetAll().Where(x=>x.GroupName.ToLower() == "workstatus").ToList();
                    var CityList = cityRepository.GetAll().ToList();

                    if (model != null && model.FormType.ToLower() == "workreport")
                    {


                        if (model.Worktype == 11)
                        {

                            // var predicate = PredicateBuilder.New<Work>();
                            data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                            {
                                data = data.Where(x => x.CreatedBy == model.ManagerUserId).ToList();
                            }
                            if (data != null && data.Count > 0 && model.FromDate != null)
                            {
                                data = data.Where(x => x.BeforeDate >= model.FromDate).ToList();
                            }
                            if (data != null && data.Count > 0 && model.ToDate != null)
                            {
                                data = data.Where(x => x.BeforeDate <= model.ToDate).ToList();
                            }

                            //data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype && (x.BeforeDate >= model.FromDate && x.BeforeDate <= model.ToDate )|| x.CreatedBy == model.ManagerUserId).OrderByDescending(x => x.Id).ToList();
                        }
                        else if (model.Worktype == 12)
                        {
                            data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                            {
                                data = data.Where(x => x.CompletedBy == model.ManagerUserId).ToList();
                            }
                            if (data != null && data.Count > 0 && model.FromDate != null)
                            {
                                data = data.Where(x => x.AfterDate >= model.FromDate).ToList();
                            }
                            if (data != null && data.Count > 0 && model.ToDate != null)
                            {
                                data = data.Where(x => x.AfterDate <= model.ToDate).ToList();
                            }

                        }
                        else if (model.Worktype == 13)
                        {
                            data = workRepository.GetAll().Where(x => x.WorkStatus == model.Worktype).OrderByDescending(x => x.Id).ToList();
                            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(model.ManagerUserId))
                            {
                                data = data.Where(x => x.RegenrateBy == model.ManagerUserId).ToList();
                            }
                            if (data != null && data.Count > 0 && model.FromDate != null)
                            {
                                data = data.Where(x => x.RegenrateDate >= model.FromDate).ToList();
                            }
                            if (data != null && data.Count > 0 && model.ToDate != null)
                            {
                                data = data.Where(x => x.RegenrateDate <= model.ToDate).ToList();
                            }
                        }
                    }
                    //else if (model.FormType.ToLower() == "workassign")
                    //{
                    //    if (model.CityId > 0)
                    //    {
                    //        data = workRepository.GetAll().Where(x => (x.AssignTo == "" || x.AssignTo == null) && x.City == idViewModel.CityId && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
                    //    }
                    //    else
                    //    {
                    //        data = workRepository.GetAll().Where(x => (x.AssignTo == "" || x.AssignTo == null) && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
                    //    }

                    //}
                    //var tt = workRepository.GetAll().ToList();

                    //***

                    //***
                    Helper help = new Helper();
                    //var data = help.ImageToBase64();

                    if (data != null && data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            var workCreatedByUserId = "";
                            var workCreatedDate = "";
                            if (data[i].WorkStatus == 11)
                            {
                                workCreatedByUserId = data[i].CreatedBy;
                                workCreatedDate = Convert.ToDateTime(data[i].BeforeDate).ToShortDateString();
                            }
                            else if (data[i].WorkStatus == 12)
                            {
                                workCreatedByUserId = data[i].CompletedBy;
                                workCreatedDate = Convert.ToDateTime(data[i].AfterDate).ToShortDateString();
                            }
                            else if (data[i].WorkStatus == 13)
                            {
                                workCreatedByUserId = data[i].RegenrateBy;
                                workCreatedDate = Convert.ToDateTime(data[i].RegenrateDate).ToShortDateString();
                            }

                            // var imgData = help.ImageToBase64(data[i].BeforeImage);
                            var contactData = contactRepository.FindBy(x => x.UserId == workCreatedByUserId && !x.IsDeleted).FirstOrDefault();
                            var WorkstatusList = masterLookupRepository.GetAll().Where(x => x.GroupName == "WorkStatus").ToList();
                            // var imgData = help.ImageToBase64(data[i].BeforeImage);
                            long streetid = Convert.ToInt64(data[i].CaseSite);
                            var streetData = streetRepository.FindBy(x => x.Id == streetid && !x.IsDeleted).FirstOrDefault();

                            WorkReportsList.Add(new WorkReportsVM
                            {
                                Id = data[i].Id,
                                WorkId = data[i].WorkId,
                                CaseSite = streetData != null ? streetData.StreetName : "",
                                //  CaseSite = data[i].CaseSite,
                                AfterDate = data[i].AfterDate,
                                BeforeDate = data[i].BeforeDate,
                                CaseNo = data[i].CaseNo,
                                AfterImage = data[i].AfterImage,
                                BeforeImage = data[i].BeforeImage,
                                Area = data[i].Length * data[i].Width,
                                RegenrateBy = data[i].RegenrateBy,
                                RegenrateDate = data[i].RegenrateDate,
                                //  AfterImage = "data:image/jpg;base64,"+ help.ImageToBase64(data[i].AfterImage),
                                // BeforeImage = "data:image/jpg;base64," + help.ImageToBase64(data[i].BeforeImage),
                                CreatedBy = contactData != null ? (contactData.FirstName + " " + contactData.LastName) : "",
                                Date = workCreatedDate,
                                Ladditude = data[i].Ladditude,
                                Landmark = data[i].Landmark,
                                Length = data[i].Length,
                                Longitude = data[i].Longitude,
                                Width = data[i].Width,
                                WorkStatus = WorkstatusList.Where(x => x.Id == data[i].WorkStatus).FirstOrDefault() != null ? WorkstatusList.Where(x => x.Id == data[i].WorkStatus).FirstOrDefault().FieldName.ToString() : "",
                                // WorkStatus = (WorkStstusList !=null || WorkStstusList.Count > 0) ? WorkStstusList.Where(x=>x.Id == data[i].WorkStatus).FirstOrDefault().FieldName : "",
                                City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName

                            });
                        }

                        // MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    }
                    //var data = inwardOutwardRepository.GetAll().Where(x => x.IsInward == true).ToList();
                    //if (data != null && data.Count > 0)
                    //{
                    //    MaterialList = Mapper.Map<List<InwardOutward>, List<InwardOutwardVM>>(data);
                    //}

                    response.Id = (long)WorkReportsList.Sum(x => x.Area);
                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = WorkReportsList;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = WorkReportsList;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        //public ResponseViewModel GetAdminWorkReportList(IdViewModel idViewModel)
        //{
        //    try
        //    {
        //        //List<InwardOutwardVM> MaterialList = new List<InwardOutwardVM>();
        //        List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
        //        if (!string.IsNullOrEmpty(idViewModel.FormType))
        //        {
        //            var data = new List<Work>();
        //            //var WorkStstusList = masterLookupRepository.GetAll().Where(x=>x.GroupName.ToLower() == "workstatus").ToList();
        //            var CityList = cityRepository.GetAll().ToList();

        //            if (idViewModel.FormType.ToLower() == "workreport")
        //            {
        //                if (idViewModel.CityId > 0)
        //                {
        //                    data = workRepository.GetAll().Where(x => x.WorkStatus == idViewModel.Id && x.City == idViewModel.CityId && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();


        //                }
        //                else
        //                {
        //                    data = workRepository.GetAll().Where(x => x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
        //                }
        //            }
        //            else if (idViewModel.FormType.ToLower() == "workassign")
        //            {
        //                if (idViewModel.CityId > 0)
        //                {
        //                    data = workRepository.GetAll().Where(x => (x.AssignTo == "" || x.AssignTo == null) && x.City == idViewModel.CityId && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
        //                }
        //                else
        //                {
        //                    data = workRepository.GetAll().Where(x => (x.AssignTo == "" || x.AssignTo == null) && x.WorkStatus == idViewModel.Id && x.IsDeleted == false).OrderByDescending(x => x.Id).ToList();
        //                }

        //            }
        //            Helper help = new Helper();
        //            //var data = help.ImageToBase64();

        //            if (data != null && data.Count > 0)
        //            {
        //                for (int i = 0; i < data.Count; i++)
        //                {
        //                    var workCreatedByUserId = "";
        //                    var workCreatedDate = "";
        //                    if (data[i].WorkStatus == 11)
        //                    {
        //                        workCreatedByUserId = data[i].CreatedBy;
        //                        workCreatedDate = Convert.ToDateTime(data[i].BeforeDate).ToShortDateString();
        //                    }
        //                    else if (data[i].WorkStatus == 12)
        //                    {
        //                        workCreatedByUserId = data[i].CompletedBy;
        //                        workCreatedDate = Convert.ToDateTime(data[i].AfterDate).ToShortDateString();
        //                    }
        //                    else if (data[i].WorkStatus == 13)
        //                    {
        //                        workCreatedByUserId = data[i].RegenrateBy;
        //                        workCreatedDate = Convert.ToDateTime(data[i].RegenrateDate).ToShortDateString();
        //                    }

        //                    // var imgData = help.ImageToBase64(data[i].BeforeImage);
        //                    var contactData = contactRepository.FindBy(x => x.UserId == workCreatedByUserId && !x.IsDeleted).FirstOrDefault();

        //                    // var imgData = help.ImageToBase64(data[i].BeforeImage);

        //                    WorkReportsList.Add(new WorkReportsVM
        //                    {
        //                        Id = data[i].Id,
        //                        CaseSite = data[i].CaseSite,
        //                        AfterDate = data[i].AfterDate,
        //                        BeforeDate = data[i].BeforeDate,
        //                        CaseNo = data[i].CaseNo,
        //                        AfterImage = data[i].AfterImage,
        //                        BeforeImage = data[i].BeforeImage,
        //                        Area = data[i].Length * data[i].Width,
        //                        //  AfterImage = "data:image/jpg;base64,"+ help.ImageToBase64(data[i].AfterImage),
        //                        // BeforeImage = "data:image/jpg;base64," + help.ImageToBase64(data[i].BeforeImage),
        //                        CreatedBy = contactData != null ? (contactData.FirstName + " " + contactData.LastName) : "",
        //                        Date = workCreatedDate,
        //                        Ladditude = data[i].Ladditude,
        //                        Landmark = data[i].Landmark,
        //                        Length = data[i].Length,
        //                        Longitude = data[i].Longitude,
        //                        Width = data[i].Width,
        //                        WorkStatus = data[i].WorkStatus,
        //                        // WorkStatus = (WorkStstusList !=null || WorkStstusList.Count > 0) ? WorkStstusList.Where(x=>x.Id == data[i].WorkStatus).FirstOrDefault().FieldName : "",
        //                        City = CityList.Where(x => x.Id == data[i].City).FirstOrDefault().CityName

        //                    });
        //                }


        //            }


        //            response.IsSuccess = true;
        //            response.Status = "SUCCESS";
        //            response.Message = "Record Saved successfully...";
        //            response.Content = WorkReportsList;
        //        }
        //        else
        //        {
        //            response.IsSuccess = false;
        //            response.Status = "FAILED";
        //            response.Message = "Someting Went Wrong!";
        //            response.Content = WorkReportsList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Status = "FAILED";
        //        response.Content = null;
        //        if (response.ReturnMessage == null)
        //            response.ReturnMessage = new List<string>();
        //        response.ReturnMessage.Add(ex.Message);
        //    }
        //    return response;
        //}
        public ResponseViewModel GetWorkDetailsById(IdViewModel idViewModel)
        {
            try
            {
                var data = workRepository.GetSingle(idViewModel.Id);
                //this.GenrateWorkReportPdf(idViewModel);
                if (data != null && data.Id > 0)
                {
                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    response.Content = data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }


        public ResponseViewModel AssignWork(AssignWorkVM assignWorkVM)
        {
            try
            {
                if (!string.IsNullOrEmpty(assignWorkVM.UserId) && assignWorkVM.WorkReportsVMList.Count > 0)
                {

                    assignWorkVM.WorkReportsVMList.ForEach(x =>
                    {

                        if (x.IsAssign)
                        {
                            var workData = workRepository.GetSingle(x.Id);
                            if (workData != null && workData.Id > 0)
                            {
                                workData.AssignTo = assignWorkVM.UserId;
                                workRepository.Edit(workData, workData);
                                unitOfWork.Commit();
                            }
                        }

                    });


                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    // response.Content = data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }


        public ResponseViewModel GenrateWorkReportPdf(PdfReportVM pdfReportVM)
        {
            try
            {

                //***

                List<WorkReportsVM> WorkReportsList = new List<WorkReportsVM>();
                WorkReportsList = pdfReportVM.WorkReportsList;
                ////var Workstatus = masterLookupRepository.GetAll().Where(x => x.GroupName == "WorkStatus").ToList();
                var ContactData = contactRepository.FindBy(x => x.UserId == pdfReportVM.UserId).FirstOrDefault();
                ////IdViewModel IDVM = new IdViewModel();
                ////IDVM.Id = idViewModel.Id;
                ////IDVM.UserId = idViewModel.UserId;
                ////IDVM.FormType = "workpdf";
                ////var list = this.GetWorkReportList(IDVM);

                ////if (idViewModel.UserType == 1)
                ////{
                ////   // WorkReportsList = (List<WorkReportsVM>)this.GetAdminWorkReportList(idViewModel).Content;
                ////}
                ////else
                ////{
                ////    WorkReportsList = (List<WorkReportsVM>)this.GetWorkReportList(idViewModel).Content;
                ////}




                if (WorkReportsList != null && WorkReportsList.Count > 0)
                {
                    //string pdfpath = HttpContext.Current.Server.MapPath("PDFs");
                    //string imagepath2 = HttpContext.Current.Server.MapPath("UploadFile/WorkPhotos");

                    //var imagepath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadFile/WorkPhotos");
                    //******************
                    ////byte[] bytes2;
                    ////using (StringWriter sw = new StringWriter())
                    ////{
                    ////    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    ////    {
                    ////        StringBuilder sbb = new StringBuilder();

                    ////        sbb.Append("<table  border = '0.5' cellspacing='0' cellpadding='2' width='100%'>");
                    ////        sbb.Append("<tr>");

                    ////        //Resize image depend upon your need
                    ////        Image png2 = Image.GetInstance(imagepath + "\\046e7b20-9167-4038-9c4a-8c0dd55e166c.jpg");
                    ////        png2.ScaleToFit(140f, 120f);

                    ////        //Give space before image

                    ////        png2.SpacingBefore = 10f;

                    ////        //Give some space after the image

                    ////        png2.SpacingAfter = 1f;

                    ////        png2.Alignment = Element.ALIGN_LEFT;

                    ////        sbb.Append("<td style='float:left;'> " + png2 + " </td>");

                    ////        sbb.Append("<td style='float:right;'>");
                    ////        sbb.Append("web link services pvt ltd");
                    ////        sbb.Append("</td>");
                    ////        sbb.Append("</tr>");



                    ////        sbb.Append(" </table>");

                    ////        //Export HTML String as PDF.
                    ////        StringReader srr = new StringReader(sbb.ToString());
                    ////        Document pdfDocc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    ////        HTMLWorker htmlparser2 = new HTMLWorker(pdfDocc);
                    ////        //PdfWriter writer = PdfWriter.GetInstance(pdfDocc, Response.OutputStream);
                    ////        //pdfDocc.Open();
                    ////        //htmlparser2.Parse(srr);


                    ////       // pdfDocc.Close();

                    ////        //---
                    ////        using (MemoryStream memoryStream2 = new MemoryStream())
                    ////        {
                    ////            PdfWriter writer2 = PdfWriter.GetInstance(pdfDocc, memoryStream2);
                    ////            pdfDocc.Open();

                    ////            //pdfDoc.Add(new Paragraph("JPG"));
                    ////            //Image gif = Image.GetInstance(imagepath + "\\046e7b20-9167-4038-9c4a-8c0dd55e166c.jpg");
                    ////            //gif.ScalePercent(24f);
                    ////            //gif.SetAbsolutePosition(pdfDoc.PageSize.Width - 36f - 72f,
                    ////            //pdfDoc.PageSize.Height - 36f - 216.6f);
                    ////            //pdfDoc.Add(gif);

                    ////            htmlparser2.Parse(srr);
                    ////            pdfDocc.Close();

                    ////            bytes2 = memoryStream2.ToArray();
                    ////            memoryStream2.Close();
                    ////        }
                    ////        Guid guid2 = Guid.NewGuid();
                    ////        var downloadName2 = "WorkReport-" + guid2;
                    ////        byte[] downloadBytes2 = null;
                    ////        downloadName2 += ".pdf";

                    ////        var savedFilePath2 = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Document\WorkReport\") + downloadName2;
                    ////        var path2 = Path.GetDirectoryName(savedFilePath2);
                    ////        if (path2 != null && !Directory.Exists(path2))
                    ////            Directory.CreateDirectory(path2);

                    ////        var file2 = new FileStream(savedFilePath2, FileMode.Create, FileAccess.Write);
                    ////        //file.Write(downloadBytes, 0, downloadBytes.Length);
                    ////        file2.Write(bytes2, 0, bytes2.Length);
                    ////        file2.Close();
                    ////        //Response.ContentType = "application/pdf";
                    ////        //Response.AddHeader("content-disposition", "attachment;filename=Invoice.pdf");
                    ////        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    ////        //Response.Write(pdfDocc);
                    ////        //Response.End();
                    ////     }
                    ////}


                    //*********************

                    //WorkReportsList =  list.Content;
                    ////WorkReportsList = (List<WorkReportsVM>)list.Content;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<h1 class='tst'> Work Report </h1>");
                    sb.Append("<div color='red' text-align='right'> Date:- " + DateTime.Now.ToShortDateString() + "</div>");
                    sb.Append("<div> Hello " + ContactData.FirstName + " " + ContactData.LastName + ",</div>");
                    sb.Append("<div> This is your work report. </div>");
                    sb.Append("<div>. </div>");
                    //Image png = Image.GetInstance(imagepath + "\\046e7b20-9167-4038-9c4a-8c0dd55e166c.jpg");
                    //png.ScaleToFit(140f, 120f);
                    //png.SpacingBefore = 10f;
                    //png.SpacingAfter = 1f;
                    //png.Alignment = Element.ALIGN_LEFT;
                    ////sb.Append(png);
                    //sb.Append("<div style='float:left;'> " + png + " </div>");


                    sb.Append("<table border='1'");
                    sb.Append("<tr>");
                    sb.Append("<td> <strong> Case No</strong></td>");
                    sb.Append("<td><strong>City</strong></td>");
                    sb.Append("<td><strong>Street</strong></td>");
                    sb.Append("<td><strong>Length</strong></td>");
                    sb.Append("<td><strong>Width</strong></td>");
                    sb.Append("<td><strong>Area</strong></td>");
                    if (pdfReportVM.UserType == 1)
                    {
                        sb.Append("<td><strong>Work By</strong></td>");
                    }
                    sb.Append("<td><strong>Created Date</strong></td>");
                    sb.Append("<td><strong>Work Status</strong></td>");

                    sb.Append("</tr>");
                    //sb.Append("</table>");
                    //sb.Append("<table border='1' cellpadding='0' cellspacing='0' width='100%'>");
                    foreach (var item in WorkReportsList)
                    {
                        //var statusData = Workstatus.Where(x => x.Id == item.WorkStatus).FirstOrDefault() == null ? "" : Workstatus.Where(x => x.Id == item.WorkStatus).FirstOrDefault().FieldName.ToString();

                        sb.Append("<tr>");
                        sb.Append("<td>" + item.WorkId.ToString() + "</td>");
                        sb.Append("<td>" + item.City.ToString() + "</td>");
                        sb.Append("<td>" + item.CaseSite.ToString() + "</td>");
                        sb.Append("<td>" + item.Length.ToString() + "</td>");
                        sb.Append("<td>" + item.Width.ToString() + "</td>");
                        sb.Append("<td>" + item.Area.ToString() + "</td>");
                        if (pdfReportVM.UserType == 1)
                        {
                            sb.Append("<td>" + item.CreatedBy.ToString() + "</td>");
                        }
                        sb.Append("<td>" + item.Date.ToString() + "</td>");
                        //sb.Append("<td>" + (Workstatus.Where(x => x.Id == item.WorkStatus).FirstOrDefault() != null ? Workstatus.Where(x => x.Id == item.WorkStatus).FirstOrDefault().FieldName.ToString() : "" )+ "</td>");
                        sb.Append("<td>" + item.WorkStatus.ToString() + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");

                    var htmlstring = sb.ToString();

                    StringReader sr = new StringReader(sb.ToString());

                    byte[] bytes;

                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();


                        //foreach (var item in WorkReportsList)
                        //{
                        //    pdfDoc.Add(new Paragraph("JPG"));
                        //    var imagepath2 = System.Web.Hosting.HostingEnvironment.MapPath("~");
                        //    var filepath3 = imagepath2 + item.BeforeImage;

                        //    if (File.Exists(filepath3))
                        //    {
                        //        Image gif = Image.GetInstance(filepath3);
                        //        gif.ScalePercent(12f);
                        //        //gif.
                        //        gif.SetAbsolutePosition(pdfDoc.PageSize.Width - 36f - 72f,
                        //        pdfDoc.PageSize.Height - 36f - 216.6f);
                        //        pdfDoc.Add(gif);
                        //    }
                        //}


                        htmlparser.Parse(sr);
                        pdfDoc.Close();

                        bytes = memoryStream.ToArray();
                        memoryStream.Close();
                    }

                    //genrate pdf from html string
                    //  var pathEmailpdf = ConfigurationManager.AppSettings["MainURL"];
                    Guid guid = Guid.NewGuid();
                    var downloadName = "WorkReport-" + guid;
                    byte[] downloadBytes = null;
                    downloadName += ".pdf";

                    var savedFilePath = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Document\WorkReport\") + downloadName;
                    var path = Path.GetDirectoryName(savedFilePath);
                    if (path != null && !Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var file = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                    //file.Write(downloadBytes, 0, downloadBytes.Length);
                    file.Write(bytes, 0, bytes.Length);
                    file.Close();

                    //----------------------------
                    var dirpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    var filepath = dirpath + @"Document\WorkReport\" + downloadName;

                    if (System.IO.File.Exists(filepath))
                    {
                        response.IsSuccess = true;
                        response.Status = "SUCCESS";
                        response.Content = "/Document/WorkReport/" + downloadName;
                        //response.Content = htmlstring;

                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel DownloadMaterialReportPdf(PdfReportVM pdfReportVM)
        {
            try
            {
                List<InwardOutwardReportsVM> MaterialReportsList = new List<InwardOutwardReportsVM>();
                MaterialReportsList = pdfReportVM.MaterialReportsList;
                //var list = new List<InwardOutwardReportsVM>();
                ////if (idViewModel.UserType == 1)
                ////{
                ////   //// MaterialReportsList = (List<InwardOutwardReportsVM>)this.GetAdminMaterialReportsList(idViewModel).Content;
                ////}
                ////else
                ////{
                ////    //MaterialReportsList = (List<InwardOutwardReportsVM>)this.GetMaterialReportsList(idViewModel).Content;
                ////}




                if (MaterialReportsList != null && MaterialReportsList.Count > 0)
                {
                    var ContactData = contactRepository.FindBy(x => x.UserId == pdfReportVM.UserId).FirstOrDefault();
                    //WorkReportsList =  list.Content;
                    // MaterialReportsList = (List<InwardOutwardReportsVM>)list.Content;

                    StringBuilder sb = new StringBuilder();
                    //sb.Append("<html><head><style type='text/css'> .tst{width: 100%; background: #fff; color:#008000; margin: 0 auto; padding: 15px 30px; max-width: 80%; display: table;box-shadow: 1px 1px 2px #ddd;} </style></head><body>");

                    sb.Append("<h1 class='tst'> Material Report </h1>");
                    sb.Append("<div color='red' text-align='right'> Date:- " + DateTime.Now.ToShortDateString() + "</div>");
                    sb.Append("<div> Hello " + ContactData.FirstName + " " + ContactData.LastName + ",</div>");
                    sb.Append("<div> This is your Material report. </div>");
                    sb.Append("<div>. </div>");
                    sb.Append("<table border='1'");
                    sb.Append("<tr>");
                    sb.Append("<td> <strong>Invoice No</strong></td>");
                    sb.Append("<td><strong>City</strong></td>");
                    sb.Append("<td><strong>SiteName</strong></td>");
                    sb.Append("<td><strong>Item</strong></td>");
                    sb.Append("<td><strong>Unit</strong></td>");
                    sb.Append("<td><strong>Quantity</strong></td>");
                    if (pdfReportVM.UserType == 1)
                    {
                        sb.Append("<td><strong>Created By</strong></td>");

                    }
                    sb.Append("<td><strong>Created Date</strong></td>");
                    sb.Append("</tr>");
                    //sb.Append("</table>");
                    //sb.Append("<table border='1' cellpadding='0' cellspacing='0' width='100%'>");
                    foreach (var item in MaterialReportsList)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>" + item.InvoiceNo.ToString() + "</td>");
                        sb.Append("<td>" + item.City.ToString() + "</td>");
                        sb.Append("<td>" + item.SiteName.ToString() + "</td>");
                        sb.Append("<td>" + item.Item.ToString() + "</td>");
                        sb.Append("<td>" + item.Unit.ToString() + "</td>");
                        sb.Append("<td>" + item.Quantity.ToString() + "</td>");

                        if (pdfReportVM.UserType == 1)
                        {
                            sb.Append("<td>" + item.CreatedBy.ToString() + "</td>");

                        }
                        sb.Append("<td>" + item.CreatedDate.ToString() + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    //sb.Append("</body></html>");


                    var htmlstring = sb.ToString();

                    StringReader sr = new StringReader(sb.ToString());

                    byte[] bytes;

                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();

                        htmlparser.Parse(sr);
                        pdfDoc.Close();

                        bytes = memoryStream.ToArray();
                        memoryStream.Close();
                    }
                    //******************

                    //genrate pdf from html string
                    //  var pathEmailpdf = ConfigurationManager.AppSettings["MainURL"];
                    Guid guid = Guid.NewGuid();
                    var downloadName = "Material_Report-" + guid;
                    byte[] downloadBytes = null;
                    downloadName += ".pdf";

                    var savedFilePath = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"Document\WorkReport\") + downloadName;
                    var path = Path.GetDirectoryName(savedFilePath);
                    if (path != null && !Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var file = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                    //file.Write(downloadBytes, 0, downloadBytes.Length);
                    file.Write(bytes, 0, bytes.Length);
                    file.Close();

                    //----------------------------
                    var dirpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                    var filepath = dirpath + @"Document\WorkReport\" + downloadName;

                    if (System.IO.File.Exists(filepath))
                    {
                        response.IsSuccess = true;
                        response.Status = "SUCCESS";
                        response.Content = "/Document/WorkReport/" + downloadName;
                        //response.Content = htmlstring;

                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
        public ResponseViewModel DeleteExistingPdf(IdViewModel idViewModel)
        {
            try
            {

                if (!string.IsNullOrEmpty(idViewModel.FormType))
                {
                    var dirpath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;

                    var splitedarr = idViewModel.FormType.Split('/');

                    var finalapath = dirpath + splitedarr[1] + "\\" + splitedarr[2] + "\\" + splitedarr[3];

                    if (System.IO.File.Exists(finalapath))
                    {
                        File.Delete(finalapath);
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Saved successfully...";
                    // response.Content = data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public ResponseViewModel WorkRegenrate(IdViewModel idViewModel)
        {
            try
            {
                if (idViewModel != null && !string.IsNullOrEmpty(idViewModel.UserId) && idViewModel.Id > 0)
                {
                    var workdata = workRepository.GetSingle(idViewModel.Id);
                    if (workdata != null && workdata.Id > 0)
                    {
                        var IDVM = new IdViewModel();
                        IDVM.FormType = "workgenrate";
                        IDVM.UserId = workdata.CreatedBy;
                        var nextId = (long)iCommonService.GetNextId(IDVM).Id;

                        //WorkVM workVM = new WorkVM();
                        //workVM = Mapper.Map<Work, WorkVM>(workdata);

                        //var WorkIdData = this.GetWorkCaseNo(workVM);
                        //if (WorkIdData.Content != null && WorkIdData.IsSuccess)
                        //{
                        //    workVM.WorkId = WorkIdData.Content.ToString();
                        //}




                        if (nextId > 0)
                        {
                            var NewWorkGenData = new Work();

                            NewWorkGenData.CaseNo = nextId;
                            NewWorkGenData.WorkId = workdata.WorkId;
                            NewWorkGenData.RegenrateBy = idViewModel.UserId;
                            NewWorkGenData.RegenrateDate = DateTime.Now;
                            NewWorkGenData.BeforeDate = workdata.BeforeDate;
                            NewWorkGenData.CaseSite = workdata.CaseSite;
                            NewWorkGenData.City = workdata.City;
                            NewWorkGenData.BeforeImage = workdata.BeforeImage;
                            NewWorkGenData.CreatedBy = workdata.CreatedBy;
                            NewWorkGenData.CreatedDate = workdata.CreatedDate;
                            NewWorkGenData.Landmark = workdata.Landmark;
                            NewWorkGenData.WorkStatus = 11;


                            workRepository.Add(NewWorkGenData);
                            unitOfWork.Commit();
                        }

                        workdata.RegenrateBy = idViewModel.UserId;
                        workdata.RegenrateDate = DateTime.Now;
                        workdata.WorkStatus = 13;

                        workRepository.Edit(workdata, workdata);
                        unitOfWork.Commit();
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Record Updated successfully...";
                    response.Content = null;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public ResponseViewModel GetWorkCaseNo(WorkVM workVM)
        {
            try
            {

                if (!string.IsNullOrEmpty(workVM.CreatedBy))
                {
                    Contact MgrContData = new Contact();
                    var city = cityRepository.FindBy(x => !x.IsDeleted && x.Id == workVM.City).FirstOrDefault();
                    var ManagersupermappData = managerSupervisorTypeMappingRepository.FindBy(x => x.SupervisorId == workVM.CreatedBy && !x.IsDeleted).FirstOrDefault();
                    if (ManagersupermappData != null && ManagersupermappData.Id > 0)
                    {
                        var usertypemappData = userTypeMappingRepository.FindBy(x => x.UserId == ManagersupermappData.ManagerId && !x.IsDeleted).FirstOrDefault();
                        if (usertypemappData != null && usertypemappData.Id > 0)
                        {
                            MgrContData = contactRepository.FindBy(x => x.Id == usertypemappData.ContactId && !x.IsDeleted).FirstOrDefault();
                        }
                    }
                    var Date = DateTime.Now;
                    var cityCode = city != null ? city.CityCode : "";
                    var mgrCode = MgrContData != null ? MgrContData.FirstName.Substring(0, 3) : "";
                    var supCode = !string.IsNullOrEmpty(workVM.CreatedBy) ? workVM.CreatedBy.Substring(0, 3) : "";
                    var DateCode = Date.Day.ToString() + Date.Month.ToString() + Date.Year.ToString();
                    var CaseNo = workVM.CaseNo;


                    var CaseNoCode = cityCode + mgrCode + supCode + DateCode + CaseNo;
                    //UserTypeMappingRepository
                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "get Work Id successfully...";
                    response.Content = CaseNoCode.ToLower();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public ResponseViewModel DeleteWorkById(IdViewModel idViewModel)
        {
            try
            {

                if (idViewModel.Id > 0)
                {
                    var workdata = workRepository.FindBy(x => x.Id == idViewModel.Id && !x.IsDeleted).FirstOrDefault();
                    if (workdata != null && workdata.Id > 0)
                    {
                        workRepository.SoftDelete(workdata);
                        unitOfWork.Commit();
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Work Deleted successfully...";
                    // response.Content = data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }

        public ResponseViewModel DeleteMaterialById(IdViewModel idViewModel)
        {
            try
            {

                if (idViewModel.Id > 0)
                {
                    var inwardOutward = inwardOutwardRepository.FindBy(x => x.Id == idViewModel.Id && !x.IsDeleted).FirstOrDefault();
                    if (inwardOutward != null && inwardOutward.Id > 0)
                    {
                        inwardOutwardRepository.SoftDelete(inwardOutward);
                        unitOfWork.Commit();
                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = "Work Deleted successfully...";
                    // response.Content = data;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = "FAILED";
                    response.Message = "Someting Went Wrong!";
                    response.Content = null;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = "FAILED";
                response.Content = null;
                if (response.ReturnMessage == null)
                    response.ReturnMessage = new List<string>();
                response.ReturnMessage.Add(ex.Message);
            }
            return response;
        }
    }
}