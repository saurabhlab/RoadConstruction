using RP.Models;
using RP.Models.Account;
using RP.Models.Admin;
using RP.Models.Company;
using RP.Models.Master;
using RP.Models.Street;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RP.Context
{
    public class DataContext:DbContext
    {
        public DataContext():base("RPConnection")
        {
            Database.SetInitializer<DataContext>(null);
        }
        public static DataContext Create()
        {
            return new DataContext();
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }


        public IDbSet<Student> Students { get; set; }

        public IDbSet<ItemMaster> ItemMasters { get; set; }
        public IDbSet<AccessPermission> AccessPermissions { get; set; }
        public IDbSet<Contact> Contacts { get; set; }
        public IDbSet<CompanyDetail> CompanyDetails { get; set; }
        public IDbSet<City> City{ get; set; }
        public IDbSet<InwardOutward> InwardOutwards { get; set; }
        public IDbSet<Work> Works { get; set; }
        public IDbSet<MasterLookup> MasterLookups { get; set; }
        public IDbSet<UserTypeMapping> UserTypeMappings { get; set; }
        public IDbSet<ManagerSupervisorTypeMapping> ManagerSupervisorTypeMappings { get; set; }
        public IDbSet<ItemOldQuantity> ItemOldQuantitys { get; set; }
        public IDbSet<Street> Streets { get; set; }
        public IDbSet<AssignStreet> AssignStreets { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new PropertyInfoConfiguration());

            modelBuilder.Entity<Student>();
            modelBuilder.Entity<AccessPermission>();
            modelBuilder.Entity<ItemMaster>();
            modelBuilder.Entity<Contact>();
            modelBuilder.Entity<CompanyDetail>();
            modelBuilder.Entity<City>();
            modelBuilder.Entity<InwardOutward>();
            modelBuilder.Entity<Work>();
            modelBuilder.Entity<MasterLookup>();
            modelBuilder.Entity<UserTypeMapping>();
            modelBuilder.Entity<ManagerSupervisorTypeMapping>();
            modelBuilder.Entity<ItemOldQuantity>();
            modelBuilder.Entity<Street>();
                modelBuilder.Entity<AssignStreet>();
            base.OnModelCreating(modelBuilder);

            //  modelBuilder.Configurations.Add(new AgentPackageAddonsMappingConfiguration());
        }
    }
}