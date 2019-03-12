using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RP.IdentityAuthentication.AccountModels;
using RP.Models.Account;
using RP.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace RP.IdentityAuthentication
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;
        ResponseViewModel responseViewModel = new ResponseViewModel();

        private UserManager<ApplicationUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }
       

        public async Task<ResponseViewModel> ConfirmEmail(Contact ouser)
        {
            try
            {
            //    var asd = _userManager;
            //    var user = asd.FindById(ouser.UserId);

            //    if (user != null)
            //    {
            //        user.EmailConfirmed = true;
            //        await asd.UpdateAsync(user);
            //        responseViewModel.IsSuccess = true;
            //        responseViewModel.Message = "Email confirmed successfully.";
            //    }
            //    else
            //    {
            //        responseViewModel.IsSuccess = false;
            //        responseViewModel.Message = "User not found.";
            //    }
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.Message = "Something went wrong. Please call technical team.";
            }
            return responseViewModel;
        }

        public string GetUserID(string email)
        {
            string userId = "";
            try
            {
                _ctx = new AuthContext();
                MembershipUser member = Membership.GetUser(email);
                userId = member.ProviderUserKey.ToString();
            }
            catch (Exception)
            {

            }
            return userId;

        }

        public ResponseViewModel GetUser(string username)
        {
            try
            {
                //var result = _userManager.FindByEmail(email);
                var result = _userManager.FindByName(username);

                if (result != null)
                {
                    //UserModel user = new UserModel()
                    //{
                    //    Email = result.UserName
                    //};
                    responseViewModel.IsSuccess = true;
                    responseViewModel.Content = result;
                    responseViewModel.Message = "User Found..";
                }
                else
                {
                    responseViewModel.IsSuccess = false;
                    responseViewModel.Message = "User Not Found.";
                }
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                responseViewModel.ReturnMessage.Add(ex.Message);
            }

            return responseViewModel;

        }

        public ResponseViewModel GetRoles()
        {
            try
            {
                _ctx = new AuthContext();
                responseViewModel.Content = _ctx.Roles;
                responseViewModel.IsSuccess = true;
                responseViewModel.Message = "UserRole Added Successfully.";
            }
            catch (Exception ex)
            {

                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                responseViewModel.ReturnMessage.Add(ex.Message);
            }
            return responseViewModel;

        }

        public bool AssignRole(string UserId, string UserRole)
        {
            try
            {
                _ctx = new AuthContext();
                _userManager.AddToRole(UserId, UserRole);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ResponseViewModel CreateRole(string RoleName)
        {
            try
            {
                _ctx = new AuthContext();
                _ctx.Roles.Add(new IdentityRole()
                {
                    Name = RoleName
                });
                _ctx.SaveChanges();
                responseViewModel.IsSuccess = true;
                responseViewModel.Message = "UserRole Added Successfully.";
            }
            catch (DbEntityValidationException e)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                        responseViewModel.ReturnMessage.Add(ve.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                responseViewModel.ReturnMessage.Add(ex.Message);
            }
            return responseViewModel;
        }

        public ResponseViewModel DeleteRole(int ID)
        {
            try
            {
                IdentityRole role = _ctx.Roles.Find(ID);
                _ctx.Roles.Remove(role);
                responseViewModel.IsSuccess = true;
                responseViewModel.Message = "UserRole Deleted Successfully..";
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                responseViewModel.ReturnMessage.Add(ex.Message);
            }
            return responseViewModel;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public ResponseViewModel FindLogin(string email, string password)
        {
            try
            {
                var result = _userManager.Find(email,password);

                if (result != null)
                {
                    //UserModel user = new UserModel()
                    //{
                    //    Email = result.UserName
                    //};
                    responseViewModel.IsSuccess = true;
                    responseViewModel.Content = result;
                    responseViewModel.Message = "Logged in successfully";
                }
                else
                {
                    responseViewModel.IsSuccess = false;
                    responseViewModel.Message = "User Not Found.";
                }
            }
            catch (Exception ex)
            {
                responseViewModel.IsSuccess = false;
                responseViewModel.ReturnMessage = new List<string>();
                responseViewModel.ReturnMessage.Add(ex.Message);
            }

            return responseViewModel;

        }
        //public ResponseViewModel FindUserLogin(string userName, string password)
        //{
        //    ApplicationUser user =  _userManager.FindAsync(userName, password);

        //    return null;
        //}
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }

    }

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        //public string PhoneNumber { get; set; }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        //    return userIdentity;
        //}
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}