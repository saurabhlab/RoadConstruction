using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using RP.App_Start;
using RP.IdentityAuthentication;
using RP.IdentityAuthentication.AccountModels;
using RP.IRepository;
using RP.IServices;
using RP.Models;
using RP.Models.Account;
using RP.Models.Admin;
using RP.Models.Common;
using RP.Models.Master;
using RP.Repository;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;
using static RP.Common.Enums;

namespace RP.Controllers
{
    //[EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {

        private const string LocalLoginProvider = "Local";
        ResponseViewModel responseViewModel = new ResponseViewModel();
        RegistrationResponseModel registrationresponseModel = new RegistrationResponseModel();
        #region--Inject Dependency--
        //private IRepository<Student> studentRepository = null;
        //private IRepository<ItemMaster> itemMasterRepository = null;
        private UserManager<ApplicationUser> _userManager;

        private readonly IEntityBaseRepository<Contact> contactRepository;

        private readonly IEntityBaseRepository<UserTypeMapping> userTypeMappingRepository;

        private readonly IEntityBaseRepository<AccessPermission> accessPermissionRepository;

        private readonly IEntityBaseRepository<ManagerSupervisorTypeMapping> managerSupervisorTypeRepository;

        private readonly IUnitOfWork _unitOfWork;

        private IAccountService accountService;



        ProcessedResponse processedResponse = new ProcessedResponse();
        public AccountController(IAccountService AccountService,
            IEntityBaseRepository<Contact> ContactRepository,
        IEntityBaseRepository<UserTypeMapping> UserTypeMappingRepository,
        IEntityBaseRepository<AccessPermission> AccessPermissionRepository,
        IEntityBaseRepository<ManagerSupervisorTypeMapping> ManagerSupervisorTypeRepository,

        IUnitOfWork unitOfWork)
        {
            accountService = AccountService;
            contactRepository = ContactRepository;
            userTypeMappingRepository = UserTypeMappingRepository;
            accessPermissionRepository = AccessPermissionRepository;
            managerSupervisorTypeRepository = ManagerSupervisorTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public AccountController(ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            _userManager = UserManager;
            AccessTokenFormat = accessTokenFormat;
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AuthContext()));
                //return manager;
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //public AccountController(//IAccountService AccountService,
        //    IEntityBaseRepository<ItemMaster> ItemMasterRepository2)
        //{
        //   // this.studentRepository = new Repository<Student>();
        //   // this.itemMasterRepository = new Repository<ItemMaster>();
        //  //  this.accountService = AccountService;
        //    itemMasterRepository2 = ItemMasterRepository2;
        //}

        #endregion

        // GET api/<controller>
        //https://www.c-sharpcorner.com/article/using-autofac-with-web-api/

        #region--  GetItemMasterDetail
           

            [HttpGet, Route("GetItemMasterDetail")]
            public HttpResponseMessage GetItemMasterDetail()
            {
                try
                {
                    UserModel userModel = new UserModel();
                    userModel.FirstName = "Super";
                    userModel.LastName = "Admin";
                    userModel.UserName = "superadmin";
                    userModel.Email = "superadmin@yopmail.com";
                    userModel.Password = "Password@123";
                    userModel.ConfirmPassword = "Password@123";
                    userModel.PhoneNumber = "7895641236";
                    userModel.UserTypeId = 1;
                    userModel.UserRole = "Superadmin";

                    //var yyb= this.Register(userModel);

                    //IdentityUser user = await _userManager.FindAsync(userModel.Email, userModel.Password);
                    //     var test = itemMasterRepository2.GetAll();
                    // var test = itemMasterRepository.GetAll();//accountService.GeMasterViewModelList();
                    //processedResponse.Content = accountService.GeMasterViewModelList();
                    processedResponse.Status = "success";
                    return Request.CreateResponse(HttpStatusCode.OK, processedResponse, "application/json");
                }
                catch (Exception ex)
                {
                    processedResponse.Message = ex.Message;
                   // log.Error("Error Message: " + ex.Message.ToString(), ex);
                    processedResponse.Status = "error";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, processedResponse, "application/json");
                }

            }
        #endregion

        #region--GetUserPermissionData
        [HttpPost, Route("GetUserPermissionData")]
        public HttpResponseMessage GetUserPermissionData(IdViewModel idViewModel)
        {
            var response = accountService.GetUserPermissionData(idViewModel);
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

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            try
            {
                var name = User.Identity.Name;
                Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                FormsAuthentication.SignOut();
                return Ok("Logout successful.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something went wrong.");
            }
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Authorize]
        [Route("ChangePassword")]
        public async Task<ResponseViewModel> ChangePassword(ChangePasswordBindingModel model)
        {
            //if (model.NewPassword != model.ConfirmPassword)
            //{
            //    responseViewModel.IsSuccess = false;
            //    responseViewModel.ReturnMessage = new List<string>();

            //    responseViewModel.ReturnMessage.Add("New Password and Confirm Password does not match.");
            //    return responseViewModel;
            //}
            string userID = "";
            using (AuthRepository _repo = new AuthRepository())
            {
                var result = (IdentityUser)_repo.GetUser(User.Identity.Name).Content;
                if (result != null)
                {
                    userID = result.Id;
                }
            }
            if (!string.IsNullOrEmpty(userID))
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(userID, model.OldPassword,
                                                                                model.NewPassword);

                if (!result.Succeeded)
                {
                    var message = "Incorrect current password.";
                    IHttpActionResult rslt = GetErrorResult(result);
                    responseViewModel.IsSuccess = false;
                    responseViewModel.ReturnMessage = new List<string>();

                    foreach (var item in result.Errors)
                    {
                        if (item == "Incorrect password.")
                        {
                            responseViewModel.ReturnMessage.Add(message);
                        }
                        else
                        {
                            responseViewModel.ReturnMessage.Add(item.ToString());

                        }

                    }
                }
                else
                {
                    responseViewModel.IsSuccess = true;
                    responseViewModel.Message = "Password changed successfully..";
                }
            }

            //IdentityResult result = await UserManager.ChangePasswordAsync()
            return responseViewModel;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<ResponseViewModel> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Invalid operation";
                return responseViewModel;
            }
            AuthRepository _repo = new AuthRepository();
            var user = await UserManager.FindByIdAsync(model.UserId);

            if (user != null)
            {


               var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                if (!string.IsNullOrEmpty(token))
                {
                  var result =  await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                    responseViewModel.IsSuccess = true;
                    responseViewModel.Message = "Password change successfully";

                }
                else
                {
                    responseViewModel.IsSuccess = false;
                    responseViewModel.Message = "token expired";
                }
            }
            else
            {
                // Don't reveal that the user does not exist
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Invalid User";

            }
           ///// var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            //var tokencode = WebUtility.UrlDecode(model.Code);
            //var tt = HttpUtility.UrlDecode(model.Code);
            //var newtoken = tt.Replace(" ", "+");
            ////var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            ////if (result.Succeeded)
            ////{
            ////    responseViewModel.IsSuccess = true;
            ////    responseViewModel.Message = "Successfully you changed your password";
            ////}
            ////else
            ////{
            ////    responseViewModel.IsSuccess = false;
            ////    responseViewModel.Message = "This link was expired";
            ////}
            return responseViewModel;
        }


        //[Route("ResetPassword")]
        //[AllowAnonymous]
        //public async Task<ResponseViewModel> ResetPassword(ChangePasswordBindingModel model)
        //{
        //    if (model.NewPassword != model.ConfirmPassword)
        //    {
        //        responseViewModel.IsSuccess = false;
        //        responseViewModel.ReturnMessage = new List<string>();

        //        responseViewModel.ReturnMessage.Add("New Password and Confirm Password does not match.");
        //        return responseViewModel;
        //    }
        //    string userID = "";
        //    string pass = "";

        //    using (AuthRepository _repo = new AuthRepository())
        //    {
        //       var   result = (IdentityUser)_repo.GetUser(model.Email).Content;
        //        if (result != null)
        //        {
        //            userID = result.Id;
        //        }
        //         pass = result.PasswordHash;
        //    }
        //    var token = "hihihh";
        //    if (!string.IsNullOrEmpty(userID))
        //    {
        //        //ApplicationUser cUser = UserManager.FindById(userID);
        //        //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.NewPassword);
        //        //UserStore<ApplicationUser> store = new UserStore<ApplicationUser>();
        //        //store.SetPasswordHashAsync(cUser, hashedNewPassword);


        //        IdentityResult result = UserManager.ResetPassword(userID, token, model.NewPassword);
        //        if (!result.Succeeded)
        //        {
        //            IHttpActionResult rslt = GetErrorResult(result);
        //            responseViewModel.IsSuccess = false;
        //            responseViewModel.ReturnMessage = new List<string>();

        //            foreach (var item in result.Errors)
        //            {
        //                responseViewModel.ReturnMessage.Add(item.ToString());
        //            }
        //        }
        //        else
        //        {
        //            responseViewModel.IsSuccess = true;
        //            responseViewModel.Message = "Password Change Successfully..";
        //        }
        //    }

        //    //IdentityResult result = await UserManager.ChangePasswordAsync()
        //    return responseViewModel;
        //}



        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //// GET api/Account/ExternalLogin
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        //[AllowAnonymous]
        //[Route("ExternalLogin", Name = "ExternalLogin")]
        //public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        //{
        //    if (error != null)
        //    {
        //        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return new ChallengeResult(provider, this);
        //    }

        //    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    if (externalLogin == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (externalLogin.LoginProvider != provider)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //        return new ChallengeResult(provider, this);
        //    }

        //    IdentityUser user = await _userManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        //        externalLogin.ProviderKey));

        //    bool hasRegistered = user != null;

        //    if (hasRegistered)
        //    {
        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //        ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //           OAuthDefaults.AuthenticationType);
        //        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
        //            CookieAuthenticationDefaults.AuthenticationType);

        //        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
        //        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        //    }
        //    else
        //    {
        //        IEnumerable<Claim> claims = externalLogin.GetClaims();
        //        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        //        Authentication.SignIn(identity);
        //    }

        //    return Ok();
        //}

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<ResponseViewModel> Register(RegisterVM model)
        {
            try
            {
                ApplicationUser existuser = null;
                using (AuthRepository _repo = new AuthRepository())
                {
                    existuser = (ApplicationUser)_repo.GetUser(model.UserName).Content;
                }
                if (existuser != null)
                {
                    if (!string.IsNullOrEmpty(existuser.Id))
                    {
                        registrationresponseModel.IsSuccess = false;
                        registrationresponseModel.IsMap = false;
                        registrationresponseModel.Message = "User already exist.";
                    }
                }
                else
                {
                    var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
                    user.LockoutEnabled = false;
                    //user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(3);

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        IHttpActionResult rslt = GetErrorResult(result);
                    }
                    else
                    {

                        // Generate Token for Email Confirmation...
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var tokencode = HttpUtility.UrlEncode(code);

                        //Add Contact Details
                        Contact contct = new Contact()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            Address = model.Address,
                            Gender =model.Gender,
                            City = model.City,
                            ContactNo= model.ContactNo,
                            Designation =model.Designation,
                            UserId = user.Id
                        };

                        contactRepository.Add(contct);
                        _unitOfWork.Commit();

                        // Add NetUserDetails
                        UserTypeMapping userTypeMapping = new UserTypeMapping()
                        {
                            UserId = user.Id,
                            ContactId = contct.Id,
                            UserType = model.Designation
                        };
                        userTypeMappingRepository.Add(userTypeMapping);

                        if (!string.IsNullOrEmpty(model.ManagerId) && !string.IsNullOrEmpty(user.Id))
                        {
                            ManagerSupervisorTypeMapping mgrsprmapp = new ManagerSupervisorTypeMapping()
                            {
                                SupervisorId = user.Id,
                                ManagerId = model.ManagerId
                            };

                            managerSupervisorTypeRepository.Add(mgrsprmapp);
                            _unitOfWork.Commit();
                        }

                        // Add Permissions

                        AccessPermission accessPermission = new AccessPermission()
                        {
                            UserId = user.Id,
                            ContactId = contct.Id,
                            UserType = model.Designation,
                           // IsMaterialReports = true,
                           // IsWorkReports = true
                        };

                        if (model.Designation == (long)UserType.Manager)
                        {
                            accessPermission.IsMaterialReports = true;
                            accessPermission.IsMaterialInward = true;
                            accessPermission.IsMaterialOutward = true;
                            accessPermission.IsWorkReports = true;
                            // accessPermission.IsWorkComplete = true;
                            //accessPermission.IsWorkGenrate = true;
                        }
                        else if (model.Designation == (long)UserType.Supervisor)
                        {                            
                            accessPermission.IsWorkComplete = true;
                            accessPermission.IsWorkGenrate = true;
                            accessPermission.IsWorkReports = true;
                        }
                        else if (model.Designation == (long)UserType.Engineer)
                        {
                            accessPermission.IsWorkReports = true;
                        }


                        accessPermissionRepository.Add(accessPermission);
                        _unitOfWork.Commit();

                    }
                    
                    registrationresponseModel.IsSuccess = true;
                    registrationresponseModel.Message = "User Registered Successfully.";

                    return registrationresponseModel;
                }
            }
            catch (Exception ex)
            {

            }

            return registrationresponseModel;
        }
      

        // GET: /Account/ConfirmEmail
        [Route("ConfirmEmailByMail")]
        [AllowAnonymous]
        public async Task<ResponseViewModel> ConfirmEmail(ConfirmViewModel model)
        {
            if (model == null)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Invalid activity. Please try again.";
                return responseViewModel;
            }
            else if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Code))
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Invalid activity. Please try again.";
                return responseViewModel;
            }
            IdentityResult result;
            try
            {
                result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = ioe.Message;
                return responseViewModel;
            }

            if (result.Succeeded)
            {
                responseViewModel.IsSuccess = true;
                responseViewModel.Message = "Email confirmed successfully.";
                return responseViewModel;
            }

            // If we got this far, something failed.
            responseViewModel.IsSuccess = false;
            responseViewModel.Message = "ConfirmEmail failed";
            return responseViewModel;
        }

        [AllowAnonymous]
        [Route("MapUserName")]
        public async Task<ResponseViewModel> MapUserName(RegisterBindingModel model)
        {
            IdentityUser IsexistUser = null;
            AuthRepository _repo = new AuthRepository();
            IsexistUser = (IdentityUser)_repo.GetUser(model.Email).Content;
            if (IsexistUser != null)
            {
                responseViewModel.IsSuccess = true;
                responseViewModel.Message = "Username already exist";
            }
            else
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Username does not exist";
            }

            return responseViewModel;

        }


      
        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }


        //[AcceptVerbs("HttpPost", "HttpGet")]
        [HttpPost]
        [Route("GetLogedInUserDetails")]
        [Authorize]
        public ResponseViewModel GetLogedInUserDetails(UserModel loginmodel)
        {
            try
            {
                var name = User.Identity.Name;
                var name1 = HttpContext.Current.Request.LogonUserIdentity.GetUserId();
                using (AuthRepository _repo = new AuthRepository())
                {
                    // var result = (IdentityUser)_repo.GetUser(loginmodel.Email).Content;
                    var result = (IdentityUser)_repo.FindLogin(loginmodel.UserName, loginmodel.Password).Content;
                    if (result != null)
                    {
                        
                        // Check for UserType
                        if (!string.IsNullOrEmpty(result.Id))
                        {

                            IdViewModel idViewModel = new IdViewModel();

                            idViewModel.UserId = result.Id;

                            var UserData = accountService.GetLogedInUserDetails(idViewModel).Content;

                            //UsersViewModel model = new UsersViewModel();
                            //var newuser = _NetUserrepository.FindBy(x => x.UserId == result.Id).FirstOrDefault();
                            //if (newuser != null)
                            //{
                            //    var contactmodel = _contactrepository.GetSingle(newuser.ContactID.Value);
                            //    newuser.UserContact = contactmodel;
                            //    model.UserID = newuser.UserId;
                            //    model.FirstName = newuser.UserContact.FirstName;
                            //    model.LastName = newuser.UserContact.LastName;
                            //    model.Email = newuser.UserContact.Email;
                            //    model.ContactId = newuser.UserContact.ID;
                            //    model.PhotoPath = newuser.UserContact.PhotoPath;
                            //    model.Mobile = newuser.UserContact.Mobile;
                            //    model.IsEmailConfirmed = result.EmailConfirmed;

                            //}


                            responseViewModel.IsSuccess = true;
                            responseViewModel.Content = UserData;
                            responseViewModel.Message = "Login Successfull.";
                        }
                        else
                        {
                            responseViewModel.ReturnMessage = new List<string>();
                            responseViewModel.ReturnMessage.Add("Invaid login details. Please try again with valid UserName and Password.");
                        }
                    }
                }

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



        [HttpPost]
        [AllowAnonymous]
        [Route("ForgetPasswordUserDetails")]
        public async Task<ResponseViewModel> ForgetPasswordUserDetails(UserModel loginmodel)
        {
            try
            {
                using (AuthRepository _repo = new AuthRepository())
                {

                    var result = (IdentityUser)_repo.GetUser(loginmodel.Email).Content;
                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(result.Id))
                        {
                            //UsersViewModel model = new UsersViewModel();

                            //var newuser = _NetUserrepository.FindBy(x => x.UserId == result.Id).FirstOrDefault();
                            //if (newuser != null)
                            //{
                            //    var contactmodel = _contactrepository.GetSingle(newuser.ContactID.Value);
                            //    newuser.UserContact = contactmodel;
                            //    model.UserID = newuser.UserId;
                            //    model.FirstName = newuser.UserContact.FirstName;
                            //    model.LastName = newuser.UserContact.LastName;
                            //    model.Email = newuser.UserContact.Email;
                            //    model.ContactId = newuser.UserContact.ID;
                            //    model.PhotoPath = newuser.UserContact.PhotoPath;
                            //}


                            var code = await UserManager.GeneratePasswordResetTokenAsync(result.Id);

                            var mailPath = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/ForgotPassword.tpl.html");
                            //var tokencode = WebUtility.UrlEncode(code);
                            var tokencode = HttpUtility.UrlEncode(code);
                            //if (System.IO.File.Exists(mailPath))
                            //{
                            //    EmailViewModel emailVm = new EmailViewModel();
                            //    emailVm.MailTo = model.Email;
                            //    emailVm.MailSubject = "Please Reset Your Password";
                            //    try
                            //    {
                            //        string baseURL = ConfigurationManager.AppSettings["MainURL"].ToString();
                            //        string redirectURL = baseURL + "/Account/Resetpassword?UserId=" + result.Id + "&Code=" + tokencode;
                            //        var message = System.IO.File.ReadAllText(mailPath);
                            //        message = message.Replace("<#firstname#>", model.FirstName);
                            //        message = message.Replace("<#lastname#>", model.LastName);
                            //        message = message.Replace("<#redirectUrl#>", redirectURL);
                            //        emailVm.MailMessage = message;
                            //        Helper.SendEmail(emailVm);


                            //        //var encodedCode = HttpUtility.UrlEncode(code);
                            //        //await UserManager.SendEmailAsync(result.Id, "Reset Password", Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/Account/Resetpassword?Code=" + encodedCode);




                            //    }
                            //    catch (Exception ex)
                            //    {

                            //    }
                            //}
                            responseViewModel.IsSuccess = true;
                            responseViewModel.Content = null;
                            responseViewModel.Message = "Email has been sent you. Please check your email.";



                            //EmailViewModel emailVm = new EmailViewModel();
                            //emailVm.MailTo = model.Email;
                            //emailVm.MailSubject = "Please Reset Your Password";

                            //var mailPath = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/ForgotPassword.tpl.html");

                            //if (System.IO.File.Exists(mailPath))
                            //{
                            //    string urlhost = System.Web.HttpContext.Current.Request.Url.Host;
                            //    int urlport = System.Web.HttpContext.Current.Request.Url.Port;
                            //    string url = "https://" + urlhost + ":" + urlport + "/Account/Login";

                            //    var message = System.IO.File.ReadAllText(mailPath);
                            //    message = message.Replace("<#firstname#>", model.FirstName);
                            //    message = message.Replace("<#lastname#>", model.LastName);
                            //    message = message.Replace("<#redirectUrl#>", url);
                            //    message = message.Replace("<#pass#>", NewpassWord);

                            //    emailVm.MailMessage = message;
                            //}

                            //Foreal.Services.Common.Helper.SendEmail(emailVm);


                        }
                        else
                        {

                            responseViewModel.ReturnMessage = new List<string>();
                            responseViewModel.ReturnMessage.Add("Invalid Email Address. Please try again with valid Email Address.");
                        }
                    }
                    else
                    {
                        responseViewModel.IsSuccess = false;
                        if (responseViewModel.ReturnMessage != null && responseViewModel.ReturnMessage.Count > 0)
                        {
                            responseViewModel.ReturnMessage.Add("User does not exist");
                        }
                        else
                        {
                            responseViewModel.ReturnMessage = new List<string>();
                            responseViewModel.ReturnMessage.Add("User does not exist");
                        }
                    }
                }

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




       
      
      
        [HttpPost, Route("UploadProfilePic"), AllowAnonymous]
        public ResponseViewModel UploadProfilePic()
        {
            try
            {
                var info = new ResponseViewModel();
                ContactVM import = new ContactVM();
                string sPath = "~/PropertyPhotos/ProfilePhotos/";
                sPath = System.Web.Hosting.HostingEnvironment.MapPath(sPath);
                Guid guid = Guid.NewGuid();
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FolderName = System.Web.HttpContext.Current.Request.Form;
                import.Id = Convert.ToInt64(FolderName["ContactID"]);

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    //import.PhotoTypeID = iCnt + 4;
                    System.Web.HttpPostedFile hpf = hfc[iCnt];

                    if (hpf.ContentLength > 0)
                    {
                        var savedFilePath = (System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"PropertyPhotos\ProfilePhotos\");
                        if (!Directory.Exists(savedFilePath))
                            Directory.CreateDirectory(savedFilePath);
                        import.PhotoPath = sPath + "\\" + guid + ".jpg";
                        if (!File.Exists(import.PhotoPath))
                        {
                            hpf.SaveAs(import.PhotoPath);
                            import.PhotoPath = hpf.FileName;
                            import.PhotoPath = "../PropertyPhotos/ProfilePhotos/" + "/" + guid + ".jpg";
                           // var conct = _contactrepository.GetSingle(import.ID);
                            //var newcnct = _contactrepository.GetSingle(import.ID);
                            //newcnct.PhotoPath = "../PropertyPhotos/ProfilePhotos/" + "/" + guid + ".jpg";
                            //_contactrepository.Edit(conct, newcnct);
                            _unitOfWork.Commit();
                        }
                    }
                }
                var model = ""; //_contactrepository.GetSingle(import.ID);
                responseViewModel.Content = model;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion



    }
}