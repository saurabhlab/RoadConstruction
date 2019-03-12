using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using RP.Context;
using RP.Controllers;
using RP.IRepository;
using RP.IServices;
using RP.Repository;
using RP.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace RP.App_Start
{
    public class AutofacWebapiConfig
    {
        public static IContainer Container;
        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var controllersAssembly = Assembly.GetAssembly(typeof(HomeController));

            builder.RegisterControllers(controllersAssembly);


            //Dependancies of All repository file
            // EF DataContext
            builder.RegisterType<DataContext>()
                   .As<DbContext>()
                   .InstancePerRequest();

            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .InstancePerRequest();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(EntityBaseRepository<>))
                   .As(typeof(IEntityBaseRepository<>))
                   .InstancePerRequest();



            //dependancies of all Service and interface
            builder.RegisterType<AccountService>()
                .As<IAccountService>()
                .InstancePerRequest();

            builder.RegisterType<InwardOutwardService>()
                .As<IInwardOutwardService>()
                .InstancePerRequest();

            builder.RegisterType<CommonService>()
                .As<ICommonService>()
                .InstancePerRequest();

            builder.RegisterType<StreetService>()
                .As<IStreetService>()
                .InstancePerRequest();
            

                        Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            return Container;
        }
    }
}