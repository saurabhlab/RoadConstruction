using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Mapping
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {   
                x.AddProfile<DomainToViewModelMappingProfile>();
                x.CreateMap<string, string>().ConvertUsing<NullStringConverter>();
            });
        }
    }
}