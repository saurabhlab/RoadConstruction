using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using RP.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace RP.IdentityAuthentication.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private ApplicationSignInManager _signInManager;
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                using (var ctx = new AuthContext())
                {
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                    var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
                    manager.UserLockoutEnabledByDefault = true;
                    var user = await manager.FindByNameAsync(context.UserName);
                    if (user != null)
                    {
                        var givenTime = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"]);
                        var validCredentials = await manager.FindAsync(context.UserName, context.Password);
                        if (user.LockoutEndDateUtc != null)
                        {
                            if (user.LockoutEndDateUtc != null && user.LockoutEnabled == true && user.AccessFailedCount == 0)
                            {
                                var msg = string.Format("Your account is Blocked By Admin.");
                                context.SetError("invalid_grant", msg);
                                return;
                            }
                            else
                            {
                                var currentTime = DateTime.UtcNow - user.LockoutEndDateUtc.Value;
                                var remainingTime = givenTime - currentTime.Minutes;
                                if (remainingTime > 0)
                                {

                                    var msg = string.Format("Your account is locked out. Please try again in {0} minutes.", remainingTime);
                                    context.SetError("invalid_grant", msg);
                                    return;
                                }
                                else
                                {
                                    user.LockoutEnabled = false;
                                    user.AccessFailedCount = 0;
                                    user.LockoutEndDateUtc = null;
                                    ctx.SaveChanges();
                                }

                            }


                        }
                        // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                        // the user can not login until the lockout duration has passed

                        var totalNoOfcount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"]);
                        if (user.AccessFailedCount + 1 == totalNoOfcount && validCredentials == null)
                        {
                            user.LockoutEnabled = true;
                            user.LockoutEndDateUtc = DateTime.UtcNow;
                            ctx.SaveChanges();
                            var msg = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                            context.SetError("invalid_grant", msg);
                            return;
                        }
                        else if (user.AccessFailedCount < totalNoOfcount && validCredentials == null)
                        {
                            user.AccessFailedCount += 1;
                            ctx.SaveChanges();
                            string message;

                            int accessFailedCount = await manager.GetAccessFailedCountAsync(user.Id);
                            int attemptsLeft =
                                Convert.ToInt32(
                                    ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString()) -
                                accessFailedCount;
                            message = string.Format(
                                "Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);


                            context.SetError("invalid_grant", message);
                            return;
                        }
                        else
                        {
                            user.LockoutEnabled = false;
                            user.AccessFailedCount = 0;
                            user.LockoutEndDateUtc = null;
                            ctx.SaveChanges();
                            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                       OAuthDefaults.AuthenticationType);
                            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                                CookieAuthenticationDefaults.AuthenticationType);

                            cookiesIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                            cookiesIdentity.AddClaim(new Claim(ClaimTypes.Sid, user.Id));
                            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));


                            //AuthenticationProperties properties = CreateProperties(user.UserName);
                            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "UserId", user.Id.ToString() },
                    { "UserName", user.UserName}
                });
                            AuthenticationTicket ticket = new AuthenticationTicket(cookiesIdentity, props);
                            context.Validated(ticket);
                            context.Request.Context.Authentication.SignIn(cookiesIdentity);


                            // When token is verified correctly, clear the access failed count used for lockout
                            //await manager.ResetAccessFailedCountAsync(user.Id);
                        }
                    }
                    else
                    {
                        context.SetError("invalid_grant", "User does not exist!");
                        return;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}