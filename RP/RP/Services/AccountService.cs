using AutoMapper;
using Microsoft.AspNet.Identity;
using RP.App_Start;
using RP.IdentityAuthentication;
using RP.IRepository;
using RP.IServices;
using RP.Models;
using RP.Models.Account;
using RP.Models.Common;
using RP.Models.Master;
using RP.Repository;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Common;
using RP.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Services
{
    public class AccountService:IAccountService
    {
        #region--Inject Dependency--
        private readonly IEntityBaseRepository<ItemMaster> itemMasterRepository;
        private readonly IEntityBaseRepository<AccessPermission> accessPermissionRepository;
        private readonly IEntityBaseRepository<Contact> contactRepository;
        private readonly IEntityBaseRepository<UserTypeMapping> userTypeMappingRepository;
        private readonly IUnitOfWork unitOfWork;
        //private UserManager<ApplicationUser> _userManager;
        ProcessedResponse processedResponse = new ProcessedResponse();
        ResponseViewModel response = new ResponseViewModel();
        public AccountService(IEntityBaseRepository<ItemMaster> ItemMasterRepository,
            IEntityBaseRepository<AccessPermission> AccessPermissionRepository,
        IEntityBaseRepository<Contact> ContactRepository,
        IEntityBaseRepository<UserTypeMapping> UserTypeMappingRepository,
         IUnitOfWork UnitOfWork
            )
        {
            itemMasterRepository = ItemMasterRepository;
            accessPermissionRepository = AccessPermissionRepository;
            contactRepository = ContactRepository;
            userTypeMappingRepository = UserTypeMappingRepository;
            unitOfWork = UnitOfWork;
        }

        //public UserManager<ApplicationUser> UserManager
        //{
        //    get
        //    {
        //        //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AuthContext()));
        //        //return manager;
        //        return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}
        #endregion


        public ResponseViewModel GetUserPermissionData(IdViewModel idViewModel)
        {
            try
            {
                if(!string.IsNullOrEmpty(idViewModel.UserId))
                {

                    AccessPermissionVM ReturnData = new AccessPermissionVM();

                    var accPerData = accessPermissionRepository.FindBy(x => x.UserId == idViewModel.UserId).FirstOrDefault();

                    //var usertypeData = userTypeMappingRepository.FindBy(x => x.UserId == idViewModel.UserId).FirstOrDefault();
                    if (accPerData != null)
                    {
                        ReturnData = Mapper.Map<AccessPermission, AccessPermissionVM>(accPerData);
                        //ReturnData.Contacts = usertypeData.Contacts;
                    }



                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get Data successfully...";
                    response.Content = ReturnData;
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


        public ResponseViewModel UpdatePermissions(AccessPermissionVM accessPermissionVM)
        {
            try
            {
                if (accessPermissionVM != null && accessPermissionVM.Id > 0)
                {
                    

                    var accPerData = accessPermissionRepository.FindBy(x => x.Id == accessPermissionVM.Id).FirstOrDefault();

                    if (accPerData != null)
                    {
                        accPerData.IsMaterialInward = accessPermissionVM.IsMaterialInward;
                        accPerData.IsMaterialOutward = accessPermissionVM.IsMaterialOutward;
                        accPerData.IsWorkGenrate = accessPermissionVM.IsWorkGenrate;
                        accPerData.IsWorkComplete = accessPermissionVM.IsWorkComplete;
                        accPerData.IsMaterialReports = accessPermissionVM.IsMaterialReports;
                        accPerData.IsWorkReports = accessPermissionVM.IsWorkReports;

                        accessPermissionRepository.Edit(accPerData, accPerData);
                        unitOfWork.Commit();

                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Updated successfully...";
                    //response.Content = ReturnData;
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



        

             public ResponseViewModel GetLogedInUserDetails(IdViewModel idViewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(idViewModel.UserId))
                {
                    AccessPermissionVM ReturnData = new AccessPermissionVM();
                    var accPerData = accessPermissionRepository.FindBy(x => x.UserId == idViewModel.UserId).FirstOrDefault();
                    if (accPerData != null)
                    {
                        ReturnData = Mapper.Map<AccessPermission, AccessPermissionVM>(accPerData);

                        var ContData = contactRepository.FindBy(x => x.UserId == idViewModel.UserId).FirstOrDefault();
                        var UserTypeMapData = userTypeMappingRepository.FindBy(x => x.UserId == idViewModel.UserId).FirstOrDefault();
                        if (UserTypeMapData != null && UserTypeMapData.Id > 0 && UserTypeMapData.Contacts != null)
                        {
                            ReturnData.Contacts = UserTypeMapData.Contacts;
                           // ReturnData.UserType = UserTypeMapData.UserType;
                        }

                    }

                    response.IsSuccess = true;
                    response.Status = "SUCCESS";
                    response.Message = " Get Data successfully...";
                    response.Content = ReturnData;
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
 
    }
}