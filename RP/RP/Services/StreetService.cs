using AutoMapper;
using RP.Common;
using RP.IRepository;
using RP.IServices;
using RP.Models.Company;
using RP.Models.Street;
using RP.ViewModel.Common;
using RP.ViewModel.Street;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RP.Common.Enums;

namespace RP.Services
{
    public class StreetService : IStreetService
    {
        #region--Inject Dependency--
        ResponseViewModel response = new ResponseViewModel();
        private readonly IEntityBaseRepository<Work> workRepository;
        private readonly IEntityBaseRepository<Street> streetRepository;
        private readonly IEntityBaseRepository<AssignStreet> assignStreetRepository;
        
        private readonly IUnitOfWork unitOfWork;
        private ICommonService iCommonService;

        public StreetService(
            IEntityBaseRepository<AssignStreet> AssignStreetRepository,
        IEntityBaseRepository<Work> WorkRepository, 
            IEntityBaseRepository<Street> StreetRepository,
        IUnitOfWork UnitOfWork,
            ICommonService ICommonService
        )
        {
            assignStreetRepository = AssignStreetRepository;
            workRepository = WorkRepository;
            streetRepository = StreetRepository;
                    unitOfWork = UnitOfWork;
            iCommonService = ICommonService;

        }

        #endregion

        public ResponseViewModel SaveUpdateStreetData(StreetVM streetVM)

        {
            try
            {
                if (streetVM.Id == 0)
                {
                    //add
                    if (streetVM != null)
                    {
                        Street data = new Street();
                        if (streetVM.UserType == UserType.Supervisor)
                        {
                            data.StreetName = streetVM.StreetName;
                            data.CreatedBy = streetVM.CreatedBy;
                            if (data != null)
                            {
                                streetRepository.Add(data);
                                unitOfWork.Commit();
                            }

                            if (data != null && data.Id > 0)
                            {

                                AssignStreet assginstreet = new AssignStreet();

                                assginstreet.StreetId = data.Id;
                                assginstreet.SupervisorId = streetVM.CreatedBy;
                                assignStreetRepository.Add(assginstreet);
                                unitOfWork.Commit();
                            }

                        }
                        else
                        {
                            //data = Mapper.Map<Street>(streetVM);
                            data.StreetName = streetVM.StreetName;
                            data.CreatedBy = streetVM.CreatedBy;
                            if (data != null)
                            {
                                streetRepository.Add(data);
                                unitOfWork.Commit();
                            }
                        }

                       
                       

                        response.Status = "SUCCESS";
                        response.IsSuccess = true;
                        response.Message = "Record Saved successfully...";
                        response.Content = null;
                    }

                }
                else
                {
                    //update




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

       

 public ResponseViewModel GetStreetList(IdViewModel idViewModel)

        {
            try
            {
                if (!string.IsNullOrEmpty(idViewModel.UserId))
                {
                    List<StreetVM> streetlist = new List<StreetVM>();

                   var streetlistData = streetRepository.FindBy(x => x.CreatedBy == idViewModel.UserId && !x.IsDeleted).ToList();

                    if (streetlistData != null && streetlistData.Count > 0)
                    {
                        streetlistData.ForEach(x => {
                            streetlist.Add(new StreetVM() {
                                CreatedBy = x.CreatedBy,
                                Id =x.Id,
                                StreetName =x.StreetName,
                                IsSelect =false
                            });

                        });

                        //streetlist = Mapper.Map<List<Street>,List<StreetVM>>(streetlistData);


                    }

                    response.Status = "SUCCESS";
                    response.IsSuccess = true;
                    response.Message = "Record get successfully...";
                    response.Content = streetlist;


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

 public ResponseViewModel AssignStreet(StreetAssignVM model)

        {
            try
            {
                if (!string.IsNullOrEmpty(model.UserId) && model.StreetListVM != null && model.StreetListVM.Count > 0)
                {
                    List<AssignStreet> assginstreet = new List<AssignStreet>();
                    model.StreetListVM.ForEach(x => {

                        if (x.IsSelect)
                        {
                            assginstreet.Add( new AssignStreet()
                            {
                                StreetId =x.Id,
                                CreatedBy = x.CreatedBy,
                                SupervisorId =model.UserId
                            });
                        }
                    });

                    if (assginstreet != null && assginstreet.Count > 0)
                    {
                        assignStreetRepository.AddRange(assginstreet);
                        unitOfWork.Commit();
                    }

                    response.Status = "SUCCESS";
                    response.IsSuccess = true;
                    response.Message = "assign street successfully...";
                    
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
public ResponseViewModel GetAssignStreetList(IdViewModel model)

        {
            try
            {
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    List<StreetVM> streetlist = new List<StreetVM>();

                    var streetlistData = assignStreetRepository.FindBy(x => x.SupervisorId == model.UserId && !x.IsDeleted).ToList();

                    if (streetlistData != null && streetlistData.Count > 0)
                    {

                        var DictinctstreetlistData = streetlistData.DistinctBy(q => q.StreetId).ToList();

                        DictinctstreetlistData.ForEach(x => {

                            var streetData = streetRepository.FindBy(y => y.Id == x.StreetId && !y.IsDeleted).FirstOrDefault();

                            streetlist.Add(new StreetVM()
                            {
                                CreatedBy = x.CreatedBy,
                                Id = streetData != null ? streetData.Id : 0,
                                StreetName = streetData != null ? streetData.StreetName: ""
                               
                            });

                        });

                        //streetlist = Mapper.Map<List<Street>,List<StreetVM>>(streetlistData);


                    }

                    response.Status = "SUCCESS";
                    response.IsSuccess = true;
                    response.Message = "assign street successfully...";
                    response.Content = streetlist;
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