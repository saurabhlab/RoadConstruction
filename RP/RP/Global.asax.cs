using AutoMapper;
using RP.App_Start;
using RP.Context;
using RP.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {

            var config = GlobalConfiguration.Configuration;

            AreaRegistration.RegisterAllAreas();
           // GlobalConfiguration.Configure(WebApiConfig.Register);            
            WebApiConfig.Register(config);
            Bootstrapper.Run();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.EnsureInitialized();  
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());
        }
    }


}
