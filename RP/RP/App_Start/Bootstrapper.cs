using RP.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RP.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            //Configure AutoMapper
            AutoMapperConfiguration.Configure();
            // Configure Autofac
            AutofacWebapiConfig.Initialize(GlobalConfiguration.Configuration);
        
        }
    }
}