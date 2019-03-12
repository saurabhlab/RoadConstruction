using AutoMapper;
using RP.Models.Account;
using RP.Models.Company;
using RP.Models.Master;
using RP.Models.Street;
using RP.ViewModel.AccountVM;
using RP.ViewModel.Company;
using RP.ViewModel.Master;
using RP.ViewModel.Street;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RP.Mapping
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
             get { return "DomainToViewModelMappings"; }
           // get { return "DomainToViewModelMappingProfile"; }
        }

        protected  void Configure()
        {


            // Created by Saurabh Wanjari
            CreateMap<ItemMaster, ItemMasterViewModel>().ReverseMap();
            CreateMap<InwardOutward, InwardOutwardVM>().ReverseMap();
            CreateMap<Work, WorkVM>().ReverseMap();
            CreateMap<City, CityVM>().ReverseMap();
            CreateMap<MasterLookup, DropDownFieldVM>().ReverseMap();
            CreateMap<AccessPermission, AccessPermissionVM>().ReverseMap();
            CreateMap<List<InwardOutward>, List<InwardOutwardVM>>().ReverseMap();
            CreateMap<Street, StreetVM>().ReverseMap();

            //CreateMap<Street, StreetVM>()
            //    .ForMember(x => x.IsSelect, opt => opt.Ignore());
            //CreateMap<List<Street>, List<StreetVM>>().ReverseMap()
            //    .ForMember(x => x.IsSelect, opt => opt.Ignore());
            //  CreateMap<AccessPermission, AccessPermissionVM>()
            //   .ForMember(x => x.UserType, opt => opt.Ignore());
            //.ForMember(vm => vm.IsDeleted, map => map.MapFrom(m => m.IsSold));
            // Mapper.AssertConfigurationIsValid();

        }
    }
    public class NullStringConverter : ITypeConverter<string, string>
    {
        public string Convert(string source, ResolutionContext context)
        {
            return source ?? string.Empty;
        }

        public string Convert(string source, string destination, ResolutionContext context)
        {
            return source ?? string.Empty;
        }
    }
}